using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

public static class LogTool
{
    // 配置常量
    private const int MaxQueueSize = 100000;//日志队列的最大容量
    private const int BufferSize = 1000;//缓冲区行数：每次写入文件的日志条数
    private const int MaxRetries = 3;//文件写入和日志轮转的最大重试次数
    private const int FileBufferSize = 64 * 1024;//文件流缓冲区大小(64KB)
    private const long MaxLogSize = 5 * 1024 * 1024;//单个日志文件最大尺寸(5MB)
    private const int RotationRetryDelay = 100;//日志轮转重试延迟(毫秒)

    // 核心数据结构
    private static readonly BlockingCollection<string> _logQueue = new BlockingCollection<string>(new ConcurrentQueue<string>(), MaxQueueSize);// 日志队列

    private static List<string> _frontBuffer = new List<string>(BufferSize);// 主缓冲区
    private static List<string> _backBuffer = new List<string>(BufferSize);// 双缓冲区
    private static readonly object _bufferLock = new object();// 缓冲区交换锁
    private static readonly object _fileLock = new object();// 文件写入锁

    // 文件资源管理
    private static FileStream _fileStream;// 文件流
    private static StreamWriter _streamWriter;// 流写入器

    // 文件路径配置
    private static readonly string _logPath = Path.Combine(Application.StartupPath, "Logs");// 日志目录
    private static readonly string _logFilePath = Path.Combine(_logPath, "Application.log");// 当前日志文件路径
    private static readonly string _backupDir = Path.Combine(_logPath, "BackUp");// 备份目录

    /// <summary>
    /// 静态构造函数，初始化日志系统
    /// </summary>
    static LogTool()
    {
        //创建日志目录和备份目录
        Directory.CreateDirectory(_logPath);
        Directory.CreateDirectory(_backupDir);
        // 初始化文件流
        InitializeFileStream();
        // 注册应用程序退出事件，确保日志被正确刷新和关闭
        AppDomain.CurrentDomain.ProcessExit += (s, e) => Shutdown();
        // 启动日志处理任务
        Task.Factory.StartNew(ProcessLogs, TaskCreationOptions.LongRunning);
    }

    #region 公共接口
    /// <summary>
    /// 将日志内容添加到日志队列中，供后台任务异步处理。
    /// </summary>
    /// <param name="content">日志内容</param>
    public static void Log(string content)
    {
        try
        {
            var logEntry = FormatLogEntry(content);// 格式化日志条目
            _logQueue.TryAdd(logEntry, 100);// 尝试将日志添加到队列（100ms超时）
        }
        catch
        {
            // 忽略日志队列满异常
        }
    }

    /// <summary>
    /// 打开当前日志文件
    /// </summary>
    /// <returns>是否成功打开</returns>
    public static bool OpenLogFile()
    {
        try
        {
            // 确保文件存在
            if (!File.Exists(_logFilePath))
            {
                return false;
            }

            // 使用默认程序打开文件
            Process.Start(new ProcessStartInfo
            {
                FileName = _logFilePath,
                UseShellExecute = true
            });

            return true;
        }
        catch (Exception ex)
        {           
            return false;
        }
    }    
    #endregion

    #region 核心处理逻辑
    /// <summary>
    /// 持续从日志队列中读取日志，填充缓冲区，并将其写入日志文件。
    /// </summary>
    private static void ProcessLogs()
    {
        // 无限循环处理日志队列
        while (true)
        {
            try
            {
                var writeBuffer = SwapBuffers();// 交换缓冲区
                FillBuffer(writeBuffer);// 填充缓冲区
                WriteToFile(writeBuffer);// 写入文件
                ReturnBuffer(writeBuffer);// 归还缓冲区
            }
            catch
            {
               
            }
        }
    }

    /// <summary>
    /// 交换前后缓冲区，返回当前满的缓冲区以供写入。
    /// </summary>
    /// <returns></returns>
    private static List<string> SwapBuffers()
    {
        lock (_bufferLock)
        {
            // 交换前后缓冲区
            var buffer = _frontBuffer;
            _frontBuffer = _backBuffer;
            _backBuffer = new List<string>(BufferSize);
            return buffer;
        }
    }

    /// <summary>
    /// 填充缓冲区直到达到指定大小或超时。
    /// </summary>
    /// <param name="buffer"></param>
    private static void FillBuffer(List<string> buffer)
    {
        var cts = new CancellationTokenSource(100);// 100ms超时取消
        // 填充直到缓冲区满或超时
        while (buffer.Count < BufferSize && !cts.Token.IsCancellationRequested)
        {
            // 尝试从日志队列中取出日志
            if (_logQueue.TryTake(out var item, 50))
            {
                buffer.Add(item);
            }
        }
    }

    /// <summary>
    /// 清空并归还缓冲区，防止内存泄漏。
    /// </summary>
    /// <param name="buffer"></param>
    private static void ReturnBuffer(List<string> buffer)
    {
        buffer.Clear();// 清空缓冲区
        lock (_bufferLock)
        {
            // 如果缓冲区过大，重新分配以防内存泄漏。
            if (_backBuffer.Capacity > BufferSize * 2)
            {
                _backBuffer = new List<string>(BufferSize);
            }
            _backBuffer = buffer;// 将缓冲区放回后端       
        }
    }
    #endregion

    #region 增强文件操作
    /// <summary>
    /// 将日志批量写入文件，包含重试和日志轮转机制。
    /// </summary>
    /// <param name="logs"></param>
    private static void WriteToFile(List<string> logs)
    {
        if (logs.Count == 0) return;// 无日志无需写入

        lock (_fileLock)
        {
            CheckAndRotateLog();// 检查并执行日志轮转

            // 重试写入机制
            for (int retry = 0; retry < MaxRetries; retry++)
            {
                try
                {
                    if (_streamWriter == null) InitializeFileStream();// 确保文件流已初始化

                    // 使用StringBuilder优化内存分配
                    var sb = new StringBuilder(logs.Count * 256);
                    foreach (var log in logs)
                    {
                        sb.Append(log);      // 原始日志内容
                        sb.AppendLine();     // 自动添加当前系统的换行符
                    }

                    //写入并刷新
                    _streamWriter.Write(sb.ToString());
                    _streamWriter.Flush();
                    return;// 成功写入，退出重试循环
                }
                catch
                {
                    HandleWriteFailure();// 处理写入失败
                    Thread.Sleep(RotationRetryDelay * (retry + 1));// 延迟后重试
                }
            }
        }
    }

    /// <summary>
    /// 检查日志文件大小，若超过限制则进行轮转。
    /// </summary>
    private static void CheckAndRotateLog()
    {
        try
        {
            if (!File.Exists(_logFilePath)) return;// 跳过不存在的文件

            // 检查文件大小
            var fileInfo = new FileInfo(_logFilePath);
            if (fileInfo.Length < MaxLogSize) return;

            // 原子性操作序列
            CloseFileStream();// 关闭当前文件流
            RotateFileWithRetry();// 轮转文件
            InitializeFileStream();// 初始化新文件流
        }
        catch
        {
            // 恢复文件流保证后续写入
            InitializeFileStream();
        }
    }

    /// <summary>
    /// 尝试多次轮转日志文件，处理可能的文件访问冲突。
    /// </summary>
    private static void RotateFileWithRetry()
    {
        for (int retry = 0; retry < MaxRetries; retry++)
        {
            try
            {
                // 生成备份文件名（带时间戳）
                var backupName = $"Application_{DateTime.Now:yyyyMMddHHmmss}.log";
                var backupPath = Path.Combine(_backupDir, backupName);

                File.Move(_logFilePath, backupPath);// 移动文件到备份目录
                return;// 成功则退出
            }
            catch
            {                
                if (retry == MaxRetries - 1) throw;// 最后一次重试失败则抛出异常
                Thread.Sleep(RotationRetryDelay * (retry + 1));// 延迟后重试
            }
        }
    }
    #endregion

    #region 资源管理
    /// <summary>
    /// 初始化文件流和流写入器。
    /// </summary>
    private static void InitializeFileStream()
    {
        try
        {
            _fileStream = new FileStream(
                _logFilePath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.ReadWrite,
                FileBufferSize);

            _streamWriter = new StreamWriter(_fileStream, Encoding.UTF8)
            {
                AutoFlush = false
            };
        }
        catch
        {
            
        }
    }

    /// <summary>
    /// 关闭并释放文件流和流写入器资源。
    /// </summary>
    private static void CloseFileStream()
    {
        try
        {
            _streamWriter?.Dispose();
            _fileStream?.Dispose();
        }
        finally
        {
            _streamWriter = null;
            _fileStream = null;
        }
    }

    /// <summary>
    /// 处理写入失败，尝试重新初始化文件流。
    /// </summary>
    private static void HandleWriteFailure()
    {
        CloseFileStream();
        InitializeFileStream();
    }

    /// <summary>
    /// 在应用程序退出时，确保所有日志被正确刷新并关闭文件流。
    /// </summary>
    private static void Shutdown()
    {
        FlushBuffers();
        CloseFileStream();
    }
    #endregion

    #region 辅助方法
    /// <summary>
    /// 格式化日志条目，添加时间戳并清理内容。
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private static string FormatLogEntry(string content)
    {
        // 移除非必要的结尾符号
        return $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffZ}|{content.Trim()}";
    }

    /// <summary>
    /// 在应用程序退出时，确保所有缓冲区日志被写入文件。
    /// </summary>
    private static void FlushBuffers()
    {
        lock (_bufferLock)
        {
            WriteToFile(_frontBuffer);
            WriteToFile(_backBuffer);
            _frontBuffer.Clear();
            _backBuffer.Clear();
        }
    }
    #endregion
}
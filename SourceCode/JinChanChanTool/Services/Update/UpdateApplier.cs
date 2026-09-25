using JinChanChanTool.DataClass;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace JinChanChanTool.Services.Update
{
    /// <summary>
    /// 更新应用器：在独立进程（--apply-update）中运行，负责把 staging 中的新文件替换到安装目录。
    /// 关键约束：主程序进程在运行中会锁定 JinChanChanTool.exe 与若干 dll，因此替换必须等待其退出；
    /// 任何一个文件失败都会整批回滚，保证安装目录不会停留在“半更新”状态。
    /// </summary>
    public class UpdateApplier
    {
        /// <summary>等待主进程退出的最长时间（秒）</summary>
        private const int WaitForParentExitSeconds = 60;

        /// <summary>等待文件锁释放的重试次数</summary>
        private const int FileRetryCount = 10;

        /// <summary>文件重试间隔（毫秒）</summary>
        private const int FileRetryDelayMs = 500;

        /// <summary>更新完成标记文件的保留天数</summary>
        private const int ResultFileRetentionDays = 7;

        /// <summary>被替换文件的临时重命名后缀（用于替换正在运行的映像）</summary>
        private const string ReplacedFileSuffix = ".old";

        private readonly string _logFilePath;

        /// <summary>
        /// 已成功替换的文件记录，用于失败回滚。
        /// RenamedOldPath 记录“旧映像被重命名到何处”，回滚时据此还原。
        /// </summary>
        private sealed class AppliedFile
        {
            public required UpdateFileEntry Entry { get; init; }

            public string? RenamedOldPath { get; set; }
        }

        /// <summary>
        /// 创建更新应用器。
        /// </summary>
        public UpdateApplier()
        {
            _logFilePath = BuildLogFilePath();
        }

        /// <summary>
        /// 执行一次更新：读取计划 → 等待主进程退出 → 备份 → 替换 → 重启 → 清理。
        /// </summary>
        /// <param name="planPath">计划文件路径</param>
        /// <returns>是否成功</returns>
        public bool Apply(string planPath)
        {
            UpdateApplyPlan? plan = null;

            try
            {
                Log($"更新进程启动，计划文件：{planPath}");

                if (!File.Exists(planPath))
                {
                    Log("计划文件不存在，更新中止。");
                    return false;
                }

                plan = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateApplyPlan>(File.ReadAllText(planPath));
                if (plan == null)
                {
                    Log("计划文件解析失败，更新中止。");
                    return false;
                }

                if (plan.SchemaVersion != UpdateApplyPlan.CurrentSchemaVersion)
                {
                    Log($"计划结构版本不匹配（{plan.SchemaVersion}），更新中止。");
                    return false;
                }

                if (!WaitForParentExit(plan.ParentProcessId))
                {
                    Log("等待主程序退出超时，更新中止。");
                    WriteResult(plan, success: false, message: "等待主程序退出超时，更新未执行。");
                    return false;
                }

                // 主进程退出后再短暂等待，让文件句柄彻底释放
                Thread.Sleep(FileRetryDelayMs * 2);

                List<AppliedFile> appliedFiles = new List<AppliedFile>();

                if (!ApplyFiles(plan, appliedFiles))
                {
                    Rollback(plan, appliedFiles);
                    WriteResult(plan, success: false, message: "更新失败，已回滚到更新前的状态。");
                    return false;
                }

                WriteResult(plan, success: true, message: $"已更新到 {plan.TargetVersion}。");
                Log($"更新完成，目标版本：{plan.TargetVersion}，文件数：{appliedFiles.Count}");

                CleanupAfterSuccess(plan);
                return true;
            }
            catch (Exception ex)
            {
                Log($"更新过程中出现异常：{ex}");
                Debug.WriteLine($"[UpdateApplier] 更新异常：{ex.Message}");

                if (plan != null)
                {
                    WriteResult(plan, success: false, message: $"更新异常：{ex.Message}");
                }

                return false;
            }
            finally
            {
                TryRestartApplication(plan);
            }
        }

        /// <summary>
        /// 等待发起更新的主进程退出。
        /// </summary>
        private bool WaitForParentExit(int parentProcessId)
        {
            if (parentProcessId <= 0)
            {
                return true;
            }

            try
            {
                using Process parent = Process.GetProcessById(parentProcessId);
                Log($"等待主进程退出，PID={parentProcessId}...");

                if (!parent.WaitForExit(WaitForParentExitSeconds * 1000))
                {
                    Log($"等待主进程退出超时（{WaitForParentExitSeconds} 秒）。");
                    return false;
                }

                return true;
            }
            catch (ArgumentException)
            {
                // 进程已经不存在，说明主程序已退出
                return true;
            }
            catch (Exception ex)
            {
                Log($"等待主进程退出时出现异常：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 逐个文件执行替换，失败时返回 false（由调用方回滚）。
        /// 关键点：更新进程自身就是从被替换的 exe/dll 启动的，Windows 不允许覆盖正在运行的映像，
        /// 因此覆盖已有文件时先“重命名旧文件 + 写入新文件”，重命名后旧映像的句柄随之释放。
        /// </summary>
        private bool ApplyFiles(UpdateApplyPlan plan, List<AppliedFile> appliedFiles)
        {
            foreach (UpdateFileEntry entry in plan.Files)
            {
                string sourcePath = Path.Combine(
                    plan.StagingDirectory,
                    entry.RelativePath.Replace(UpdateConstants.RelativePathSeparator, Path.DirectorySeparatorChar));

                string targetPath = Path.Combine(
                    plan.InstallDirectory,
                    entry.RelativePath.Replace(UpdateConstants.RelativePathSeparator, Path.DirectorySeparatorChar));

                if (!File.Exists(sourcePath))
                {
                    Log($"暂存文件缺失：{sourcePath}");
                    return false;
                }

                if (!IsInsideDirectory(plan.InstallDirectory, targetPath))
                {
                    Log($"目标路径越界，已拒绝：{targetPath}");
                    return false;
                }

                AppliedFile appliedFile = new AppliedFile { Entry = entry };

                // 覆盖前先备份原文件，新增文件无需备份
                if (File.Exists(targetPath))
                {
                    string backupPath = Path.Combine(
                        plan.BackupDirectory,
                        entry.RelativePath.Replace(UpdateConstants.RelativePathSeparator, Path.DirectorySeparatorChar));

                    if (!TryBackupFile(targetPath, backupPath))
                    {
                        return false;
                    }
                }

                if (!TryReplaceFile(sourcePath, targetPath, appliedFile))
                {
                    return false;
                }

                // 替换后校验哈希，防止写入被截断或被占用导致内容不完整
                if (!string.IsNullOrEmpty(entry.Sha256))
                {
                    string actualHash = UpdatePackageExtractor.ComputeSha256(targetPath);
                    if (!string.Equals(actualHash, entry.Sha256, StringComparison.OrdinalIgnoreCase))
                    {
                        Log($"文件校验失败：{entry.RelativePath}");
                        return false;
                    }
                }

                appliedFiles.Add(appliedFile);
                Log($"已更新：{entry.RelativePath}（{entry.Action}）");
            }

            return true;
        }

        /// <summary>
        /// 回滚已经替换的文件，尽力恢复更新前状态。
        /// </summary>
        private void Rollback(UpdateApplyPlan plan, List<AppliedFile> appliedFiles)
        {
            Log($"开始回滚，共 {appliedFiles.Count} 个文件。");

            for (int i = appliedFiles.Count - 1; i >= 0; i--)
            {
                AppliedFile appliedFile = appliedFiles[i];
                UpdateFileEntry entry = appliedFile.Entry;

                string targetPath = Path.Combine(
                    plan.InstallDirectory,
                    entry.RelativePath.Replace(UpdateConstants.RelativePathSeparator, Path.DirectorySeparatorChar));

                string backupPath = Path.Combine(
                    plan.BackupDirectory,
                    entry.RelativePath.Replace(UpdateConstants.RelativePathSeparator, Path.DirectorySeparatorChar));

                try
                {
                    if (entry.Action == UpdateFileAction.Replace && File.Exists(backupPath))
                    {
                        File.Copy(backupPath, targetPath, overwrite: true);
                        Log($"已回滚：{entry.RelativePath}");
                    }
                    else if (entry.Action == UpdateFileAction.Add && File.Exists(targetPath))
                    {
                        // 新增文件直接删除，恢复到“不存在”的状态
                        File.Delete(targetPath);
                        Log($"已移除新增文件：{entry.RelativePath}");
                    }

                    // 清理替换过程中留下的 .old 映像
                    if (!string.IsNullOrEmpty(appliedFile.RenamedOldPath))
                    {
                        TryDeleteFile(appliedFile.RenamedOldPath);
                    }
                }
                catch (Exception ex)
                {
                    // 回滚单个文件失败不应中断整体回滚，继续处理其余文件
                    Log($"回滚失败（{entry.RelativePath}）：{ex.Message}");
                    Debug.WriteLine($"[UpdateApplier] 回滚失败（{entry.RelativePath}）：{ex.Message}");
                }
            }
        }

        /// <summary>
        /// 备份单个文件到备份目录。
        /// </summary>
        private bool TryBackupFile(string sourcePath, string backupPath)
        {
            try
            {
                string? directory = Path.GetDirectoryName(backupPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.Copy(sourcePath, backupPath, overwrite: true);
                return true;
            }
            catch (Exception ex)
            {
                Log($"备份文件失败（{sourcePath}）：{ex.Message}");
                Debug.WriteLine($"[UpdateApplier] 备份文件失败（{sourcePath}）：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 用暂存文件替换目标文件。
        /// 覆盖已有文件时采用“旧文件重命名为 .old → 写入新文件”的顺序：
        /// 更新进程自身正运行在被替换的 exe/dll 上，直接覆盖会被系统拒绝（Access denied），
        /// 而重命名后旧映像的句柄随之释放，随后即可写入新内容。
        /// </summary>
        private bool TryReplaceFile(string sourcePath, string targetPath, AppliedFile appliedFile)
        {
            bool targetExists = File.Exists(targetPath);
            string renamedOldPath = targetPath + ReplacedFileSuffix;

            for (int attempt = 1; attempt <= FileRetryCount; attempt++)
            {
                try
                {
                    string? directory = Path.GetDirectoryName(targetPath);
                    if (!string.IsNullOrEmpty(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // 上一轮遗留的 .old 先清掉，避免重命名失败
                    TryDeleteFile(renamedOldPath);

                    if (targetExists)
                    {
                        // 重命名可能因安全软件短暂占用而失败，交由重试处理
                        File.Move(targetPath, renamedOldPath);
                        appliedFile.RenamedOldPath = renamedOldPath;
                    }

                    // 先写临时文件再改名，避免中途中断留下半截的目标文件
                    string tempPath = targetPath + ".new";
                    File.Copy(sourcePath, tempPath, overwrite: true);
                    File.Move(tempPath, targetPath, overwrite: true);
                    return true;
                }
                catch (IOException ex)
                {
                    Log($"替换文件重试 {attempt}/{FileRetryCount}（{targetPath}）：{ex.Message}");
                    Thread.Sleep(FileRetryDelayMs);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Log($"替换文件重试 {attempt}/{FileRetryCount}（{targetPath}）：{ex.Message}");
                    Thread.Sleep(FileRetryDelayMs);
                }
                catch (Exception ex)
                {
                    Log($"替换文件失败（{targetPath}）：{ex.Message}");
                    Debug.WriteLine($"[UpdateApplier] 替换文件失败（{targetPath}）：{ex.Message}");
                    return false;
                }
            }

            // 多次重试仍失败：把旧文件还原回去，清理临时文件后放弃
            if (appliedFile.RenamedOldPath != null && !File.Exists(targetPath))
            {
                try
                {
                    File.Move(appliedFile.RenamedOldPath, targetPath);
                    appliedFile.RenamedOldPath = null;
                    Log($"已还原未能替换的文件：{targetPath}");
                }
                catch (Exception ex)
                {
                    Log($"还原失败（{targetPath}）：{ex.Message}");
                }
            }

            TryDeleteFile(targetPath + ".new");
            return false;
        }

        /// <summary>
        /// 更新成功后清理暂存目录与下载包。
        /// 注意：必须保留 Backups、Defaults、Logs，否则回滚能力、配置增量合并与更新日志会一并丢失。
        /// 同时尽力删除替换过程中产生的 .old 映像；更新进程自身仍在运行的 exe 会删除失败，
        /// 由下次启动时的清理逻辑兜底。
        /// </summary>
        private void CleanupAfterSuccess(UpdateApplyPlan plan)
        {
            string updateRootDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "JinChanChanTool",
                UpdateConstants.UpdateRootDirectoryName);

            foreach (string directory in plan.CleanupDirectories)
            {
                if (!IsInsideDirectory(updateRootDirectory, directory))
                {
                    Log($"跳过不在更新目录内的清理项：{directory}");
                    continue;
                }

                TryDeleteDirectory(directory);
            }

            foreach (UpdateFileEntry entry in plan.Files)
            {
                string targetPath = Path.Combine(
                    plan.InstallDirectory,
                    entry.RelativePath.Replace(UpdateConstants.RelativePathSeparator, Path.DirectorySeparatorChar));

                TryDeleteFile(targetPath + ReplacedFileSuffix);
            }
        }

        /// <summary>
        /// 更新结束后重新启动主程序。
        /// </summary>
        private void TryRestartApplication(UpdateApplyPlan? plan)
        {
            if (plan == null)
            {
                return;
            }

            try
            {
                string executablePath = Path.Combine(plan.InstallDirectory, plan.ExecutableRelativePath);
                if (!File.Exists(executablePath))
                {
                    Log($"未找到主程序，无法重启：{executablePath}");
                    return;
                }

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    WorkingDirectory = plan.InstallDirectory,
                    UseShellExecute = true
                };

                Process.Start(startInfo);
                Log("已重新启动主程序。");
            }
            catch (Exception ex)
            {
                Log($"重新启动主程序失败：{ex.Message}");
                Debug.WriteLine($"[UpdateApplier] 重新启动主程序失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 写出更新结果标记文件，供下一次启动时提示用户，并顺带清理过期标记。
        /// </summary>
        private void WriteResult(UpdateApplyPlan plan, bool success, string message)
        {
            try
            {
                string resultDirectory = GetResultDirectory();
                Directory.CreateDirectory(resultDirectory);

                string fileName = $"{UpdateConstants.UpdateResultFilePrefix}{plan.TargetVersion}_{DateTime.Now:yyyyMMddHHmmss}.json";

                UpdateResultRecord record = new UpdateResultRecord
                {
                    TargetVersion = plan.TargetVersion,
                    IsSuccess = success,
                    Message = message,
                    CompletedAt = DateTimeOffset.Now
                };
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(
                    record,
                    Newtonsoft.Json.Formatting.Indented);

                File.WriteAllText(Path.Combine(resultDirectory, fileName), json, Encoding.UTF8);

                CleanupExpiredResults(resultDirectory);
            }
            catch (Exception ex)
            {
                Log($"写入更新结果失败：{ex.Message}");
                Debug.WriteLine($"[UpdateApplier] 写入更新结果失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 清理超过保留期的更新结果标记。
        /// </summary>
        private static void CleanupExpiredResults(string resultDirectory)
        {
            try
            {
                foreach (string file in Directory.GetFiles(
                    resultDirectory,
                    UpdateConstants.UpdateResultFilePrefix + "*.json"))
                {
                    if (File.GetCreationTimeUtc(file) < DateTime.UtcNow.AddDays(-ResultFileRetentionDays))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateApplier] 清理更新结果标记失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 判断目标路径是否位于指定目录内部，防止计划文件被篡改后写到目录之外。
        /// </summary>
        private static bool IsInsideDirectory(string directory, string targetPath)
        {
            try
            {
                string normalizedDirectory = Path.GetFullPath(directory)
                    .TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
                string normalizedTarget = Path.GetFullPath(targetPath);

                return normalizedTarget.StartsWith(normalizedDirectory, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateApplier] 路径校验失败：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 更新进程自身的日志文件路径（放在用户目录，不占用安装目录）。
        /// </summary>
        private static string BuildLogFilePath()
        {
            return Path.Combine(
                GetUpdaterLogDirectory(),
                $"updater_{DateTime.Now:yyyyMMdd}.log");
        }

        /// <summary>
        /// 更新流程的日志目录。
        /// </summary>
        private static string GetUpdaterLogDirectory()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "JinChanChanTool",
                UpdateConstants.UpdateRootDirectoryName,
                UpdateConstants.LogsDirectoryName);
        }

        /// <summary>
        /// 更新结果标记文件所在目录（与日志同目录，便于用户一并查看）。
        /// </summary>
        private static string GetResultDirectory()
        {
            return GetUpdaterLogDirectory();
        }

        /// <summary>
        /// 记录更新过程日志：更新进程独立于主程序，需要自己的日志便于排查失败原因。
        /// </summary>
        private void Log(string message)
        {
            try
            {
                string? directory = Path.GetDirectoryName(_logFilePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.AppendAllText(
                    _logFilePath,
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}|{message}{Environment.NewLine}",
                    Encoding.UTF8);

                Debug.WriteLine($"[UpdateApplier] {message}");
            }
            catch (Exception ex)
            {
                // 日志失败不能影响更新流程本身
                Debug.WriteLine($"[UpdateApplier] 写入日志失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 尽力删除文件。
        /// </summary>
        private static void TryDeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateApplier] 删除文件失败（{filePath}）：{ex.Message}");
            }
        }

        /// <summary>
        /// 尽力删除目录。
        /// </summary>
        private static void TryDeleteDirectory(string directory)
        {
            try
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateApplier] 删除目录失败（{directory}）：{ex.Message}");
            }
        }
    }
}

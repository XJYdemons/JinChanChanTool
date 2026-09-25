using JinChanChanTool.DataClass;
using System.Security.Cryptography;

namespace JinChanChanTool.Services.Update
{
    /// <summary>
    /// 更新包解压与差异清单生成。
    /// 关键约定：解压目录与安装目录逐文件比较，只有内容真正变化的文件才进入更新清单，
    /// 用户态文件（设置、阵容、纠正列表等）永不进入清单，从而保证升级后用户数据不丢。
    /// </summary>
    public class UpdatePackageExtractor
    {
        /// <summary>解压 staging 目录名</summary>
        public const string StagingDirectoryName = "staging";

        /// <summary>逐文件比较时的读取缓冲区大小（1MB）</summary>
        private const int CompareBufferSize = 1024 * 1024;

        /// <summary>计算 SHA256 时的读取缓冲区大小（1MB）</summary>
        private const int HashBufferSize = 1024 * 1024;

        /// <summary>zip 解压的根层探测最大深度</summary>
        private const int MaxRootProbeDepth = 3;

        /// <summary>
        /// 把更新包解压到指定 staging 目录。若目录已存在会先清空，避免残留旧文件影响差异判断。
        /// </summary>
        /// <param name="packagePath">更新包 zip 路径</param>
        /// <param name="stagingDirectory">staging 目录</param>
        /// <param name="cancellationToken">取消令牌</param>
        public async Task ExtractAsync(
            string packagePath,
            string stagingDirectory,
            CancellationToken cancellationToken = default)
        {
            if (!File.Exists(packagePath))
            {
                throw new FileNotFoundException("更新包不存在。", packagePath);
            }

            if (Directory.Exists(stagingDirectory))
            {
                Directory.Delete(stagingDirectory, true);
            }

            Directory.CreateDirectory(stagingDirectory);

            await Task.Run(() =>
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(
                    packagePath,
                    stagingDirectory,
                    overwriteFiles: true);
            }, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// 校验解压结果是否落在 staging 目录内。
        /// .NET 的解压 API 本身会拒绝逃逸条目，这里再做一次显式检查：
        /// 一旦发现异常即中止更新，避免把可疑内容继续送进替换流程。
        /// </summary>
        /// <param name="stagingDirectory">staging 目录</param>
        /// <returns>是否存在越界文件</returns>
        public static bool HasEscapedEntries(string stagingDirectory)
        {
            string normalizedRoot = Path.GetFullPath(stagingDirectory)
                .TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

            foreach (string filePath in Directory.EnumerateFiles(stagingDirectory, "*", SearchOption.AllDirectories))
            {
                string normalizedFile = Path.GetFullPath(filePath);
                if (!normalizedFile.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 定位解压后的实际根目录。
        /// 当前发布包为“解压即覆盖安装目录”的结构，根目录即 staging 本身；
        /// 若将来发布包改为带一层顶层目录，这里会自动向内探测，避免更新路径整体错位。
        /// </summary>
        /// <param name="stagingDirectory">staging 目录</param>
        /// <returns>包含程序文件的根目录</returns>
        public string ResolvePackageRoot(string stagingDirectory)
        {
            string current = stagingDirectory;

            for (int depth = 0; depth < MaxRootProbeDepth; depth++)
            {
                // 只要当前层出现了主程序或 Resources 目录，就认定它是包根
                if (File.Exists(Path.Combine(current, UpdateConstants.ExecutableFileName)) ||
                    Directory.Exists(Path.Combine(current, UpdateConstants.ResourcesDirectoryName)))
                {
                    return current;
                }

                string[] entries = Directory.GetFileSystemEntries(current);
                if (entries.Length == 0)
                {
                    return current;
                }

                // 仅当目录下只有一个子目录时才向内探测，避免误判多目录结构
                if (entries.Length == 1 && Directory.Exists(entries[0]))
                {
                    current = entries[0];
                    continue;
                }

                return current;
            }

            return current;
        }

        /// <summary>
        /// 生成待更新文件清单：遍历包内所有文件，跳过用户态文件，仅保留与安装目录内容不同的文件。
        /// </summary>
        /// <param name="packageRoot">包根目录</param>
        /// <param name="installDirectory">安装目录</param>
        /// <param name="isUserStatePath">判断相对路径是否为用户态文件的委托</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>待更新文件清单</returns>
        public async Task<List<UpdateFileEntry>> BuildChangeListAsync(
            string packageRoot,
            string installDirectory,
            Func<string, bool> isUserStatePath,
            CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                List<UpdateFileEntry> entries = new List<UpdateFileEntry>();
                string normalizedPackageRoot = Path.GetFullPath(packageRoot);
                string normalizedInstallDirectory = Path.GetFullPath(installDirectory);

                foreach (string filePath in Directory.EnumerateFiles(normalizedPackageRoot, "*", SearchOption.AllDirectories))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string relativePath = Path
                        .GetRelativePath(normalizedPackageRoot, filePath)
                        .Replace(Path.DirectorySeparatorChar, UpdateConstants.RelativePathSeparator);

                    if (isUserStatePath(relativePath))
                    {
                        // 用户态文件不参与覆盖，交由配置增量合并处理
                        continue;
                    }

                    string targetPath = Path.Combine(
                        normalizedInstallDirectory,
                        relativePath.Replace(UpdateConstants.RelativePathSeparator, Path.DirectorySeparatorChar));

                    bool targetExists = File.Exists(targetPath);
                    if (targetExists && IsSameContent(filePath, targetPath))
                    {
                        // 内容一致的文件无需写入，缩短更新窗口、降低失败概率
                        continue;
                    }

                    entries.Add(new UpdateFileEntry
                    {
                        RelativePath = relativePath,
                        Action = targetExists ? UpdateFileAction.Replace : UpdateFileAction.Add,
                        Size = new FileInfo(filePath).Length,
                        Sha256 = ComputeSha256(filePath)
                    });
                }

                return entries;
            }, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 从更新包中抽取“用户态文件”的默认值快照。
        /// 用户态文件在更新时不会被覆盖，但新版本可能给它们增加了新字段/新条目，
        /// 因此这里先把包内的默认值留存一份，供下次启动时的配置增量合并使用。
        /// </summary>
        /// <param name="packageRoot">包根目录</param>
        /// <param name="snapshotDirectory">快照输出目录</param>
        /// <param name="isUserStatePath">判断相对路径是否为用户态文件的委托</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>写入快照的文件数量</returns>
        public async Task<int> BuildUserStateSnapshotAsync(
            string packageRoot,
            string snapshotDirectory,
            Func<string, bool> isUserStatePath,
            CancellationToken cancellationToken = default)
        {
            if (Directory.Exists(snapshotDirectory))
            {
                Directory.Delete(snapshotDirectory, true);
            }

            Directory.CreateDirectory(snapshotDirectory);

            return await Task.Run(() =>
            {
                int copiedCount = 0;
                string normalizedPackageRoot = Path.GetFullPath(packageRoot);

                foreach (string filePath in Directory.EnumerateFiles(normalizedPackageRoot, "*", SearchOption.AllDirectories))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string relativePath = Path
                        .GetRelativePath(normalizedPackageRoot, filePath)
                        .Replace(Path.DirectorySeparatorChar, UpdateConstants.RelativePathSeparator);

                    if (!isUserStatePath(relativePath))
                    {
                        continue;
                    }

                    string destinationPath = Path.Combine(
                        snapshotDirectory,
                        relativePath.Replace(UpdateConstants.RelativePathSeparator, Path.DirectorySeparatorChar));

                    string? destinationDirectory = Path.GetDirectoryName(destinationPath);
                    if (!string.IsNullOrEmpty(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    File.Copy(filePath, destinationPath, overwrite: true);
                    copiedCount++;
                }

                return copiedCount;
            }, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 比较两个文件内容是否一致：先比长度，再逐块比对字节。
        /// </summary>
        private static bool IsSameContent(string leftPath, string rightPath)
        {
            FileInfo leftInfo = new FileInfo(leftPath);
            FileInfo rightInfo = new FileInfo(rightPath);

            if (leftInfo.Length != rightInfo.Length)
            {
                return false;
            }

            byte[] leftBuffer = new byte[CompareBufferSize];
            byte[] rightBuffer = new byte[CompareBufferSize];

            using FileStream leftStream = new FileStream(
                leftPath, FileMode.Open, FileAccess.Read, FileShare.Read, CompareBufferSize, FileOptions.SequentialScan);
            using FileStream rightStream = new FileStream(
                rightPath, FileMode.Open, FileAccess.Read, FileShare.Read, CompareBufferSize, FileOptions.SequentialScan);

            while (true)
            {
                int leftRead = leftStream.Read(leftBuffer, 0, leftBuffer.Length);
                int rightRead = rightStream.Read(rightBuffer, 0, rightBuffer.Length);

                if (leftRead != rightRead)
                {
                    return false;
                }

                if (leftRead == 0)
                {
                    return true;
                }

                if (!leftBuffer.AsSpan(0, leftRead).SequenceEqual(rightBuffer.AsSpan(0, rightRead)))
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 计算文件的 SHA256（小写十六进制）。
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>SHA256 文本</returns>
        public static string ComputeSha256(string filePath)
        {
            using FileStream stream = new FileStream(
                filePath, FileMode.Open, FileAccess.Read, FileShare.Read, HashBufferSize, FileOptions.SequentialScan);
            using SHA256 sha256 = SHA256.Create();

            byte[] hash = sha256.ComputeHash(stream);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}

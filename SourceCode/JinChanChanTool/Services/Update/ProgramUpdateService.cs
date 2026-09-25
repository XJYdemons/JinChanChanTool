using JinChanChanTool.DataClass;
using JinChanChanTool.DataClass.StaticData;
using JinChanChanTool.Services.GPUEnvironments;
using System.Diagnostics;

namespace JinChanChanTool.Services.Update
{
    /// <summary>
    /// 程序本体更新服务实现。
    /// 流程：检查 GitHub 最新发布 → 下载主程序包 → 解压到 staging → 与安装目录比对生成差异清单 →
    /// 写出待应用计划 → 由独立进程（UpdateApplier）在程序退出后完成替换并重启。
    /// 更新产物全部存放在用户目录（%LocalAppData%）下，不落在安装目录，避免被自身替换过程覆盖。
    /// </summary>
    public class ProgramUpdateService : Interface.IProgramUpdateService
    {
        /// <summary>用户数据根目录名</summary>
        private const string UserDataDirectoryName = "JinChanChanTool";

        /// <summary>下载进度映射到的百分比区间起点（下载占整体进度的 0-80）</summary>
        private const int DownloadProgressCeiling = 80;

        /// <summary>解压与比对阶段的进度区间起点</summary>
        private const int PrepareProgressStart = 80;

        /// <summary>更新前要求的最小剩余磁盘空间（字节），留出解压与备份余量</summary>
        private const long RequiredFreeSpaceBytes = 1024L * 1024L * 1024L;

        /// <summary>残留 .old 文件的后台重试次数</summary>
        private const int StaleFileRetryCount = 10;

        /// <summary>残留 .old 文件的重试间隔（毫秒）</summary>
        private const int StaleFileRetryDelayMs = 2000;

        private readonly GitHubReleaseClient _releaseClient;
        private readonly UpdatePathPolicy _pathPolicy;
        private readonly UpdatePackageExtractor _packageExtractor;
        private readonly string _installDirectory;
        private readonly string _updateRootDirectory;

        /// <summary>
        /// 创建程序更新服务。
        /// </summary>
        public ProgramUpdateService()
        {
            _releaseClient = new GitHubReleaseClient();
            _pathPolicy = new UpdatePathPolicy();
            _packageExtractor = new UpdatePackageExtractor();
            _installDirectory = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);
            _updateRootDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                UserDataDirectoryName,
                UpdateConstants.UpdateRootDirectoryName);
        }

        /// <summary>
        /// 当前安装目录。
        /// </summary>
        public string InstallDirectory => _installDirectory;

        /// <summary>
        /// 更新产物根目录（下载包、staging、备份、计划文件）。
        /// </summary>
        public string UpdateRootDirectory => _updateRootDirectory;

        /// <summary>
        /// 查询远端最新版本并判断是否存在可用更新。
        /// 预发布规则：只有正式版参与自动更新。
        /// - 当前程序为预发布版（如 7.8.0-beta）→ 不检查更新；
        /// - 远端标签为预发布（如 v7.8.0-beta）→ 正式版用户跳过，避免被推向测试版。
        /// </summary>
        public async Task<UpdateCheckResult> CheckForUpdateAsync(CancellationToken cancellationToken = default)
        {
            string currentVersionText = ProgramVersion.RawVersionText;

            // 预发布版本不参与自动更新：用户需手动下载正式版
            if (ProgramVersion.IsPrerelease)
            {
                LogTool.Log($"[ProgramUpdateService] 当前为预发布版本（{currentVersionText}），已跳过自动更新检查。");
                return UpdateCheckResult.NoUpdate(
                    currentVersionText,
                    $"当前为预发布版本（{currentVersionText}），不参与自动更新。");
            }

            try
            {
                UpdateReleaseInfo? release = await _releaseClient
                    .GetLatestReleaseAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (release == null)
                {
                    return UpdateCheckResult.Failure(currentVersionText, "无法连接到更新服务器，请检查网络后重试。");
                }

                if (release.Draft)
                {
                    return UpdateCheckResult.NoUpdate(currentVersionText, "远端最新发布仍为草稿状态，暂不更新。");
                }

                // 以标签为准判断预发布：即使发布时未勾选 prerelease，只要 tag 带后缀就不推送给正式版用户
                if (ProgramVersion.ContainsPrereleaseSuffix(release.TagName))
                {
                    LogTool.Log($"[ProgramUpdateService] 远端标签 {release.TagName} 为预发布版本，已跳过。");
                    return UpdateCheckResult.NoUpdate(
                        currentVersionText,
                        $"远端最新发布（{release.TagName}）为预发布版本，正式版用户不自动更新。");
                }

                if (release.Prerelease)
                {
                    return UpdateCheckResult.NoUpdate(currentVersionText, "远端最新发布为预发布版本，已跳过。");
                }

                if (!ProgramVersion.TryParseTag(release.TagName, out Version latestVersion))
                {
                    return UpdateCheckResult.Failure(
                        currentVersionText,
                        $"无法解析远端版本号：{release.TagName}");
                }

                string latestVersionText = latestVersion.ToString(3);
                bool hasUpdate = latestVersion > ProgramVersion.Current;
                UpdateAssetInfo? packageAsset = _releaseClient.SelectPackageAsset(release);
                if (hasUpdate && packageAsset == null)
                {
                    return UpdateCheckResult.Failure(
                        currentVersionText,
                        $"发现新版本 {latestVersionText}，但发布中未找到可用的程序更新包。");
                }

                UpdateCheckResult result = new UpdateCheckResult
                {
                    CurrentVersion = currentVersionText,
                    LatestVersion = latestVersionText,
                    LatestTag = release.TagName,
                    HasUpdate = hasUpdate,
                    IsSucceeded = true,
                    SkipReason = hasUpdate ? string.Empty : $"当前已是最新版本（{currentVersionText}）。",
                    ReleaseNotes = release.Body ?? string.Empty,
                    ReleasePageUrl = release.HtmlUrl,
                    PublishedAt = release.PublishedAt.ToLocalTime(),
                    PackageAsset = packageAsset
                };

                LogTool.Log($"[ProgramUpdateService] 版本检查完成：当前 {currentVersionText}，远端 {latestVersionText}，可更新={hasUpdate}");
                return result;
            }
            catch (OperationCanceledException)
            {
                return UpdateCheckResult.Failure(currentVersionText, "更新检查已取消。");
            }
            catch (Exception ex)
            {
                LogTool.Log($"[ProgramUpdateService] 检查更新异常：{ex.Message}");
                Debug.WriteLine($"[ProgramUpdateService] 检查更新异常：{ex.Message}");
                return UpdateCheckResult.Failure(currentVersionText, $"检查更新失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 下载更新包、解压并生成差异清单，最后写出待应用计划。
        /// </summary>
        public async Task<UpdateApplyPlan> DownloadAndPrepareAsync(
            UpdateCheckResult checkResult,
            IProgress<Tuple<int, string>>? progress = null,
            CancellationToken cancellationToken = default)
        {
            if (checkResult.PackageAsset == null)
            {
                throw new InvalidOperationException("检查结果中没有可用的更新包。");
            }

            string versionDirectory = Path.Combine(
                _updateRootDirectory,
                UpdateConstants.PackagesDirectoryName,
                checkResult.LatestVersion);

            Directory.CreateDirectory(versionDirectory);

            string packagePath = Path.Combine(
                versionDirectory,
                checkResult.PackageAsset.Name);

            EnsureFreeSpace(checkResult.PackageAsset.Size);

            Report(progress, 0, $"正在下载更新包 {checkResult.PackageAsset.Name}...");
            await DownloadPackageAsync(checkResult, packagePath, progress, cancellationToken).ConfigureAwait(false);

            Report(progress, DownloadProgressCeiling, "正在校验更新包...");
            VerifyPackage(packagePath, checkResult.PackageAsset);

            cancellationToken.ThrowIfCancellationRequested();

            string stagingDirectory = Path.Combine(versionDirectory, UpdatePackageExtractor.StagingDirectoryName);
            Report(progress, PrepareProgressStart, "正在解压更新包...");
            await _packageExtractor
                .ExtractAsync(packagePath, stagingDirectory, cancellationToken)
                .ConfigureAwait(false);

            string packageRoot = _packageExtractor.ResolvePackageRoot(stagingDirectory);

            if (UpdatePackageExtractor.HasEscapedEntries(stagingDirectory))
            {
                throw new InvalidDataException("更新包解压结果越界，已中止更新。");
            }

            Report(progress, PrepareProgressStart + 5, "正在比对本地文件...");

            // 先抽取用户态文件的默认值快照：这些文件不会被覆盖，但新版本可能给它们增加了字段，
            // 快照会由下次启动时的配置增量合并服务使用。
            // 注意：快照必须放在下载目录之外，否则更新成功后的清理会连同快照一起删除。
            string snapshotDirectory = Path.Combine(
                _updateRootDirectory,
                UpdateConstants.DefaultsDirectoryName,
                $"{checkResult.LatestVersion}{UpdateConstants.BackupSuffixPrefix}{DateTime.Now:yyyyMMddHHmmss}");

            int snapshotCount = await _packageExtractor
                .BuildUserStateSnapshotAsync(
                    packageRoot,
                    snapshotDirectory,
                    _pathPolicy.IsUserStatePath,
                    cancellationToken)
                .ConfigureAwait(false);

            if (snapshotCount > 0)
            {
                SaveSnapshotPointer(snapshotDirectory);
                LogTool.Log($"[ProgramUpdateService] 已保存 {snapshotCount} 个用户态文件的默认值快照：{snapshotDirectory}");
            }

            List<UpdateFileEntry> changeList = await _packageExtractor
                .BuildChangeListAsync(
                    packageRoot,
                    _installDirectory,
                    _pathPolicy.IsUserStatePath,
                    cancellationToken)
                .ConfigureAwait(false);

            // 二次校验：清单中的路径必须落在安装目录内部
            List<UpdateFileEntry> safeEntries = changeList
                .Where(entry => _pathPolicy.IsSafeRelativePath(entry.RelativePath))
                .ToList();

            if (safeEntries.Count != changeList.Count)
            {
                LogTool.Log(
                    $"[ProgramUpdateService] 已过滤 {changeList.Count - safeEntries.Count} 个不安全的更新路径。");
            }

            string backupDirectory = Path.Combine(
                _updateRootDirectory,
                UpdateConstants.BackupsDirectoryName,
                $"{checkResult.LatestVersion}{UpdateConstants.BackupSuffixPrefix}{DateTime.Now:yyyyMMddHHmmss}");

            UpdateApplyPlan plan = new UpdateApplyPlan
            {
                TargetVersion = checkResult.LatestVersion,
                StagingDirectory = packageRoot,
                InstallDirectory = _installDirectory,
                BackupDirectory = backupDirectory,
                ParentProcessId = Environment.ProcessId,
                CreatedAt = DateTimeOffset.Now,
                ExecutableRelativePath = UpdateConstants.ExecutableFileName,
                Files = safeEntries,
                CleanupDirectories = new List<string> { stagingDirectory, versionDirectory }
            };

            SavePlan(plan);
            Report(progress, 100, $"准备完成，共 {safeEntries.Count} 个文件需要更新。");

            LogTool.Log(
                $"[ProgramUpdateService] 更新包已准备：目标版本 {plan.TargetVersion}，" +
                $"待更新文件 {safeEntries.Count} 个，staging={plan.StagingDirectory}");

            return plan;
        }

        /// <summary>
        /// 启动独立的应用进程执行更新，并退出当前程序。
        /// </summary>
        public bool LaunchApplyProcess(UpdateApplyPlan plan)
        {
            try
            {
                string executablePath = Path.Combine(_installDirectory, UpdateConstants.ExecutableFileName);
                if (!File.Exists(executablePath))
                {
                    LogTool.Log($"[ProgramUpdateService] 未找到主程序，无法启动更新进程：{executablePath}");
                    return false;
                }

                string planPath = GetPlanFilePath();

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    Arguments = $"{UpdateConstants.ApplyUpdateArgument} \"{planPath}\"",
                    WorkingDirectory = _installDirectory,
                    UseShellExecute = true
                };

                using Process? process = Process.Start(startInfo);
                if (process == null)
                {
                    LogTool.Log("[ProgramUpdateService] 更新进程启动返回空对象。");
                    return false;
                }

                LogTool.Log($"[ProgramUpdateService] 已启动更新进程，PID={process.Id}，计划={planPath}");
                return true;
            }
            catch (Exception ex)
            {
                LogTool.Log($"[ProgramUpdateService] 启动更新进程失败：{ex.Message}");
                Debug.WriteLine($"[ProgramUpdateService] 启动更新进程失败：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 清理历史下载包与过期备份，只保留最近若干次备份。
        /// </summary>
        public void CleanupExpiredArtifacts(int keepBackupCount)
        {
            // 先清理上次更新遗留的 .old 映像：它们位于安装目录，与更新缓存目录是否存在无关
            CleanupStaleReplacedFiles();

            try
            {
                if (!Directory.Exists(_updateRootDirectory))
                {
                    return;
                }

                // 清理解压用的 staging：更新包已在应用阶段由 UpdateApplier 负责删除，
                // 这里处理的是上一次失败或中断留下的残留目录。
                string packagesDirectory = Path.Combine(_updateRootDirectory, UpdateConstants.PackagesDirectoryName);
                if (Directory.Exists(packagesDirectory))
                {
                    foreach (string versionDirectory in Directory.GetDirectories(packagesDirectory))
                    {
                        string stagingDirectory = Path.Combine(
                            versionDirectory,
                            UpdatePackageExtractor.StagingDirectoryName);

                        if (Directory.Exists(stagingDirectory))
                        {
                            TryDeleteDirectory(stagingDirectory);
                        }
                    }
                }

                string backupsDirectory = Path.Combine(_updateRootDirectory, UpdateConstants.BackupsDirectoryName);
                if (Directory.Exists(backupsDirectory))
                {
                    int effectiveKeepCount = keepBackupCount > 0 ? keepBackupCount : UpdateConstants.KeepBackupCount;

                    List<string> backups = Directory
                        .GetDirectories(backupsDirectory)
                        .OrderByDescending(Directory.GetCreationTimeUtc)
                        .ToList();

                    foreach (string backup in backups.Skip(effectiveKeepCount))
                    {
                        TryDeleteDirectory(backup);
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.Log($"[ProgramUpdateService] 清理更新缓存失败：{ex.Message}");
                Debug.WriteLine($"[ProgramUpdateService] 清理更新缓存失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 清理上次更新遗留的 .old 映像文件。
        /// 更新进程自身运行在被替换的 exe 上，当时无法删除自己的旧映像；程序重启后该文件才解锁，
        /// 因此这里先直接尝试，失败的文件再交给后台有限次重试（重启瞬间更新进程可能尚未退出）。
        /// </summary>
        private void CleanupStaleReplacedFiles()
        {
            List<string> pendingFiles = new List<string>();

            try
            {
                foreach (string file in Directory.GetFiles(_installDirectory, "*" + UpdateConstants.ReplacedFileSuffix))
                {
                    if (TryDeleteStaleFile(file))
                    {
                        LogTool.Log($"[ProgramUpdateService] 已清理更新残留文件：{Path.GetFileName(file)}");
                    }
                    else
                    {
                        pendingFiles.Add(file);
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.Log($"[ProgramUpdateService] 扫描更新残留文件失败：{ex.Message}");
                Debug.WriteLine($"[ProgramUpdateService] 扫描更新残留文件失败：{ex.Message}");
            }

            if (pendingFiles.Count > 0)
            {
                _ = Task.Run(() => RetryDeleteStaleFilesAsync(pendingFiles));
            }
        }

        /// <summary>
        /// 对暂时删除失败（仍被占用）的残留文件做有限次后台重试。
        /// </summary>
        /// <param name="files">待删除文件列表</param>
        private static async Task RetryDeleteStaleFilesAsync(List<string> files)
        {
            foreach (string file in files)
            {
                for (int attempt = 1; attempt <= StaleFileRetryCount; attempt++)
                {
                    await Task.Delay(StaleFileRetryDelayMs).ConfigureAwait(false);

                    if (TryDeleteStaleFile(file))
                    {
                        LogTool.Log($"[ProgramUpdateService] 已清理更新残留文件：{Path.GetFileName(file)}");
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 尝试删除残留文件，失败时返回 false（不抛异常）。
        /// </summary>
        private static bool TryDeleteStaleFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return true;
                }

                File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ProgramUpdateService] 删除残留文件失败（{filePath}）：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 读取并消费最近一次更新留下的结果记录。
        /// 更新进程在主程序退出后写入该记录，程序重启后在此读取并向用户提示；
        /// 无论成功与否，读取后都会删除记录文件，保证同一结果只提示一次。
        /// </summary>
        /// <returns>更新结果记录；不存在或无法解析时返回 null</returns>
        public UpdateResultRecord? ConsumeLatestUpdateResult()
        {
            try
            {
                string resultDirectory = Path.Combine(
                    _updateRootDirectory,
                    UpdateConstants.LogsDirectoryName);

                if (!Directory.Exists(resultDirectory))
                {
                    return null;
                }

                string[] resultFiles = Directory
                    .GetFiles(resultDirectory, UpdateConstants.UpdateResultFilePrefix + "*.json")
                    .OrderByDescending(File.GetCreationTimeUtc)
                    .ToArray();

                if (resultFiles.Length == 0)
                {
                    return null;
                }

                string latestFile = resultFiles[0];

                UpdateResultRecord? record;
                try
                {
                    record = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateResultRecord>(
                        File.ReadAllText(latestFile));
                }
                catch (Exception ex)
                {
                    LogTool.Log($"[ProgramUpdateService] 更新结果记录解析失败：{ex.Message}");
                    record = new UpdateResultRecord
                    {
                        IsSuccess = false,
                        Message = UpdateMessages.ResultFileUnreadable
                    };
                }

                if (record != null)
                {
                    record.FilePath = latestFile;
                }

                // 同一结果只提示一次：历史记录一并清理
                foreach (string file in resultFiles)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[ProgramUpdateService] 删除更新结果记录失败（{file}）：{ex.Message}");
                    }
                }

                return record;
            }
            catch (Exception ex)
            {
                LogTool.Log($"[ProgramUpdateService] 读取更新结果记录失败：{ex.Message}");
                Debug.WriteLine($"[ProgramUpdateService] 读取更新结果记录失败：{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 获取待应用计划文件路径。
        /// </summary>
        /// <returns>计划文件完整路径</returns>
        public string GetPlanFilePath()
        {
            return Path.Combine(_updateRootDirectory, UpdateConstants.PlanFileName);
        }

        /// <summary>
        /// 把计划写入磁盘（UTF-8、缩进、中文可读）。
        /// </summary>
        /// <param name="plan">待应用计划</param>
        public void SavePlan(UpdateApplyPlan plan)
        {
            Directory.CreateDirectory(_updateRootDirectory);

            string planPath = GetPlanFilePath();
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(
                plan,
                Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(planPath, json);
        }

        /// <summary>
        /// 记录用户态文件默认值快照的当前位置，并清理过期的旧快照。
        /// 快照目录名带时间戳，通过指针文件让下次启动时能找到最新一份。
        /// </summary>
        /// <param name="snapshotDirectory">本次生成的快照目录</param>
        private void SaveSnapshotPointer(string snapshotDirectory)
        {
            try
            {
                Directory.CreateDirectory(_updateRootDirectory);

                File.WriteAllText(
                    Path.Combine(_updateRootDirectory, UpdateConstants.SnapshotPointerFileName),
                    snapshotDirectory);

                // 保留最近若干份快照，避免长期累积占用磁盘
                string defaultsRoot = Path.Combine(_updateRootDirectory, UpdateConstants.DefaultsDirectoryName);
                if (!Directory.Exists(defaultsRoot))
                {
                    return;
                }

                List<string> snapshots = Directory
                    .GetDirectories(defaultsRoot)
                    .OrderByDescending(Directory.GetCreationTimeUtc)
                    .ToList();

                foreach (string expired in snapshots.Skip(UpdateConstants.KeepDefaultsCount))
                {
                    TryDeleteDirectory(expired);
                }
            }
            catch (Exception ex)
            {
                // 快照指针写入失败只会导致下次启动少合并一次默认值，不应中断更新
                LogTool.Log($"[ProgramUpdateService] 保存默认值快照指针失败：{ex.Message}");
                Debug.WriteLine($"[ProgramUpdateService] 保存默认值快照指针失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 读取最近一次更新留下的用户态文件默认值快照目录。
        /// </summary>
        /// <returns>快照目录；不存在时返回 null</returns>
        public string? GetLatestDefaultsSnapshotDirectory()
        {
            try
            {
                string pointerPath = Path.Combine(_updateRootDirectory, UpdateConstants.SnapshotPointerFileName);
                if (!File.Exists(pointerPath))
                {
                    return null;
                }

                string directory = File.ReadAllText(pointerPath).Trim();
                return Directory.Exists(directory) ? directory : null;
            }
            catch (Exception ex)
            {
                LogTool.Log($"[ProgramUpdateService] 读取默认值快照指针失败：{ex.Message}");
                Debug.WriteLine($"[ProgramUpdateService] 读取默认值快照指针失败：{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 下载更新包（多镜像回退、断点续传、进度回调）。
        /// </summary>
        private async Task DownloadPackageAsync(
            UpdateCheckResult checkResult,
            string packagePath,
            IProgress<Tuple<int, string>>? progress,
            CancellationToken cancellationToken)
        {
            List<string> urls = _releaseClient.BuildDownloadUrls(checkResult.PackageAsset!.BrowserDownloadUrl);
            if (urls.Count == 0)
            {
                throw new InvalidOperationException("更新包下载地址为空。");
            }

            DownloadService downloadService = new DownloadService();
            downloadService.ProgressChanged += (_, args) =>
            {
                // 下载阶段占用 0-80 的进度区间，避免与后续解压阶段重叠
                int percentage = (int)Math.Clamp(args.ProgressPercentage * DownloadProgressCeiling / 100.0, 0, DownloadProgressCeiling);
                string message = string.IsNullOrWhiteSpace(args.Message)
                    ? "正在下载更新包..."
                    : args.Message;

                Report(progress, percentage, message);
            };

            bool success = await downloadService
                .DownloadFileAsync(urls, packagePath, cancellationToken)
                .ConfigureAwait(false);

            if (!success)
            {
                throw new IOException("更新包下载失败，已尝试全部下载源。");
            }
        }

        /// <summary>
        /// 校验更新包大小与摘要（GitHub 提供 digest 时）。
        /// </summary>
        private void VerifyPackage(string packagePath, UpdateAssetInfo asset)
        {
            FileInfo fileInfo = new FileInfo(packagePath);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("更新包下载后未找到文件。", packagePath);
            }

            if (asset.Size > 0 && fileInfo.Length != asset.Size)
            {
                throw new InvalidDataException(
                    $"更新包大小不匹配：期望 {asset.Size} 字节，实际 {fileInfo.Length} 字节。");
            }

            if (!string.IsNullOrEmpty(asset.Sha256))
            {
                string actual = UpdatePackageExtractor.ComputeSha256(packagePath);
                if (!string.Equals(actual, asset.Sha256, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidDataException("更新包校验失败（SHA256 不匹配），已中止更新。");
                }

                LogTool.Log("[ProgramUpdateService] 更新包 SHA256 校验通过。");
            }
        }

        /// <summary>
        /// 检查可用磁盘空间，空间不足时提前中止，避免下载到一半失败。
        /// </summary>
        private void EnsureFreeSpace(long packageSize)
        {
            try
            {
                string root = Path.GetPathRoot(_updateRootDirectory) ?? string.Empty;
                if (string.IsNullOrEmpty(root))
                {
                    return;
                }

                DriveInfo drive = new DriveInfo(root);
                long required = Math.Max(packageSize * 2, RequiredFreeSpaceBytes);

                if (drive.AvailableFreeSpace < required)
                {
                    throw new IOException(
                        $"磁盘剩余空间不足，更新需要约 {required / 1024 / 1024} MB，当前可用 " +
                        $"{drive.AvailableFreeSpace / 1024 / 1024} MB。");
                }
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // 空间检测本身失败不应阻断更新流程
                Debug.WriteLine($"[ProgramUpdateService] 磁盘空间检测失败：{ex.Message}");
                LogTool.Log($"[ProgramUpdateService] 磁盘空间检测失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 上报进度，progress 为 null 时忽略。
        /// </summary>
        private static void Report(IProgress<Tuple<int, string>>? progress, int percentage, string message)
        {
            progress?.Report(Tuple.Create(percentage, message));
        }

        /// <summary>
        /// 尽力删除目录，失败只记录日志（可能被其他进程占用）。
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
                LogTool.Log($"[ProgramUpdateService] 删除目录失败（{directory}）：{ex.Message}");
                Debug.WriteLine($"[ProgramUpdateService] 删除目录失败（{directory}）：{ex.Message}");
            }
        }
    }
}

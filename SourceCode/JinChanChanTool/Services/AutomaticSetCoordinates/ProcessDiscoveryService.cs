using System.Diagnostics;

namespace JinChanChanTool.Services.AutoSetCoordinates
{
    /// <summary>
    /// 负责发现当前系统中拥有可见窗口的进程。
    /// </summary>
    public class ProcessDiscoveryService
    {
        private static readonly string[] TftGameProcessNames =
        {
            "TFTTencentClient-Win64-Shipping",
            "League of Legends"
        };

        private static readonly string[] MumuGameProcessNames =
        {
            "MuMuNxDevice"
        };

        private static readonly string[] LdGameProcessNames =
        {
            "dnplayer"
        };

        /// <summary>
        /// 获取当前系统中所有拥有可见主窗口的进程列表。
        /// </summary>
        /// <returns>一个进程快照列表，按进程名排序。</returns>
        public List<ProcessSnapshot> GetPotentiallyVisibleProcesses()
        {
            return GetPotentiallyVisibleProcesses(includeExecutablePath: true);
        }

        private static List<ProcessSnapshot> GetPotentiallyVisibleProcesses(bool includeExecutablePath)
        {
            var snapshots = new List<ProcessSnapshot>();
            foreach (Process process in Process.GetProcesses())
            {
                using (process)
                {
                    if (TryCreateSnapshot(process, includeExecutablePath, out ProcessSnapshot? snapshot)
                        && snapshot != null
                        && snapshot.MainWindowHandle != nint.Zero
                        && !string.IsNullOrEmpty(snapshot.MainWindowTitle))
                    {
                        snapshots.Add(snapshot);
                    }
                }
            }

            return snapshots
                .OrderBy(p => p.ProcessName)
                .ToList();
        }

        /// <summary>
        /// 获取可自动识别的游戏窗口进程。
        /// </summary>
        public List<ProcessSnapshot> GetAutoDetectableGameProcesses()
        {
            return GetPotentiallyVisibleProcesses(includeExecutablePath: false)
                .Where(IsSupportedAutoDetectProcess)
                .OrderBy(GetAutoDetectPriority)
                .ThenBy(p => p.ProcessName)
                .ThenBy(p => p.Id)
                .ToList();
        }

        /// <summary>
        /// 自动识别单个目标进程。若同一类目标存在多个实例，返回 false 并通过 ambiguousProcessName 告知冲突类型。
        /// </summary>
        public bool TryGetAutoDetectedProcess(out ProcessSnapshot? targetProcess, out string ambiguousProcessName)
        {
            targetProcess = null;
            ambiguousProcessName = string.Empty;

            List<ProcessSnapshot> candidates = GetAutoDetectableGameProcesses();
            List<ProcessSnapshot> tftProcesses = candidates.Where(IsTftGameProcess).ToList();
            if (tftProcesses.Count == 1)
            {
                targetProcess = tftProcesses[0];
                return true;
            }

            if (tftProcesses.Count > 1)
            {
                ambiguousProcessName = TftGameProcessNames[0];
                return false;
            }

            List<ProcessSnapshot> mumuProcesses = candidates.Where(IsMumuProcess).ToList();
            if (mumuProcesses.Count == 1)
            {
                targetProcess = mumuProcesses[0];
                return true;
            }

            if (mumuProcesses.Count > 1)
            {
                ambiguousProcessName = MumuGameProcessNames[0];
                return false;
            }

            List<ProcessSnapshot> ldProcesses = candidates.Where(IsLdProcess).ToList();
            if (ldProcesses.Count == 1)
            {
                targetProcess = ldProcesses[0];
                return true;
            }

            if (ldProcesses.Count > 1)
            {
                ambiguousProcessName = LdGameProcessNames[0];
                return false;
            }

            return true;
        }

        /// <summary>
        /// 根据 PID 获取一次进程快照，原始 Process 在方法返回前释放。
        /// </summary>
        public bool TryGetProcessById(int processId, out ProcessSnapshot? snapshot)
        {
            snapshot = null;
            try
            {
                using Process process = Process.GetProcessById(processId);
                return TryCreateSnapshot(process, includeExecutablePath: false, out snapshot);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 按名称获取进程快照，枚举产生的 Process 对象均在方法返回前释放。
        /// </summary>
        public List<ProcessSnapshot> GetProcessesByName(string processName)
        {
            var snapshots = new List<ProcessSnapshot>();
            foreach (Process process in Process.GetProcessesByName(processName))
            {
                using (process)
                {
                    if (TryCreateSnapshot(process, includeExecutablePath: false, out ProcessSnapshot? snapshot)
                        && snapshot != null)
                    {
                        snapshots.Add(snapshot);
                    }
                }
            }

            return snapshots;
        }

        /// <summary>
        /// 检查快照对应的进程是否仍存在且名称未发生变化。
        /// </summary>
        public bool IsProcessAlive(ProcessSnapshot snapshot)
        {
            if (!TryGetProcessById(snapshot.Id, out ProcessSnapshot? current))
            {
                return false;
            }

            // TryGetProcessById 返回 true 时快照必定非空，此处显式判空以满足可空性检查，空值与“进程已退出”同样返回 false
            if (current == null)
            {
                LogTool.Log($"[ProcessDiscoveryService] IsProcessAlive 进程快照查询结果为空，视为进程已退出：{snapshot.Id}");
                Debug.WriteLine($"[ProcessDiscoveryService] IsProcessAlive 进程快照查询结果为空，视为进程已退出：{snapshot.Id}");
                return false;
            }

            return current.ProcessName.Equals(snapshot.ProcessName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryCreateSnapshot(
            Process process,
            bool includeExecutablePath,
            out ProcessSnapshot? snapshot)
        {
            snapshot = null;
            try
            {
                string? executablePath = null;
                if (includeExecutablePath)
                {
                    try
                    {
                        executablePath = process.MainModule?.FileName;
                    }
                    catch
                    {
                        // 部分系统进程不允许读取主模块，窗口信息仍可正常使用。
                    }
                }

                snapshot = new ProcessSnapshot(
                    process.Id,
                    process.ProcessName,
                    process.MainWindowHandle,
                    process.MainWindowTitle,
                    executablePath);
                return true;
            }
            catch
            {
                // 枚举期间进程可能已经退出，忽略该快照。
                return false;
            }
        }

        private static bool IsSupportedAutoDetectProcess(ProcessSnapshot process)
        {
            return IsTftGameProcess(process) || IsMumuProcess(process) || IsLdProcess(process);
        }

        private static bool IsTftGameProcess(ProcessSnapshot process)
        {
            return TftGameProcessNames.Any(name =>
                process.ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsMumuProcess(ProcessSnapshot process)
        {
            return MumuGameProcessNames.Any(name =>
                process.ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsLdProcess(ProcessSnapshot process)
        {
            return LdGameProcessNames.Any(name =>
                process.ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private static int GetAutoDetectPriority(ProcessSnapshot process)
        {
            if (IsTftGameProcess(process)) return 0;
            if (IsMumuProcess(process)) return 1;
            if (IsLdProcess(process)) return 2;
            return 3;
        }
    }
}

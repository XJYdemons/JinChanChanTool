using System.Diagnostics;

namespace JinChanChanTool.Services.AutoSetCoordinates
{
    /// <summary>
    /// 负责发现当前系统中拥有可见窗口的进程。
    /// </summary>
    public class ProcessDiscoveryService
    {
        private static readonly string[] LeagueGameProcessNames =
        {
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
        /// <returns>一个 Process 列表，按进程名排序。</returns>
        public List<Process> GetPotentiallyVisibleProcesses()
        {
            return Process.GetProcesses()
                .Where(p => p.MainWindowHandle != nint.Zero && !string.IsNullOrEmpty(p.MainWindowTitle))
                .OrderBy(p => p.ProcessName)
                .ToList();
        }

        /// <summary>
        /// 获取可自动识别的游戏窗口进程。
        /// </summary>
        public List<Process> GetAutoDetectableGameProcesses()
        {
            return GetPotentiallyVisibleProcesses()
                .Where(IsSupportedAutoDetectProcess)
                .OrderBy(GetAutoDetectPriority)
                .ThenBy(p => p.ProcessName)
                .ThenBy(p => p.Id)
                .ToList();
        }

        /// <summary>
        /// 自动识别单个目标进程。若同一类目标存在多个实例，返回 false 并通过 ambiguousProcessName 告知冲突类型。
        /// </summary>
        public bool TryGetAutoDetectedProcess(out Process? targetProcess, out string ambiguousProcessName)
        {
            targetProcess = null;
            ambiguousProcessName = string.Empty;

            List<Process> candidates = GetAutoDetectableGameProcesses();
            List<Process> leagueProcesses = candidates.Where(IsLeagueGameProcess).ToList();
            if (leagueProcesses.Count == 1)
            {
                targetProcess = leagueProcesses[0];
                return true;
            }

            if (leagueProcesses.Count > 1)
            {
                ambiguousProcessName = LeagueGameProcessNames[0];
                return false;
            }

            List<Process> mumuProcesses = candidates.Where(IsMumuProcess).ToList();
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

            List<Process> ldProcesses = candidates.Where(IsLdProcess).ToList();
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

        private static bool IsSupportedAutoDetectProcess(Process process)
        {
            return IsLeagueGameProcess(process) || IsMumuProcess(process) || IsLdProcess(process);
        }

        private static bool IsLeagueGameProcess(Process process)
        {
            return LeagueGameProcessNames.Any(name =>
                process.ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsMumuProcess(Process process)
        {
            return MumuGameProcessNames.Any(name =>
                process.ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsLdProcess(Process process)
        {
            return LdGameProcessNames.Any(name =>
                process.ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private static int GetAutoDetectPriority(Process process)
        {
            if (IsLeagueGameProcess(process)) return 0;
            if (IsMumuProcess(process)) return 1;
            if (IsLdProcess(process)) return 2;
            return 3;
        }
    }
}

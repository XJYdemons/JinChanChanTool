using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JinChanChanTool.Services
{
    /// <summary>
    /// 负责发现当前系统中拥有可见窗口的进程。
    /// </summary>
    public class ProcessDiscoveryService
    {
        /// <summary>
        /// 获取当前系统中所有拥有可见主窗口的进程列表。
        /// </summary>
        /// <returns>一个 Process 列表，按进程名排序。</returns>
        public List<Process> GetPotentiallyVisibleProcesses()
        {
            return Process.GetProcesses()
                .Where(p => p.MainWindowHandle != System.IntPtr.Zero && !string.IsNullOrEmpty(p.MainWindowTitle))
                .OrderBy(p => p.ProcessName)
                .ToList();
        }
    }
}
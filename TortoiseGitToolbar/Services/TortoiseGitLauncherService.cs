using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using Process = System.Diagnostics.Process;

namespace MattDavies.TortoiseGitToolbar.Services
{
    public interface ITortoiseGitLauncherService
    {
        void ExecuteTortoiseProc(ToolbarCommand command);
    }

    public class TortoiseGitLauncherService : ITortoiseGitLauncherService
    {
        private readonly IProcessManagerService _processManagerService;
        private readonly Solution2 _solution;

        public TortoiseGitLauncherService(IProcessManagerService processManagerService, Solution2 solution)
        {
            _processManagerService = processManagerService;
            _solution = solution;
        }

        public void ExecuteTortoiseProc(ToolbarCommand command)
        {
            var solutionPath = PathConfiguration.GetSolutionPath(_solution);
            if (solutionPath == null)
            {
                MessageBox.Show(
                    Resources.Resources.TortoiseGitLauncherService_SolutionPath_You_need_to_open_a_solution_first,
                    Resources.Resources.TortoiseGitLauncherService_SolutionPath_No_solution_found,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return;
            }

            var process = command == ToolbarCommand.Bash
                ? _processManagerService.GetProcess(PathConfiguration.GetGitBashPath(), "--login -i", solutionPath)
                : _processManagerService.GetProcess(PathConfiguration.GetTortoiseGitPath(), string.Format(@"/command:{0} /path:""{1}""", command.ToString().ToLower(), solutionPath));

            if (process != null)
                Process.Start(process);
        }
    }
}

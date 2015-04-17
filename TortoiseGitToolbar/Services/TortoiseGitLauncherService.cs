using System.Diagnostics;
using System.Windows.Forms;
using EnvDTE;
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
        private readonly Solution _solution;

        public TortoiseGitLauncherService(IProcessManagerService processManagerService, Solution solution)
        {
            _processManagerService = processManagerService;
            _solution = solution;
        }

        public void ExecuteTortoiseProc(ToolbarCommand command)
        {
            var solutionPath = PathConfiguration.GetSolutionPath(_solution);
            var openedFilePath = PathConfiguration.GetOpenedFilePath(_solution);
            // todo: make the bash/tortoise paths configurable
            // todo: detect if the solution is a git solution first
            if (command == ToolbarCommand.Bash && PathConfiguration.GetGitBashPath() == null)
            {
                MessageBox.Show(
                    Resources.Resources.TortoiseGitLauncherService_ExecuteTortoiseProc_Could_not_find_Git_Bash_in_the_standard_install_path_,
                    Resources.Resources.TortoiseGitLauncherService_ExecuteTortoiseProc_Git_Bash_not_found,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return;
            }
            if (command != ToolbarCommand.Bash && solutionPath == null)
            {
                MessageBox.Show(
                    Resources.Resources.TortoiseGitLauncherService_SolutionPath_You_need_to_open_a_solution_first,
                    Resources.Resources.TortoiseGitLauncherService_SolutionPath_No_solution_found,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return;
            }
            if (command != ToolbarCommand.Bash && PathConfiguration.GetTortoiseGitPath() == null)
            {
                MessageBox.Show(
                    Resources.Resources.TortoiseGitLauncherService_ExecuteTortoiseProc_Could_not_find_TortoiseGit_in_the_standard_install_path_,
                    Resources.Resources.TortoiseGitLauncherService_ExecuteTortoiseProc_TortoiseGit_not_found,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return;
            }

            ProcessStartInfo process;
            switch (command)
            {
                case ToolbarCommand.Bash:
                    process = _processManagerService.GetProcess(
                        PathConfiguration.GetGitBashPath(),
                        "--login -i",
                        solutionPath
                    );
                    break;
                case ToolbarCommand.RebaseContinue:
                    process = _processManagerService.GetProcess(
                        PathConfiguration.GetGitBashPath(),
                        @"--login -i -c 'echo; echo ""Running git rebase --continue""; echo; git rebase --continue; echo; echo ""Please review the output above and press enter to continue.""; read'",
                        solutionPath
                    );
                    break;
                case ToolbarCommand.FileLog:
                case ToolbarCommand.FileDiff:
                    var commandParam = command.ToString().Replace("File", string.Empty).ToLower();
                    process = _processManagerService.GetProcess(
                        PathConfiguration.GetTortoiseGitPath(),
                        string.Format(@"/command:{0} /path:""{1}""", commandParam, openedFilePath)
                    );
                    break;
                case ToolbarCommand.FileBlame:
                    var line = GetCurrentLine(_solution);
                    process = _processManagerService.GetProcess(
                        PathConfiguration.GetTortoiseGitPath(),
                        string.Format(@"/command:blame /path:""{0}"" /line:{1}", openedFilePath, line)
                    );
                    break;
                default:
                    process = _processManagerService.GetProcess(
                        PathConfiguration.GetTortoiseGitPath(),
                        string.Format(@"/command:{0} /path:""{1}""", command.ToString().ToLower(), solutionPath)
                    );
                    break;
            }

            if (process != null)
                Process.Start(process);
        }

        private int GetCurrentLine(Solution solution)
        {
            if (solution.DTE != null && solution.DTE.ActiveDocument != null
                && solution.DTE.ActiveDocument.Selection != null)
            {
                dynamic selection = solution.DTE.ActiveDocument.Selection;
                return selection.CurrentLine;
            }
            return 0;
        }
    }
}

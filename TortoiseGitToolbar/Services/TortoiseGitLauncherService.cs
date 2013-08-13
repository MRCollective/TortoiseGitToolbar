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
        private readonly Solution2 _solution;
        private readonly string _tortoiseGitPath;
        private readonly string _gitBashPath;

        //todo: can this service be dependency injected and unit tested?
        public TortoiseGitLauncherService(Solution2 solution)
        {
            _solution = solution;

            if (File.Exists(TortoiseGitConstants.TortoiseGitx64))
                _tortoiseGitPath = TortoiseGitConstants.TortoiseGitx64;
            else if (File.Exists(TortoiseGitConstants.TortoiseGitx86))
                _tortoiseGitPath = TortoiseGitConstants.TortoiseGitx86;
            
            _gitBashPath = TortoiseGitConstants.GitBash;
        }
        
        public void ExecuteTortoiseProc(ToolbarCommand command)
        {
            var solutionPath = GetSolutionPath();

            //Todo: hide the buttons if not in an active solution (+ potentially in a git solution if we can detect that)
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

            if (command == ToolbarCommand.Bash)
            {
                LaunchProcess(_gitBashPath, "--login -i", false, false);
            }
            else
            {
                LaunchProcess(_tortoiseGitPath, string.Format(@"/command:{0} /path:""{1}""", command.ToString().ToLower(), solutionPath));
            }
        }

        public virtual void LaunchProcess(string fileName, string arguments, bool waitForInputIdle = true, bool useShellExecute = true)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = useShellExecute,
                WorkingDirectory = GetSolutionPath()
            };

            var p = Process.Start(startInfo);
            
            if (waitForInputIdle)
                p.WaitForInputIdle();
            MoveWindow(p.MainWindowHandle, 0, 0, 0, 0, false);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);

        private string GetSolutionPath()
        {
            return _solution != null && _solution.IsOpen
                ? Path.GetDirectoryName(_solution.FullName)
                : null;
        }
    }
}

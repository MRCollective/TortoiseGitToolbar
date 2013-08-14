using System.IO;
using EnvDTE80;

namespace MattDavies.TortoiseGitToolbar.Config.Constants
{
    public static class PathConfiguration
    {
        private const string TortoiseGitx64 = @"C:\Program Files\TortoiseGit\bin\TortoiseGitProc.exe";
        private const string TortoiseGitx86 = @"C:\Program Files (x86)\TortoiseGit\bin\TortoiseGitProc.exe";
        private const string GitBash = @"C:\Program Files (x86)\Git\bin\sh.exe";

        public static string GetTortoiseGitPath()
        {
            if (File.Exists(TortoiseGitx64))
                return TortoiseGitx64;
            return File.Exists(TortoiseGitx86)
                ? TortoiseGitx86
                : null;
        }

        public static string GetGitBashPath()
        {
            return GitBash;
        }

        public static string GetSolutionPath(Solution2 solution)
        {
            return solution != null && solution.IsOpen
                ? Path.GetDirectoryName(solution.FullName)
                : null;
        }
    }
}

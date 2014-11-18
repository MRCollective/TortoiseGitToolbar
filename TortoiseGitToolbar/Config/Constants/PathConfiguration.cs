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

        public static string GetOpenedFilePath(Solution2 solution)
        {
            return solution != null && solution.DTE != null && solution.DTE.ActiveDocument != null
                // GetExactPathName is needed because visual studio can provide lowercase path, 
                // but git is case sensitive to file names
                ? GetExactPathName(solution.DTE.ActiveDocument.FullName)
                : null;
        }

        /// <summary>
        /// Get case sensitive path.
        /// http://stackoverflow.com/questions/325931/getting-actual-file-name-with-proper-casing-on-windows-with-net
        /// </summary>
        public static string GetExactPathName(string pathName)
        {
            if (!(File.Exists(pathName) || Directory.Exists(pathName)))
                return pathName;

            var di = new DirectoryInfo(pathName);

            if (di.Parent != null)
            {
                return Path.Combine(
                    GetExactPathName(di.Parent.FullName),
                    di.Parent.GetFileSystemInfos(di.Name)[0].Name);
            }
            return di.Name.ToUpper();
        }
    }
}

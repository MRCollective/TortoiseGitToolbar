using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE80;

namespace MattDavies.TortoiseGitToolbar.Config.Constants
{
    public static class PathConfiguration
    {
        private const string TortoiseGitx64 = @"Program Files\TortoiseGit\bin\TortoiseGitProc.exe";
        private const string TortoiseGitx86 = @"Program Files (x86)\TortoiseGit\bin\TortoiseGitProc.exe";
        private const string GitBash = @"Program Files (x86)\Git\bin\sh.exe";
        private const string GitBashNoX86 = @"Program Files\Git\bin\sh.exe";

        public static readonly string ConfigFile;

        private static string _tortoiseGitPath;
        private static string _gitBashPath;

        static PathConfiguration()
        {
            // init path of the config file
            // e.g. C:\Users\username\Documents\Visual Studio 2013\Extensions\TortoiseGitToolbar\config.ini
            var myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ConfigFile = Path.Combine(myDocumentsPath, @"Visual Studio 2013\Extensions\TortoiseGitToolbar\config.ini");
        }

        public static string GetTortoiseGitPath()
        {
            _tortoiseGitPath = _tortoiseGitPath
                ?? ReadConfig("path", "TortoiseGit")
                ?? GetPathTraverseDisks(TortoiseGitx64)
                ?? GetPathTraverseDisks(TortoiseGitx86);

            return _tortoiseGitPath;
        }

        public static string GetGitBashPath()
        {
            _gitBashPath = _gitBashPath
                ?? ReadConfig("path", "GitBash")
                ?? GetPathTraverseDisks(GitBash)
                ?? GetPathTraverseDisks(GitBashNoX86);

            return _gitBashPath;
        }

        public static string GetSolutionPath(Solution2 solution)
        {
            return solution != null && solution.IsOpen
                ? Path.GetDirectoryName(solution.FullName)
                : null;
        }

        private static string GetPathTraverseDisks(string pathWithoutDisk)
        {
            var disks = new[] { "C:\\", "D:\\", "E:\\", "F:\\" };

            foreach (var disk in disks)
            {
                var path = disk + pathWithoutDisk;
                if (File.Exists(path))
                    return path;
            }

            return null;
        }

        private static string ReadConfig(string section, string key)
        {
            if (!File.Exists(ConfigFile))
                return null;

            const int size = 1024;
            var sb = new StringBuilder(size, size);
            NativeMethods.GetPrivateProfileString(section, key, string.Empty, sb, size, ConfigFile);
            return sb.ToString();
        }

        private static class NativeMethods
        {
            [DllImport("kernel32", CharSet = CharSet.Unicode)]
            internal static extern int GetPrivateProfileString(
                string section, string key, string def, StringBuilder retVal, int bufferSize, string fileName);
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using EnvDTE80;
using Microsoft.Win32;

namespace MattDavies.TortoiseGitToolbar.Config.Constants
{
    public static class PathConfiguration
    {
        private const string TortoiseGitx64 = @"C:\Program Files\TortoiseGit\bin\TortoiseGitProc.exe";
        private const string TortoiseGitx86 = @"C:\Program Files (x86)\TortoiseGit\bin\TortoiseGitProc.exe";
        private const string GitBashx86 = @"C:\Program Files (x86)\Git\bin\sh.exe";
        private const string GitBashx64 = @"C:\Program Files\Git\bin\sh.exe";

        private static readonly RegistryKey TortoiseGitProcRegistryRoot = Registry.LocalMachine;
        private const string TortoiseGitProcRegistryPath = @"SOFTWARE\TortoiseGit";
        private const string TortoiseGitProcRegistryKeyName = "ProcPath";

        private static readonly RegistryKey GitBashRegistryRoot = Registry.CurrentUser;
        private const string GitBashRegistryPath = @"SOFTWARE\TortoiseGit";
        private const string GitBashRegistryKeyName = "MSysGit";

        public static string GetTortoiseGitPath()
        {
            var path = GetTortoiseGitPathFromRegistry();
            if (path != null)
                return path;
            if (File.Exists(TortoiseGitx64))
                return TortoiseGitx64;
            return File.Exists(TortoiseGitx86)
                ? TortoiseGitx86
                : null;
        }

        public static string GetGitBashPath()
        {
            var path = GetGitBashPathFromRegistry();
            if (path != null)
                return path;
            return File.Exists(GitBashx64) ? GitBashx64
                 : File.Exists(GitBashx86) ? GitBashx86
                 : null;
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

        public static string GetTortoiseGitPathFromRegistry()
        {
            var path = GetValeFromRegistry(TortoiseGitProcRegistryRoot, TortoiseGitProcRegistryPath, TortoiseGitProcRegistryKeyName);
            Debug.WriteLine("TortoiseGit path from registry: " + (path ?? "(null)"));

            if (path != null && File.Exists(path))
            {
                Debug.WriteLine("TortoiseGit path from registry exists.");
                return path;
            }

            Debug.WriteLine("TortoiseGit path from registry does not exist.");
            return null;
        }

        public static string GetGitBashPathFromRegistry()
        {
            var path = GetValeFromRegistry(GitBashRegistryRoot, GitBashRegistryPath, GitBashRegistryKeyName);
            Debug.WriteLine("Git bash path from registry: " + (path ?? "(null)"));

            if (path != null && Directory.Exists(path))
            {
                Debug.WriteLine("Git bash path from registry exists.");

                var shPath = Path.Combine(path, "sh.exe");
                if (File.Exists(shPath))
                {
                    Debug.WriteLine("Git bash path comined exists: " + shPath);
                    return shPath;
                }
            }

            return null;
        }

        private static string GetValeFromRegistry(RegistryKey root, string registryPath, string registryKeyName)
        {
            try
            {
                using (var key = root.OpenSubKey(registryPath))
                {
                    return (string)key.GetValue(registryKeyName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while retrieving registry path: " + root.ToString() + "\\" + registryPath + "\\" + registryKeyName);
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}

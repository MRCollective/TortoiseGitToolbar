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
            if (solution != null && solution.IsOpen)
            {
                var solutionPathFromSln = Path.GetDirectoryName(solution.FullName);

                var solutionPathInfo = new DirectoryInfo(solutionPathFromSln);
                Debug.WriteLine("Solution path is: " + solutionPathInfo.FullName);

                // find parent folder that holds the .git folder
                while (!Directory.Exists(Path.Combine(solutionPathInfo.FullName, ".git")))
                {
                    Debug.WriteLine("No .git folder found in solution path.");
                    if (solutionPathInfo.Parent == null)
                    {
                        Debug.WriteLine("No parent folder found. Using original path: " + solutionPathFromSln);
                        return solutionPathFromSln;
                    }

                    solutionPathInfo = solutionPathInfo.Parent;
                }

                Debug.WriteLine("Using solution path: " + solutionPathInfo.FullName);
                return solutionPathInfo.FullName;
            }

            return null;
        }

        public static string GetOpenedFilePath(Solution2 solution)
        {
            if (solution != null && solution.DTE != null)
            {
                try
                {
                    if (solution.DTE.ActiveDocument == null) // no active window
                        return null;
                }
                catch (ArgumentException)
                {
                    // active window is no file (e.g. the project properties)
                    return null;
                }

                // GetExactPathName is needed because visual studio can provide lowercase path,
                // but git is case sensitive to file names
                return GetExactPathName(solution.DTE.ActiveDocument.FullName);
            }

            return null;
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
            var path = GetValueFromRegistry(TortoiseGitProcRegistryRoot, TortoiseGitProcRegistryPath, TortoiseGitProcRegistryKeyName);
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
            var path = GetValueFromRegistry(GitBashRegistryRoot, GitBashRegistryPath, GitBashRegistryKeyName);
            Debug.WriteLine("Git bash path from registry: " + (path ?? "(null)"));

            if (path != null && Directory.Exists(path))
            {
                Debug.WriteLine("Git bash path from registry exists.");

                var shPath = Path.Combine(path, "sh.exe");
                if (File.Exists(shPath))
                {
                    Debug.WriteLine("Git bash path sh.exe exists: " + shPath);
                    return shPath;
                }
            }

            return null;
        }

        private static string GetValueFromRegistry(RegistryKey root, string registryPath, string registryKeyName)
        {
            try
            {
                using (var key = root.OpenSubKey(registryPath))
                {
                    if (key == null)
                    {
                        Debug.WriteLine("Registry path not found: " + root + "\\" + registryPath + "\\" + registryKeyName);
                        return null;
                    }

                    return (string)key.GetValue(registryKeyName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while retrieving registry path: " + root + "\\" + registryPath + "\\" + registryKeyName);
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace MattDavies.TortoiseGitToolbar.Config.Constants
{
    public static class PathConfiguration
    {
        private const string TortoiseGitx64 = @"C:\Program Files\TortoiseGit\bin\TortoiseGitProc.exe";
        private const string TortoiseGitx86 = @"C:\Program Files (x86)\TortoiseGit\bin\TortoiseGitProc.exe";
        private const string GitBashx86 = @"C:\Program Files (x86)\Git\bin\sh.exe";
        private const string GitBashx64 = @"C:\Program Files\Git\bin\sh.exe";
        private const string GitExex86 = @"C:\Program Files (x86)\Git\bin\git.exe";
        private const string GitExex64 = @"C:\Program Files\Git\bin\git.exe";


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

        public static string GetGitExePath()
        {
            var path = GetGitExePathFromRegistry();
            if (path != null)
                return path;
            return File.Exists(GitExex64) ? GitExex64
                : File.Exists(GitExex86) ? GitExex86
                : null;
        }

        public static string GetSolutionPath(Solution2 solution)
        {
            if (solution != null && solution.IsOpen)
            {
                var solutionPathFromSln = Path.GetDirectoryName(solution.FullName);
                Debug.WriteLine("Solution path is: " + solutionPathFromSln);

                var repositoryRootPath = GetRepositoryRootGit(solutionPathFromSln);
                if (repositoryRootPath == null)
                {
                    Debug.WriteLine("Failed to get root path from git. Trying to filesystem-based approach");
                    repositoryRootPath = GetRepositoryRootFs(solutionPathFromSln);
                    if (repositoryRootPath == null)
                    {
                        Debug.WriteLine("No parent folder found. Using original path: " + solutionPathFromSln);
                        return solutionPathFromSln;
                    }
                }

                Debug.WriteLine("Using solution path: " + repositoryRootPath);
                return repositoryRootPath;
            }

            return null;
        }

        /// <summary>
        /// Find repository root by calling "git rev-parse --show-toplevel" command
        /// </summary>
        /// <param name="solutionPath">Path inside repository (working path for git command)</param>
        /// <returns> Path to repository root, if found. Otherwise null.</returns>
        private static string GetRepositoryRootGit(string solutionPath)
        {
            var gitPath = GetGitExePath();
            if (gitPath == null)
            {
                return null;
            }

            var procInfo = new ProcessStartInfo
            {
                FileName = gitPath,
                Arguments = "rev-parse --show-toplevel",
                UseShellExecute = false,
                WorkingDirectory = solutionPath,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };

            using (var process = Process.Start(procInfo))
            {
                var stdOut = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                var errCode = process.ExitCode;

                Debug.WriteLine($"git rev-parse --show-toplevel exited with code {errCode} and stdout: {stdOut}");
                if (errCode != 0 || string.IsNullOrWhiteSpace(stdOut))
                {
                    return null;
                }

                try
                {
                    return Path.GetFullPath(stdOut);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"GetFullPath failed for {stdOut}, with {e} reason: {e.Message}");
                    return null;
                }
            }
        }


        /// <summary>
        /// Find repository root basing on filesystem by finding parent directory that contains .git folder
        /// </summary>
        /// <param name="solutionPath">Directory to start search from.</param>
        /// <returns> Path to repository root, if found. Otherwise null.</returns>
        private static string GetRepositoryRootFs(string solutionPath)
        {
            var solutionPathInfo = new DirectoryInfo(solutionPath);

            // find parent folder that holds the .git folder
            while (!Directory.Exists(Path.Combine(solutionPathInfo.FullName, ".git")))
            {
                Debug.WriteLine("No .git folder found in solution path.");
                if (solutionPathInfo.Parent == null)
                {
                    return null;
                }

                solutionPathInfo = solutionPathInfo.Parent;
            }

            return solutionPathInfo.FullName;
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
            return GetGitExecutablePathFromRegisty("sh.exe");
        }

        public static string GetGitExePathFromRegistry()
        {
            return GetGitExecutablePathFromRegisty("git.exe");
        }

        private static string GetGitExecutablePathFromRegisty(string executableName)
        {
            var path = GetValueFromRegistry(GitBashRegistryRoot, GitBashRegistryPath, GitBashRegistryKeyName);
            Debug.WriteLine("Git bash path from registry: " + (path ?? "(null)"));

            if (path != null && Directory.Exists(path))
            {
                Debug.WriteLine("Git bash path from registry exists.");

                var exePath = Path.Combine(path, executableName);
                if (File.Exists(exePath))
                {
                    Debug.WriteLine($"Git bash path {executableName} exists: " + exePath);
                    return exePath;
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

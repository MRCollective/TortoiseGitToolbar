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

        public static string GetSolutionFilePath(Solution2 solution)
        {
            if (solution != null)
            {
                return solution.FullName;
            }
            return null;
        }

        private static string FindGitRepositoryRootPathFromFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return null;

            // Prefer the authoritative answer from git itself
            var fromGit = GetRepositoryRootGit(filePath);
            if (!string.IsNullOrEmpty(fromGit))
                return fromGit;

            // Fallback: pure filesystem walk (handles worktrees where .git is a file)
            FileInfo fileInfo;
            try { fileInfo = new FileInfo(filePath); }
            catch { return null; }

            DirectoryInfo dirInfo = fileInfo.Directory;
            while (dirInfo != null && dirInfo.Exists)
            {
                var gitPath = Path.Combine(dirInfo.FullName, ".git");
                if (Directory.Exists(gitPath) || File.Exists(gitPath))
                {
                    return dirInfo.FullName;
                }
                dirInfo = dirInfo.Parent;
            }
            return null;
        }

        /// <summary>
        /// Best-effort repository root using "git rev-parse --show-toplevel".
        /// Working directory can be any path inside the repo (file or directory).
        /// </summary>
        private static string GetRepositoryRootGit(string anyPathInsideRepo)
        {
            var gitExe = GetGitExePath();
            if (gitExe == null || string.IsNullOrWhiteSpace(anyPathInsideRepo))
                return null;

            string workingDir;
            try
            {
                workingDir = File.Exists(anyPathInsideRepo)
                    ? Path.GetDirectoryName(anyPathInsideRepo)
                    : anyPathInsideRepo;
            }
            catch { return null; }

            if (string.IsNullOrEmpty(workingDir) || !Directory.Exists(workingDir))
                return null;

            var psi = new ProcessStartInfo
            {
                FileName = gitExe,
                Arguments = "rev-parse --show-toplevel",
                UseShellExecute = false,
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            try
            {
                using (var process = Process.Start(psi))
                {
                    var stdout = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit(3000);
                    if (process.ExitCode == 0 && !string.IsNullOrWhiteSpace(stdout))
                    {
                        return Path.GetFullPath(stdout);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("git rev-parse failed: " + ex.Message);
            }
            return null;
        }

        public static string GetRepositoryRootPath(Solution2 solution)
        {
            // 1. Prefer the currently active document (most accurate for multi-repo solutions)
            var selectedFile = GetOpenedFilePath(solution);
            if (!string.IsNullOrEmpty(selectedFile))
            {
                var root = FindGitRepositoryRootPathFromFilePath(selectedFile);
                if (!string.IsNullOrEmpty(root))
                {
                    return root;
                }
            }

            // 2. Fall back to the solution file / directory
            var solutionFile = GetSolutionFilePath(solution);
            if (!string.IsNullOrEmpty(solutionFile))
            {
                return FindGitRepositoryRootPathFromFilePath(solutionFile);
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

        public static string GetGitExePathFromRegistry()
        {
            return GetGitExecutablePathFromRegistry("git.exe");
        }

        public static string GetGitBashPathFromRegistry()
        {
            var path = GetValueFromRegistry(GitBashRegistryRoot, GitBashRegistryPath, GitBashRegistryKeyName);
            Debug.WriteLine("Git bash path from registry: " + (path ?? "(null)"));

            if (path != null && Directory.Exists(path))
            {
                return GetGitExecutablePathFromRegistry("sh.exe");
            }
            return null;
        }

        private static string GetGitExecutablePathFromRegistry(string executableName)
        {
            var path = GetValueFromRegistry(GitBashRegistryRoot, GitBashRegistryPath, GitBashRegistryKeyName);
            Debug.WriteLine("Git path from registry: " + (path ?? "(null)"));

            if (path != null && Directory.Exists(path))
            {
                var exePath = Path.Combine(path, executableName);
                if (File.Exists(exePath))
                {
                    Debug.WriteLine($"Git {executableName} exists: {exePath}");
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

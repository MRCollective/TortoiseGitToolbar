using System.Diagnostics;

namespace MattDavies.TortoiseGitToolbar.Services
{
    public interface IProcessManagerService
    {
        ProcessStartInfo GetProcess(string fileName, string arguments, string workingDirectory = null);
    }

    public class ProcessManagerService : IProcessManagerService
    {
        public ProcessStartInfo GetProcess(string fileName, string arguments, string workingDirectory = null)
        {
            return new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = string.IsNullOrEmpty(workingDirectory),
                WorkingDirectory = workingDirectory ?? string.Empty
            };
        } 
    }
}
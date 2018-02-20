using MattDavies.TortoiseGitToolbar.Services;
using Xunit;

namespace TortoiseGitToolbar.UnitTests.Services
{
    public class ProcessManagerServiceTests
    {
        private const string TestFileName = "testFileName";
        private const string TestArguments = "testArguments";
        private const string TestWorkingDirectory = "testWorkingDirectory";
        private readonly ProcessManagerService _processManagerService;
        
        public ProcessManagerServiceTests()
        {
            _processManagerService = new ProcessManagerService();
        }

        [Fact]
        public void Get_process_with_working_directory()
        {
            var process = _processManagerService.GetProcess(TestFileName, TestArguments, TestWorkingDirectory);

            Assert.Equal(TestFileName, process.FileName);
            Assert.Equal(TestArguments, process.Arguments);
            Assert.Equal(TestWorkingDirectory, process.WorkingDirectory);
            Assert.False(process.UseShellExecute);
        }

        [Fact]
        public void Get_process_without_working_directory()
        {
            var process = _processManagerService.GetProcess(TestFileName, TestArguments);

            Assert.Equal(TestFileName, process.FileName);
            Assert.Equal(TestArguments, process.Arguments);
            Assert.Equal(string.Empty, process.WorkingDirectory);
            Assert.True(process.UseShellExecute);
        }
    }
}

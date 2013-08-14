using MattDavies.TortoiseGitToolbar.Services;
using NUnit.Framework;

namespace TortoiseGitToolbar.UnitTests.Services
{
    [TestFixture]
    public class ProcessManagerServiceTests
    {
        private ProcessManagerService _processManagerService;

        [SetUp]
        public void Setup()
        {
            _processManagerService = new ProcessManagerService();
        }

        [Test]
        public void Get_process_with_working_directory()
        {
            const string fileName = "testFileName";
            const string arguments = "testArguments";
            const string workingDirectory = "testWorkingDirectory";

            var process = _processManagerService.GetProcess(fileName, arguments, workingDirectory);

            Assert.That(process.FileName, Is.EqualTo(fileName));
            Assert.That(process.Arguments, Is.EqualTo(arguments));
            Assert.That(process.WorkingDirectory, Is.EqualTo(workingDirectory));
            Assert.That(process.UseShellExecute, Is.False);
        }

        [Test]
        public void Get_process_without_working_directory()
        {
            const string fileName = "testFileName";
            const string arguments = "testArguments";

            var process = _processManagerService.GetProcess(fileName, arguments);

            Assert.That(process.FileName, Is.EqualTo(fileName));
            Assert.That(process.Arguments, Is.EqualTo(arguments));
            Assert.That(process.WorkingDirectory, Is.EqualTo(string.Empty));
            Assert.That(process.UseShellExecute, Is.True);
        }
    }
}

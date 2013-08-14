using MattDavies.TortoiseGitToolbar.Services;
using NUnit.Framework;

namespace TortoiseGitToolbar.UnitTests.Services
{
    [TestFixture]
    public class ProcessManagerServiceTests
    {
        private const string TestFileName = "testFileName";
        private const string TestArguments = "testArguments";
        private const string TestWorkingDirectory = "testWorkingDirectory";
        private ProcessManagerService _processManagerService;

        [SetUp]
        public void Setup()
        {
            _processManagerService = new ProcessManagerService();
        }

        [Test]
        public void Get_process_with_working_directory()
        {
            var process = _processManagerService.GetProcess(TestFileName, TestArguments, TestWorkingDirectory);

            Assert.That(process.FileName, Is.EqualTo(TestFileName));
            Assert.That(process.Arguments, Is.EqualTo(TestArguments));
            Assert.That(process.WorkingDirectory, Is.EqualTo(TestWorkingDirectory));
            Assert.That(process.UseShellExecute, Is.False);
        }

        [Test]
        public void Get_process_without_working_directory()
        {
            var process = _processManagerService.GetProcess(TestFileName, TestArguments);

            Assert.That(process.FileName, Is.EqualTo(TestFileName));
            Assert.That(process.Arguments, Is.EqualTo(TestArguments));
            Assert.That(process.WorkingDirectory, Is.EqualTo(string.Empty));
            Assert.That(process.UseShellExecute, Is.True);
        }
    }
}

using MattDavies.TortoiseGitToolbar.Config.Constants;
using NUnit.Framework;

namespace TortoiseGitToolbar.UnitTests.Services
{
    [TestFixture]
    public class PathConfigurationTests
    {
        [Test, Explicit("Works only if TortoiseGit installed.")]
        public void CanGetTortoiseProcPathFromRegistry()
        {
            var path = PathConfiguration.GetTortoiseGitPathFromRegistry();
            Assert.That(path, Is.Not.Null);
            Assert.That(path, Is.StringEnding("TortoiseGitProc.exe").IgnoreCase);
        }

        [Test, Explicit("Works only if TortoiseGit installed.")]
        public void CanGetGitBashPathFromRegistry()
        {
            var path = PathConfiguration.GetGitBashPathFromRegistry();
            Assert.That(path, Is.Not.Null);
            Assert.That(path, Is.StringEnding("sh.exe").IgnoreCase);
        }
    }
}

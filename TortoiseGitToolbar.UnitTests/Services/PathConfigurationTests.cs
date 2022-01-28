using System;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using Microsoft.VisualStudio.Sdk.TestFramework;
using Xunit;

namespace TortoiseGitToolbar.UnitTests.Services
{
    [Collection(MockedVS.Collection)]
    public class PathConfigurationTests
    {
        [Fact]
        public void CanGetTortoiseProcPathFromRegistry()
        {
            var path = PathConfiguration.GetTortoiseGitPathFromRegistry();
            Assert.NotNull(path);
            Assert.EndsWith("TortoiseGitProc.exe", path, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void CanGetGitBashPathFromRegistry()
        {
            var path = PathConfiguration.GetGitBashPathFromRegistry();
            Assert.NotNull(path);
            Assert.EndsWith("sh.exe", path, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

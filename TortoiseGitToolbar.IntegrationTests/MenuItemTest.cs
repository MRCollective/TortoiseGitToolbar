using System.ComponentModel.Design;
using System.Globalization;
using MattDavies.TortoiseGitToolbar;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace TortoiseGitToolbar.IntegrationTests
{
    [TestClass()]
    public class MenuItemTest
    {
        private delegate void ThreadInvoker();
        public TestContext TestContext { get; set; }

        [TestMethod]
        [HostType("VS IDE")]
        public void LaunchCommitWindowWithoutException()
        {
            UIThreadInvoker.Invoke((ThreadInvoker)delegate
            {
                var menuItemCmd = new CommandID(PackageConstants.guidTortoiseGitToolbarCmdSet, (int)ToolbarCommand.Commit);

                var testUtils = new TestUtils();
                testUtils.ExecuteCommand(menuItemCmd);
            });
        }

    }
}

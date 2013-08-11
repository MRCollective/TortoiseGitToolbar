using System.ComponentModel.Design;
using System.Globalization;
using MattDavies.TortoiseGitToolbar;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;

namespace TortoiseGitToolbar.IntegrationTests
{
    [TestClass()]
    public class MenuItemTest
    {
        private delegate void ThreadInvoker();
        public TestContext TestContext { get; set; }

        [TestMethod]
        [HostType("VS IDE")]
        public void LaunchCommand()
        {
            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                CommandID menuItemCmd = new CommandID(PackageConstants.guidTortoiseGitToolbarCmdSet, (int)CommandIdConstants.CmdCommit);

                // Create the DialogBoxListener Thread.
                string expectedDialogBoxText = string.Format(CultureInfo.CurrentCulture, "{0}\n\nInside {1}.MenuItemCallback()", "TortoiseGitToolbar", "MattDaviesmdaviesnet.TortoiseGitToolbar.TortoiseGitToolbarPackage");
                DialogBoxPurger purger = new DialogBoxPurger(NativeMethods.IDOK, expectedDialogBoxText);

                try
                {
                    purger.Start();

                    TestUtils testUtils = new TestUtils();
                    testUtils.ExecuteCommand(menuItemCmd);
                }
                finally
                {
                    Assert.IsTrue(purger.WaitForDialogThreadToTerminate(), "The dialog box has not shown");
                }
            });
        }

    }
}

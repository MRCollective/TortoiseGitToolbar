using System;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace TortoiseGitToolbar.IntegrationTests
{
    /// <summary>
    ///     Integration test for package validation
    /// </summary>
    [TestClass]
    public class PackageTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [HostType("VS IDE")]
        public void PackageLoadTest()
        {
            UIThreadInvoker.Invoke((ThreadInvoker) delegate
            {
                //Get the Shell Service
                var shellService = VsIdeTestHostContext.ServiceProvider.GetService(typeof (SVsShell)) as IVsShell;
                Assert.IsNotNull(shellService);

                //Validate package load
                IVsPackage package;
                var packageGuid = new Guid(PackageConstants.guidTortoiseGitToolbarPkgString);
                Assert.IsTrue(0 == shellService.LoadPackage(ref packageGuid, out package));
                Assert.IsNotNull(package, "Package failed to load");
            });
        }

        private delegate void ThreadInvoker();
    }
}
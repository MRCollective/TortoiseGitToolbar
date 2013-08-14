using MattDavies.TortoiseGitToolbar.Config.Constants;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace TortoiseGitToolbar.IntegrationTests
{
    [TestClass]
    public class PackageShould
    {
        [TestMethod]
        [HostType("VS IDE")]
        public void Load_shell_service()
        {
            var shellService = VsIdeTestHostContext.ServiceProvider.GetService(typeof(SVsShell)) as IVsShell;
            
            Assert.IsNotNull(shellService);
        }
        
        [TestMethod]
        [HostType("VS IDE")]
        public void Load_into_the_ide()
        {
            UIThreadInvoker.Invoke((ThreadInvoker) delegate
            {
                IVsPackage package;
                var shellService = (IVsShell) VsIdeTestHostContext.ServiceProvider.GetService(typeof (SVsShell));
                var packageGuid = PackageConstants.GuidTortoiseGitToolbarPkg;
                
                var packageLoaded = shellService.LoadPackage(ref packageGuid, out package);
                
                Assert.IsTrue(packageLoaded == 0);
                Assert.IsNotNull(package, "Package failed to load");
            });
        }

        private delegate void ThreadInvoker();
    }
}
using MattDavies.TortoiseGitToolbar;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.UnitTestLibrary;

namespace TortoiseGitToolbar.UnitTests
{
    [TestClass]
    public class PackageTest
    {
        [TestMethod]
        public void CreateInstance()
        {
            var package = new TortoiseGitToolbarPackage();
        }

        [TestMethod]
        public void IsIVsPackage()
        {
            var package = new TortoiseGitToolbarPackage();
            Assert.IsNotNull(package, "The object does not implement IVsPackage");
        }

        [TestMethod]
        public void SetSite()
        {
            var package = new TortoiseGitToolbarPackage() as IVsPackage;
            
            var serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
            
            Assert.IsNotNull(package, "The object does not implement IVsPackage");
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");
            Assert.AreEqual(0, package.SetSite(null), "SetSite(null) did not return S_OK");
        }
    }
}

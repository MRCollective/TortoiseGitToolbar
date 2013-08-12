using MattDavies.TortoiseGitToolbar;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.UnitTestLibrary;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TortoiseGitToolbar.UnitTests
{
    [TestFixture]
    public class PackageTest
    {
        [Test]
        public void CreateInstance()
        {
            var package = new TortoiseGitToolbarPackage();
        }

        [Test]
        public void IsIVsPackage()
        {
            var package = new TortoiseGitToolbarPackage();
            Assert.IsNotNull(package, "The object does not implement IVsPackage");
        }

        [Test]
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

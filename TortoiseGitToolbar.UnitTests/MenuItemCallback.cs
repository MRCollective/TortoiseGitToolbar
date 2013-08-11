using System.ComponentModel.Design;
using System.Reflection;
using MattDavies.TortoiseGitToolbar;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.UnitTestLibrary;
using TortoiseGitToolbar.UnitTests.Helpers;

namespace TortoiseGitToolbar.UnitTests
{
    [TestClass]
    public class MenuItemTest
    {
        [TestMethod]
        public void InitializeMenuCommand()
        {
            var package = new TortoiseGitToolbarPackage() as IVsPackage;
            var serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            var menuCommandID = new CommandID(PackageConstants.guidTortoiseGitToolbarCmdSet, (int)CommandIdConstants.CmdCommit);
            var info = typeof(Package).GetMethod("GetService", BindingFlags.Instance | BindingFlags.NonPublic);
            var mcs = info.Invoke(package, new object[] { (typeof(IMenuCommandService)) }) as OleMenuCommandService;

            Assert.IsNotNull(package, "The object does not implement IVsPackage");
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");
            Assert.IsNotNull(info);
            Assert.IsNotNull(mcs.FindCommand(menuCommandID));
        }

        [TestMethod]
        public void MenuItemCallback()
        {
            var package = new TortoiseGitToolbarPackage() as IVsPackage;
            var info = package.GetType().GetMethod("MenuItemCallback", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(info, "Failed to get the private method MenuItemCallback through reflection");
            var serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
            var uishellMock = UIShellServiceMock.GetUiShellInstance();
            
            serviceProvider.AddService(typeof(SVsUIShell), uishellMock, true);
            info.Invoke(package, new object[] { null, null });
            serviceProvider.RemoveService(typeof(SVsUIShell));

            Assert.IsNotNull(package, "The object does not implement IVsPackage");
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");
        }
    }
}

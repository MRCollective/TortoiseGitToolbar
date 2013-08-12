using System.ComponentModel.Design;
using System.Reflection;
using EnvDTE;
using MattDavies.TortoiseGitToolbar;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
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
            // Create the package
            IVsPackage package = new TortoiseGitToolbarPackage() as IVsPackage;
            Assert.IsNotNull(package, "The object does not implement IVsPackage");

            // Create a basic service provider
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Create a UIShell service mock and proffer the service so that it can called from the MenuItemCallback method
            BaseMock uishellMock = UIShellServiceMock.GetUiShellInstance();
            serviceProvider.AddService(typeof(SVsUIShell), uishellMock, true);

            // Site the package
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");

            //Invoke private method on package class and observe that the method does not throw
            System.Reflection.MethodInfo info = package.GetType().GetMethod("Commit", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(info, "Failed to get the private method MenuItemCallback throug refplection");
            info.Invoke(package, new object[] { null, null });

            //Clean up services
            serviceProvider.RemoveService(typeof(SVsUIShell));
        }
    }
}

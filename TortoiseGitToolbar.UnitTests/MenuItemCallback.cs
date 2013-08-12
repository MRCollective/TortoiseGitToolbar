using System.ComponentModel.Design;
using System.Reflection;
using MattDavies.TortoiseGitToolbar;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VsSDK.UnitTestLibrary;
using NUnit.Framework;
using TortoiseGitToolbar.UnitTests.Helpers;

namespace TortoiseGitToolbar.UnitTests
{
    [TestFixture]
    public class MenuItemTest
    {
        [Test]
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

        [Test]
        public void MenuItemCallback()
        {
            var package = new TortoiseGitToolbarPackage() as IVsPackage;
            Assert.IsNotNull(package, "The object does not implement IVsPackage");

            var serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            var uishellMock = UIShellServiceMock.GetUiShellInstance();
            serviceProvider.AddService(typeof(SVsUIShell), uishellMock, true);

            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");
            var info = package.GetType().GetMethod("Commit", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(info, "Failed to get the private method Commit");
            info.Invoke(package, new object[] { null, null });

            serviceProvider.RemoveService(typeof(SVsUIShell));
        }
    }
}

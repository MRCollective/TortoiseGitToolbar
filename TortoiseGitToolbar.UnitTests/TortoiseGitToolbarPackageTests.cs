using System;
using System.ComponentModel.Design;
using System.Reflection;
using FizzWare.NBuilder;
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
    public class TortoiseGitToolbarPackageShould
    {
        private IVsPackage _package;
        private MethodInfo _getServiceMethod;
        private OleServiceProvider _serviceProvider;
        private readonly ToolbarCommand[] _toolbarCommands = EnumHelper.GetValues<ToolbarCommand>();
        
        [SetUp]
        public void Setup()
        {
            _package = new TortoiseGitToolbarPackage();
            _serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
            _getServiceMethod = typeof(Package).GetMethod("GetService", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [Test]
        public void Implement_vspackage()
        {
            Assert.That(_package, Is.Not.Null, "The package does not implement IVsPackage");
        }

        [Test]
        public void Correctly_set_site()
        {
            Assert.That(_package.SetSite(_serviceProvider), Is.EqualTo(0), "Package SetSite did not return S_OK");
        }

        [TestCaseSource("_toolbarCommands")]
        public void Ensure_all_tortoisegit_commands_exist(ToolbarCommand toolbarCommand)
        {
            var command = GetMenuCommand(toolbarCommand);
            
            Assert.That(command, Is.Not.Null, string.Format("Couldn't find command for {0}", toolbarCommand));
        }

        [TestCaseSource("_toolbarCommands")]
        public void Ensure_all_tortoisegit_commands_bind_to_event_handlers(ToolbarCommand toolbarCommand)
        {
            var command = GetMenuCommand(toolbarCommand);

            var execHandler = typeof(MenuCommand).GetField("execHandler", BindingFlags.NonPublic | BindingFlags.Instance);
            
            Assert.That(execHandler, Is.Not.Null);
            Assert.That(execHandler.GetValue(command), Is.Not.Null);
        }

        [TestCaseSource("_toolbarCommands")]
        public void Invoke_all_command_handlers_without_exception(ToolbarCommand toolbarCommand)
        {
            try
            {
                var uishellMock = UIShellServiceMock.GetUiShellInstance();
                _serviceProvider.AddService(typeof(SVsUIShell), uishellMock, true);
                var command = GetMenuCommand(toolbarCommand);
                var execHandler = (EventHandler) typeof(MenuCommand).GetField("execHandler", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(command);

                TestDelegate commandHandler = () => execHandler.Invoke(null, null);

                //Todo: block dialog (asserting which dialog was invoked if possible)
                Assert.DoesNotThrow(commandHandler);
            }
            finally
            {
                _serviceProvider.RemoveService(typeof(SVsUIShell));
            }
        }

        private MenuCommand GetMenuCommand(ToolbarCommand toolbarCommand)
        {
            _package.SetSite(_serviceProvider);

            var menuCommandID = new CommandID(PackageConstants.guidTortoiseGitToolbarCmdSet, (int)toolbarCommand);
            var menuCommandService = _getServiceMethod.Invoke(_package, new object[] { (typeof(IMenuCommandService)) }) as OleMenuCommandService;
            return menuCommandService != null ? menuCommandService.FindCommand(menuCommandID) : null;
        }
    }
}

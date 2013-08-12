using System;
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
    public class TortoiseGitToolbarPackageShould
    {
        private IVsPackage _package;
        private MethodInfo _getServiceMethod;
        private OleServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            _package = new TortoiseGitToolbarPackage();
            _serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
            _package.SetSite(_serviceProvider);
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

        [TestCase(CommandIdConstants.CmdCommit, "cmdCommit")]
        [TestCase(CommandIdConstants.CmdResolve, "cmdResolve")]
        [TestCase(CommandIdConstants.CmdPull, "cmdPull")]
        [TestCase(CommandIdConstants.CmdPush, "cmdPush")]
        [TestCase(CommandIdConstants.CmdLog, "cmdLog")]
        [TestCase(CommandIdConstants.CmdBash, "cmdBash")]
        public void Ensure_all_tortoisegit_commands_exist(uint commandId, string handlerName)
        {
            var command = GetMenuCommand(commandId);
            
            Assert.That(command, Is.Not.Null, string.Format("Couldn't find command for {0}", handlerName));
        }

        [TestCase(CommandIdConstants.CmdCommit, "Commit")]
        [TestCase(CommandIdConstants.CmdResolve, "Resolve")]
        [TestCase(CommandIdConstants.CmdPull, "Pull")]
        [TestCase(CommandIdConstants.CmdPush, "Push")]
        [TestCase(CommandIdConstants.CmdLog, "Log")]
        [TestCase(CommandIdConstants.CmdBash, "Bash")]
        public void Ensure_all_tortoisegit_commands_bind_to_correct_event_handlers(uint commandId, string handlerName)
        {
            var command = GetMenuCommand(commandId);

            var execHandler = typeof(MenuCommand).GetField("execHandler", BindingFlags.NonPublic | BindingFlags.Instance);
            
            Assert.That(execHandler, Is.Not.Null);
            Assert.That(((EventHandler) execHandler.GetValue(command)).Method.Name, Is.EqualTo(handlerName));
        }

        [TestCase("Commit")]
        [TestCase("Resolve")]
        [TestCase("Pull")]
        [TestCase("Push")]
        [TestCase("Log")]
        [TestCase("Bash")]
        public void Invoke_all_command_handlers_without_exception(string commandHandlerName)
        {
            try
            {
                var uishellMock = UIShellServiceMock.GetUiShellInstance();
                _serviceProvider.AddService(typeof (SVsUIShell), uishellMock, true);

                var commandHandler = _package.GetType().GetMethod(commandHandlerName, BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.That(commandHandler, Is.Not.Null, string.Format("Failed to get the private method {0}", commandHandlerName));
                commandHandler.Invoke(_package, new object[] {null, null});
            }
            finally
            {
                _serviceProvider.RemoveService(typeof(SVsUIShell));
            }
        }

        private MenuCommand GetMenuCommand(uint commandId)
        {
            var menuCommandID = new CommandID(PackageConstants.guidTortoiseGitToolbarCmdSet, (int)commandId);
            var menuCommandService = _getServiceMethod.Invoke(_package, new object[] { (typeof(IMenuCommandService)) }) as OleMenuCommandService;
            return menuCommandService != null ? menuCommandService.FindCommand(menuCommandID) : null;
        }
    }
}

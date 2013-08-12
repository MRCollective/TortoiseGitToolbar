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
        public void Ensure_all_tortoisegit_commands_exist(uint commandId, string commandName)
        {
            var menuCommandID = new CommandID(PackageConstants.guidTortoiseGitToolbarCmdSet, (int)commandId);

            var menuCommandService = _getServiceMethod.Invoke(_package, new object[] { (typeof(IMenuCommandService)) }) as OleMenuCommandService;

            Assert.That(menuCommandService, Is.Not.Null, "Menu command service was null");
            Assert.That(menuCommandService.FindCommand(menuCommandID), Is.Not.Null, string.Format("Couldn't find command {0}", commandName));
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
    }
}

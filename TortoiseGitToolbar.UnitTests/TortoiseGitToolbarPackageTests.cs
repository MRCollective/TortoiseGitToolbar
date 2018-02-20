using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using FizzWare.NBuilder;
using MattDavies.TortoiseGitToolbar;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using MattDavies.TortoiseGitToolbar.Services;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VsSDK.UnitTestLibrary;
using NSubstitute;
using TortoiseGitToolbar.UnitTests.Helpers;
using Xunit;

namespace TortoiseGitToolbar.UnitTests
{
    public class TortoiseGitToolbarPackageShould
    {
        private readonly IVsPackage _package;
        private readonly OleServiceProvider _serviceProvider;
        public static IEnumerable<object[]> TortoiseCommands = EnumHelper.GetValues<ToolbarCommand>().Select(t => new object[] {t});

        public TortoiseGitToolbarPackageShould()
        {
            _package = new TortoiseGitToolbarPackage();
            _serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
        }

        [Fact]
        public void Implement_vspackage()
        {
            Assert.NotNull(_package);
        }

        [Fact]
        public void Correctly_set_site()
        {
            Assert.Equal(VSConstants.S_OK, _package.SetSite(_serviceProvider));
        }

        [Theory]
        [MemberData(nameof(TortoiseCommands))]
        public void Ensure_all_tortoisegit_commands_exist(ToolbarCommand toolbarCommand)
        {
            var command = GetMenuCommand(toolbarCommand);
            
            Assert.NotNull(command);
        }

        [Theory]
        [MemberData(nameof(TortoiseCommands))]
        public void Ensure_all_tortoisegit_commands_bind_to_event_handlers(ToolbarCommand toolbarCommand)
        {
            var command = GetMenuCommand(toolbarCommand);

            var execHandler = typeof(MenuCommand).GetField("execHandler", BindingFlags.NonPublic | BindingFlags.Instance);
            
            Assert.NotNull(execHandler);
            Assert.NotNull(execHandler.GetValue(command));
        }

        [Theory]
        [MemberData(nameof(TortoiseCommands))]
        public void Invoke_all_command_handlers_without_exception(ToolbarCommand toolbarCommand)
        {
            try
            {
                var uishellMock = UIShellServiceMock.GetUiShellInstance();
                _serviceProvider.AddService(typeof(SVsUIShell), uishellMock, true);
                var tortoiseGitLauncherService = Substitute.For<ITortoiseGitLauncherService>();
                _serviceProvider.AddService(typeof(TortoiseGitLauncherService), tortoiseGitLauncherService, true);
                var command = GetMenuCommand(toolbarCommand);
                var execHandler = (EventHandler) typeof(MenuCommand).GetField("execHandler", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(command);

                execHandler.Invoke(null, null);
                
                tortoiseGitLauncherService.Received().ExecuteTortoiseProc(toolbarCommand);
            }
            finally
            {
                _serviceProvider.RemoveService(typeof(SVsUIShell));
            }
        }

        private MenuCommand GetMenuCommand(ToolbarCommand toolbarCommand)
        {
            _package.SetSite(_serviceProvider);

            var getServiceMethod = typeof(Package).GetMethod("GetService", BindingFlags.Instance | BindingFlags.NonPublic);
            var menuCommandId = new CommandID(PackageConstants.GuidTortoiseGitToolbarCmdSet, (int)toolbarCommand);
            var menuCommandService = getServiceMethod.Invoke(_package, new object[] { typeof(IMenuCommandService) }) as OleMenuCommandService;
            return menuCommandService?.FindCommand(menuCommandId);
        }
    }
}

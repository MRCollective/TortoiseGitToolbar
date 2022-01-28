using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using MattDavies.TortoiseGitToolbar;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using MattDavies.TortoiseGitToolbar.Services;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Sdk.TestFramework;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NSubstitute;
using TortoiseGitToolbar.UnitTests.Helpers;
using Xunit;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace TortoiseGitToolbar.UnitTests
{
    [Collection(MockedVS.Collection)]
    public class TortoiseGitToolbarPackageShould
    {
        private readonly IVsPackage _package;
        private readonly GlobalServiceProvider _serviceProvider;
        private readonly ITortoiseGitLauncherService _tortoiseGitLauncherService;
        public static IEnumerable<object[]> TortoiseCommands = Enum.GetValues(typeof(ToolbarCommand)).Cast<ToolbarCommand>().Select(t => new object[] {t});

        public TortoiseGitToolbarPackageShould(GlobalServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _serviceProvider.Reset();
            _tortoiseGitLauncherService = Substitute.For<ITortoiseGitLauncherService>();
            _serviceProvider.AddService(typeof(TortoiseGitLauncherService), _tortoiseGitLauncherService);
            _package = new TortoiseGitToolbarPackage();
            _package.SetSite((IServiceProvider)ServiceProvider.GlobalProvider.GetService(typeof(IServiceProvider)));
        }

        [Fact]
        public void Implement_vspackage()
        {
            Assert.NotNull(_package);
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
            var command = GetMenuCommand(toolbarCommand);
            var execHandler = (EventHandler) typeof(MenuCommand).GetField("execHandler", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(command);

            try
            {
                execHandler.Invoke(null, null);
            } catch (MissingManifestResourceException)
            {
                // The test project does not contain the resource file for messages
            }
                
            _tortoiseGitLauncherService.Received().ExecuteTortoiseProc(toolbarCommand);
        }

        private MenuCommand GetMenuCommand(ToolbarCommand toolbarCommand)
        {
            var getServiceMethod = typeof(Package).GetMethod("GetService", BindingFlags.Instance | BindingFlags.NonPublic);
            var menuCommandId = new CommandID(PackageConstants.GuidTortoiseGitToolbarCmdSet, (int)toolbarCommand);
            var menuCommandService = getServiceMethod.Invoke(_package, new object[] { typeof(IMenuCommandService) }) as OleMenuCommandService;
            return menuCommandService?.FindCommand(menuCommandId);
        }
    }
}

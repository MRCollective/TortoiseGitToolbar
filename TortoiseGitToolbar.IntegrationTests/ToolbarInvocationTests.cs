using System;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using TortoiseGitToolbar.IntegrationTests.Helpers;
using Xunit;

namespace TortoiseGitToolbar.IntegrationTests
{
    public class ToolbarInvocationShould
    {
        [VsFact(UIThread = true)]
        public void Launch_all_commands()
        {
            foreach (var toolbarCommand in Enum.GetValues(typeof(ToolbarCommand)).Cast<ToolbarCommand>().Where(v => v != ToolbarCommand.Bash && v != ToolbarCommand.FileBlame && v != ToolbarCommand.FileDiff && v != ToolbarCommand.FileLog))
            {
                InvokeCommand(toolbarCommand);
            }
        }

        private static void InvokeCommand(ToolbarCommand toolbarCommand)
        {
            var menuItemCmd = new CommandID(PackageConstants.GuidTortoiseGitToolbarCmdSet, (int)toolbarCommand);

            using (var dialogboxPurger = new DialogBoxPurger(NativeMethods.IDOK, 1))
            {
                dialogboxPurger.Start();

                ExecuteCommand(menuItemCmd);
            }
        }

        private static void ExecuteCommand(CommandID cmd)
        {
            object customin = null;
            object customout = null;
            var guidString = cmd.Guid.ToString("B").ToUpper();
            var cmdId = cmd.ID;
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            dte.Commands.Raise(guidString, cmdId, ref customin, ref customout);
        }
    }
}
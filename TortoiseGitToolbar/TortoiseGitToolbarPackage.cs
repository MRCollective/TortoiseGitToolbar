using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using EnvDTE;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using MattDavies.TortoiseGitToolbar.Services;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace MattDavies.TortoiseGitToolbar
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageConstants.guidTortoiseGitToolbarPkgString)]
    [ProvideKeyBindingTable(PackageConstants.guidTortoiseGitToolbarPkgString, 110)]
    public sealed class TortoiseGitToolbarPackage : Package
    {
        private OleMenuCommandService _commandService;
        private TortoiseGitLauncherService _tortoiseGitLauncherService;

        protected override void Initialize()
        {
            base.Initialize();

            _commandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            var dte = ((DTE)GetService(typeof(DTE)));
            _tortoiseGitLauncherService = new TortoiseGitLauncherService(dte != null ? dte.Solution : null);
            
            RegisterCommand(CommandId.CmdCommit, Commit);
            RegisterCommand(CommandId.CmdResolve, Resolve);
            RegisterCommand(CommandId.CmdPush, Push);
            RegisterCommand(CommandId.CmdPull, Pull);
            RegisterCommand(CommandId.CmdLog, Log);
            RegisterCommand(CommandId.CmdBash, Bash);
        }

        private void Commit(object sender, EventArgs e)
        {
            _tortoiseGitLauncherService.ExecuteTortoiseProc("commit");
        }

        private void Resolve(object sender, EventArgs e)
        {
            _tortoiseGitLauncherService.ExecuteTortoiseProc("resolve");
        }

        private void Push(object sender, EventArgs e)
        {
            _tortoiseGitLauncherService.ExecuteTortoiseProc("push");
        }

        private void Pull(object sender, EventArgs e)
        {
            _tortoiseGitLauncherService.ExecuteTortoiseProc("pull");
        }

        private void Log(object sender, EventArgs e)
        {
            _tortoiseGitLauncherService.ExecuteTortoiseProc("log");
        }

        private void Bash(object sender, EventArgs e)
        {
            _tortoiseGitLauncherService.ExecuteTortoiseProc("bash");
        }

        private void RegisterCommand(CommandId id, EventHandler callback)
        {
            var menuCommandID = new CommandID(PackageConstants.guidTortoiseGitToolbarCmdSet, (int)id);
            var menuItem = new OleMenuCommand(callback, menuCommandID);
            _commandService.AddCommand(menuItem);
        }
    }
}

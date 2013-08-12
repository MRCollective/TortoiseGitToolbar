using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using EnvDTE;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using MattDavies.TortoiseGitToolbar.Services;
using Microsoft.VisualStudio.Shell;

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
            _tortoiseGitLauncherService = new TortoiseGitLauncherService(((DTE) GetService(typeof(DTE))).Solution);

            RegisterCommand(CommandIdConstants.CmdCommit, Commit);
            RegisterCommand(CommandIdConstants.CmdResolve, Resolve);
            RegisterCommand(CommandIdConstants.CmdPush, Push);
            RegisterCommand(CommandIdConstants.CmdPull, Pull);
            RegisterCommand(CommandIdConstants.CmdLog, Log);
            RegisterCommand(CommandIdConstants.CmdBash, Bash);
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

        private void RegisterCommand(uint id, EventHandler callback)
        {
            var menuCommandID = new CommandID(PackageConstants.guidTortoiseGitToolbarCmdSet, (int)id);
            var menuItem = new OleMenuCommand(callback, menuCommandID);
            _commandService.AddCommand(menuItem);
        }
    }
}

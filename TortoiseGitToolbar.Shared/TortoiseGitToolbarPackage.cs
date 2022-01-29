using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using MattDavies.TortoiseGitToolbar.Services;
using Microsoft.VisualStudio.Shell;

namespace MattDavies.TortoiseGitToolbar
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageConstants.GuidTortoiseGitToolbarPkgString)]
    [ProvideKeyBindingTable(PackageConstants.GuidTortoiseGitToolbarPkgString, 110)]
    public sealed class TortoiseGitToolbarPackage : Package
    {
        private OleMenuCommandService _commandService;
        private ITortoiseGitLauncherService _tortoiseGitLauncherService;
        
        protected override void Initialize()
        {
            base.Initialize();
            SetDependencies();

            foreach (ToolbarCommand toolbarCommand in Enum.GetValues(typeof(ToolbarCommand)))
            {
                RegisterCommand(toolbarCommand, (s, e) => _tortoiseGitLauncherService.ExecuteTortoiseProc(toolbarCommand));
            }
        }

        //Todo: See if this can be refactored with better IoC
        private void SetDependencies()
        {
            var dte = ((DTE)GetService(typeof(DTE)));
            var solution = dte != null ? (Solution2)dte.Solution : null;
            
            _commandService = (OleMenuCommandService) GetService(typeof (IMenuCommandService));
            _tortoiseGitLauncherService = (ITortoiseGitLauncherService) GetService(typeof (TortoiseGitLauncherService))
                ?? new TortoiseGitLauncherService(new ProcessManagerService(), solution);
        }

        private void RegisterCommand(ToolbarCommand id, EventHandler callback)
        {
            var menuCommandID = new CommandID(PackageConstants.GuidTortoiseGitToolbarCmdSet, (int)id);
            var menuItem = new OleMenuCommand(callback, menuCommandID);
            menuItem.Visible = false;
            _commandService.AddCommand(menuItem);
        }
    }
}

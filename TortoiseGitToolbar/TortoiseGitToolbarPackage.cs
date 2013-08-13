using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
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
        private ITortoiseGitLauncherService _tortoiseGitLauncherService;
        
        protected override void Initialize()
        {
            base.Initialize();
            
            _commandService = (OleMenuCommandService) GetService(typeof(IMenuCommandService));
            _tortoiseGitLauncherService = (ITortoiseGitLauncherService) GetService(typeof (TortoiseGitLauncherService));
            if (_tortoiseGitLauncherService == null)
            {
                var dte = ((DTE)GetService(typeof(DTE)));
                _tortoiseGitLauncherService = new TortoiseGitLauncherService(dte != null ? (Solution2)dte.Solution : null);
            }

            foreach (ToolbarCommand toolbarCommand in Enum.GetValues(typeof(ToolbarCommand)))
            {
                RegisterCommand(toolbarCommand, (s, e) => _tortoiseGitLauncherService.ExecuteTortoiseProc(toolbarCommand));
            }
        }
        
        private void RegisterCommand(ToolbarCommand id, EventHandler callback)
        {
            var menuCommandID = new CommandID(PackageConstants.guidTortoiseGitToolbarCmdSet, (int)id);
            var menuItem = new OleMenuCommand(callback, menuCommandID);
            _commandService.AddCommand(menuItem);
        }
    }
}

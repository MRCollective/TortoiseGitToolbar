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

        private void SetDependencies()
        {
            _commandService = (OleMenuCommandService)GetService(typeof(IMenuCommandService));

            // Pass a method that obtains a fresh Solution2 on every call.
            // Using a method on the Package avoids interop type-identity problems
            // that appear with lambdas across VS 2022 / VS 2026.
            _tortoiseGitLauncherService = new TortoiseGitLauncherService(
                new ProcessManagerService(),
                GetCurrentSolution);
        }

        /// <summary>
        /// Returns the current Solution2. Called on every command so we never
        /// hold a stale reference (important for .slnx / Open Folder in VS 2026).
        /// </summary>
        private Solution2 GetCurrentSolution()
        {
            // Prefer the modern service identifier when available, fall back to the classic one.
            var dte = GetService(typeof(DTE)) as DTE;

            return dte?.Solution as Solution2;
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

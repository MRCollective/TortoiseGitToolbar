using System.Diagnostics;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using Microsoft.VisualStudio.Shell.Interop;
using Xunit;

[assembly: VsixRunner(TraceLevel = SourceLevels.All)]
namespace TortoiseGitToolbar.IntegrationTests
{
    public class PackageShould
    {
        [VsixFact(VisualStudioVersion.Current, RootSuffix = "Exp")]
        public void Load_shell_service()
        {
            var shellService = GlobalServices.GetService<SVsShell>() as IVsShell;
            
            Assert.NotNull(shellService);
        }

        [VsixFact(VisualStudioVersion.Current, RootSuffix = "Exp", RunOnUIThread = true)]
        public void Load_into_the_ide()
        {
            IVsPackage package;
            var shellService = GlobalServices.GetService<SVsShell>() as IVsShell;
            var packageGuid = PackageConstants.GuidTortoiseGitToolbarPkg;

            var packageLoaded = shellService.LoadPackage(ref packageGuid, out package);

            Assert.True(packageLoaded == 0);
            Assert.NotNull(package);
        }
    }
}
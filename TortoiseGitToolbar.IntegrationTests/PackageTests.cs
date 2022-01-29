using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Xunit;

namespace TortoiseGitToolbar.IntegrationTests
{
    public class PackageShould
    {
        [VsTheory(Version = "2022-")]
        [InlineData(PackageConstants.GuidTortoiseGitToolbarPkgString, true)]
        [InlineData("11111111-2222-3333-4444-555555555555", false)]
        public async Task Load_into_the_ide(string guidString, bool expectedSuccess)
        {
            var shell = (IVsShell7)ServiceProvider.GlobalProvider.GetService(typeof(SVsShell));
            Assert.NotNull(shell);

            var guid = Guid.Parse(guidString);

            if (expectedSuccess)
                await shell.LoadPackageAsync(ref guid);
            else
                await Assert.ThrowsAnyAsync<Exception>(async () => await shell.LoadPackageAsync(ref guid));
        }
    }
}
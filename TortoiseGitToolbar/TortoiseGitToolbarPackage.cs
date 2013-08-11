using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace MattDavies.TortoiseGitToolbar
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidTortoiseGitToolbarPkgString)]
    public sealed class TortoiseGitToolbarPackage : Package
    {
        public TortoiseGitToolbarPackage()
        {
            Debug.WriteLine(CultureInfo.CurrentCulture.ToString(), "Entering constructor for: {0}", this);
        }

        protected override void Initialize()
        {
            Debug.WriteLine(CultureInfo.CurrentCulture.ToString(), "Entering Initialize() of: {0}", this);
            base.Initialize();
        }
    }
}

using System;

namespace MattDavies.TortoiseGitToolbar.Config.Constants
{
    public static class PackageConstants
    {
        public const string GuidTortoiseGitToolbarPkgString = "f388ee16-eef2-4ae1-85bd-4cb19151beb0";
        public const string GuidTortoiseGitToolbarCmdSetString = "b594f95f-5d4d-4383-be94-e21d105fa58c";

        public static readonly Guid GuidTortoiseGitToolbarCmdSet = new Guid(GuidTortoiseGitToolbarCmdSetString);
        public static readonly Guid GuidTortoiseGitToolbarPkg = new Guid(GuidTortoiseGitToolbarPkgString);
    };
}
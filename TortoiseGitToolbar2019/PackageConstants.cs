using System;
using System.Reflection;

namespace MattDavies.TortoiseGitToolbar.Config.Constants
{
    public static class PackageConstants
    {
        public const string GuidTortoiseGitToolbarPkgString = "d09f7b88-0165-4ec2-94da-cdb0c9fc914a";
        public const string GuidTortoiseGitToolbarCmdSetString = "46533cec-75d7-43bf-bfbb-910b527b9b4a";

        public static readonly Guid GuidTortoiseGitToolbarCmdSet = new Guid(GuidTortoiseGitToolbarCmdSetString);
        public static readonly Guid GuidTortoiseGitToolbarPkg = new Guid(GuidTortoiseGitToolbarPkgString);
    };
}
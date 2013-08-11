// Guids.cs
// MUST match guids.h

using System;

namespace MattDavies.TortoiseGitToolbar.Config.Constants
{
    public static class PackageConstants
    {
        public const string guidTortoiseGitToolbarPkgString = "f388ee16-eef2-4ae1-85bd-4cb19151beb0";
        public const string guidTortoiseGitToolbarCmdSetString = "b594f95f-5d4d-4383-be94-e21d105fa58c";

        public static readonly Guid guidTortoiseGitToolbarCmdSet = new Guid(guidTortoiseGitToolbarCmdSetString);
    };
}
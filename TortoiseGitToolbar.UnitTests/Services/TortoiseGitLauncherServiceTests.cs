using System;
using System.Reflection;
using EnvDTE;
using EnvDTE80;
using FizzWare.NBuilder;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using MattDavies.TortoiseGitToolbar.Services;
using NSubstitute;
using NUnit.Framework;

namespace TortoiseGitToolbar.UnitTests.Services
{
    [TestFixture]
    public class TortoiseGitLauncherServiceShould
    {
        private readonly ToolbarCommand[] _toolbarCommands = EnumHelper.GetValues<ToolbarCommand>();
        private TortoiseGitLauncherService _tortoiseGitLauncherService;
        private IProcessManagerService _processManagerService;
        private Solution2 _solution;

        [SetUp]
        public void Setup()
        {
            _solution = Substitute.For<Solution2>();
            _solution.IsOpen.Returns(true);
            _solution.FullName.Returns(Environment.CurrentDirectory + "\\file.sln");
            _processManagerService = Substitute.For<IProcessManagerService>();
            _tortoiseGitLauncherService = Substitute.For<TortoiseGitLauncherService>(_processManagerService, _solution);
        }

        [TestCaseSource("_toolbarCommands")]
        public void Launch_command_with_correct_parameters(ToolbarCommand toolbarCommand)
        {
            var solutionPath = PathConfiguration.GetSolutionPath(_solution);

            _tortoiseGitLauncherService.ExecuteTortoiseProc(toolbarCommand);

            _processManagerService.Received().GetProcess(
                GetExpectedCommand(toolbarCommand),
                GetExpectedParameters(toolbarCommand),
                toolbarCommand == ToolbarCommand.Bash ? solutionPath : null
            );
        }

        private static string GetExpectedCommand(ToolbarCommand toolbarCommand)
        {
            switch (toolbarCommand)
            {
                case ToolbarCommand.Commit:
                case ToolbarCommand.Log:
                case ToolbarCommand.Pull:
                case ToolbarCommand.Push:
                case ToolbarCommand.Resolve:
                    return PathConfiguration.GetTortoiseGitPath();
                case ToolbarCommand.Bash:
                    return PathConfiguration.GetGitBashPath();
            }

            throw new InvalidOperationException(string.Format("You need to define an expected test process command result for {0}.", toolbarCommand));
        }

        private static string GetExpectedParameters(ToolbarCommand toolbarCommand)
        {
            switch (toolbarCommand)
            {
                case ToolbarCommand.Bash:
                    return "--login -i";
                case ToolbarCommand.Commit:
                    return string.Format(@"/command:commit /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.Log:
                    return string.Format(@"/command:log /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.Pull:
                    return string.Format(@"/command:pull /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.Push:
                    return string.Format(@"/command:push /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.Resolve:
                    return string.Format(@"/command:resolve /path:""{0}""", Environment.CurrentDirectory);
            }

            throw new InvalidOperationException(string.Format("You need to define an expected test process parameters result for {0}.", toolbarCommand));
        }
    }
}
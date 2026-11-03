using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE80;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using MattDavies.TortoiseGitToolbar.Services;
using Microsoft.VisualStudio.Sdk.TestFramework;
using NSubstitute;
using TortoiseGitToolbar.UnitTests.Helpers;
using Xunit;

namespace TortoiseGitToolbar.UnitTests.Services
{
    [Collection(MockedVS.Collection)]
    public class TortoiseGitLauncherServiceShould
    {
        public static IEnumerable<object[]> TortoiseCommands = Enum.GetValues(typeof(ToolbarCommand)).Cast<ToolbarCommand>().Where(t => t != ToolbarCommand.Bash && t != ToolbarCommand.RebaseContinue).Select(t => new object[] { t });
        private readonly IProcessManagerService _processManagerService;
        private static readonly string TestFilePath = Path.Combine(Environment.CurrentDirectory, "test.txt");
        private const int CurrentLine = 42;

        public TortoiseGitLauncherServiceShould()
        {
            _processManagerService = Substitute.For<IProcessManagerService>();
        }

        [Theory]
        [MemberData(nameof(TortoiseCommands))]
        public void Launch_tortoise_command_with_correct_parameters(ToolbarCommand toolbarCommand)
        {
            var solution = GetOpenSolution();
            var tortoiseGitLauncherService = Substitute.For<TortoiseGitLauncherService>(_processManagerService, solution);

            tortoiseGitLauncherService.ExecuteTortoiseProc(toolbarCommand);

            _processManagerService.Received().GetProcess(
                GetExpectedCommand(toolbarCommand),
                GetExpectedParameters(toolbarCommand)
            );
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Launch_git_bash(bool solutionOpen)
        {
            var solution = solutionOpen ? GetOpenSolution() : GetClosedSolution();
            var tortoiseGitLauncherService = Substitute.For<TortoiseGitLauncherService>(_processManagerService, solution);
            const ToolbarCommand command = ToolbarCommand.Bash;

            tortoiseGitLauncherService.ExecuteTortoiseProc(command);

            _processManagerService.Received().GetProcess(
                GetExpectedCommand(command),
                GetExpectedParameters(command),
                PathConfiguration.GetRepositoryRootPath(solution)
            );
        }

        [Fact]
        public void Launch_rebase_continue_in_git_bash()
        {
            var solution = GetOpenSolution();
            var tortoiseGitLauncherService = Substitute.For<TortoiseGitLauncherService>(_processManagerService, solution);
            const ToolbarCommand command = ToolbarCommand.RebaseContinue;

            tortoiseGitLauncherService.ExecuteTortoiseProc(command);

            _processManagerService.Received().GetProcess(
                GetExpectedCommand(command),
                GetExpectedParameters(command),
                PathConfiguration.GetRepositoryRootPath(solution)
            );
        }

        [Fact]
        public void Get_solution_folder_traverses_parents_till_git_folder_found()
        {
            var solution = GetOpenSolution();
            var gitRepoRoot = PathConfiguration.GetRepositoryRootPath(solution);
            var dotGitPath = Path.Combine(gitRepoRoot, ".git");
            Assert.True(Directory.Exists(dotGitPath) || File.Exists(dotGitPath), "Returned path is not the repository root.");
        }

        private static Solution2 GetOpenSolution()
        {
            var solution = Substitute.For<Solution2>();
            solution.IsOpen.Returns(true);
            solution.FullName.Returns(Environment.CurrentDirectory + "\\file.sln");
            solution.DTE.ActiveDocument.Returns(new DocumentMock(CurrentLine, TestFilePath));
            return solution;
        }

        private static Solution2 GetClosedSolution()
        {
            var solution = Substitute.For<Solution2>();
            solution.IsOpen.Returns(false);
            solution.FullName.Returns(string.Empty);
            return solution;
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
                case ToolbarCommand.Switch:
                case ToolbarCommand.Cleanup:
                case ToolbarCommand.Fetch:
                case ToolbarCommand.Revert:
                case ToolbarCommand.Sync:
                case ToolbarCommand.Merge:
                case ToolbarCommand.StashSave:
                case ToolbarCommand.StashPop:
                case ToolbarCommand.StashList:
                case ToolbarCommand.Rebase:
                case ToolbarCommand.FileLog:
                case ToolbarCommand.FileDiff:
                case ToolbarCommand.FileBlame:
                    return PathConfiguration.GetTortoiseGitPath();
                case ToolbarCommand.Bash:
                case ToolbarCommand.RebaseContinue:
                    return PathConfiguration.GetGitBashPath();
            }

            throw new InvalidOperationException($"You need to define an expected test process command result for {toolbarCommand}.");
        }

        private static string GetExpectedParameters(ToolbarCommand toolbarCommand)
        {
            var gitRepoRoot = PathConfiguration.GetRepositoryRootPath(GetOpenSolution());
            switch (toolbarCommand)
            {
                case ToolbarCommand.Bash:
                    return "--login -i";
                case ToolbarCommand.RebaseContinue:
                    return @"--login -i -c 'echo; echo ""Running git rebase --continue""; echo; git rebase --continue; echo; echo ""Please review the output above and press enter to continue.""; read'";
                case ToolbarCommand.Commit:
                    return $@"/command:commit /path:""{gitRepoRoot}""";
                case ToolbarCommand.Log:
                    return $@"/command:log /path:""{gitRepoRoot}""";
                case ToolbarCommand.Pull:
                    return $@"/command:pull /path:""{gitRepoRoot}""";
                case ToolbarCommand.Push:
                    return $@"/command:push /path:""{gitRepoRoot}""";
                case ToolbarCommand.Switch:
                    return $@"/command:switch /path:""{gitRepoRoot}""";
                case ToolbarCommand.Cleanup:
                    return $@"/command:cleanup /path:""{gitRepoRoot}""";
                case ToolbarCommand.Fetch:
                    return $@"/command:fetch /path:""{gitRepoRoot}""";
                case ToolbarCommand.Revert:
                    return $@"/command:revert /path:""{gitRepoRoot}""";
                case ToolbarCommand.Sync:
                    return $@"/command:sync /path:""{gitRepoRoot}""";
                case ToolbarCommand.Merge:
                    return $@"/command:merge /path:""{gitRepoRoot}""";
                case ToolbarCommand.Resolve:
                    return $@"/command:resolve /path:""{gitRepoRoot}""";
                case ToolbarCommand.StashSave:
                    return $@"/command:stashsave /path:""{gitRepoRoot}""";
                case ToolbarCommand.StashPop:
                    return $@"/command:stashpop /path:""{gitRepoRoot}""";
                case ToolbarCommand.StashList:
                    return $@"/command:reflog /path:""{gitRepoRoot}"" /ref:""refs/stash""";
                case ToolbarCommand.Rebase:
                    return $@"/command:rebase /path:""{gitRepoRoot}""";
                case ToolbarCommand.FileBlame:
                    return $@"/command:blame /path:""{TestFilePath}"" /line:{CurrentLine}";
                case ToolbarCommand.FileDiff:
                    return $@"/command:diff /path:""{TestFilePath}""";
                case ToolbarCommand.FileLog:
                    return $@"/command:log /path:""{TestFilePath}""";
                default:
                    throw new InvalidOperationException($"You need to define an expected test process parameters result for {toolbarCommand}.");
            }
        }
    }
}
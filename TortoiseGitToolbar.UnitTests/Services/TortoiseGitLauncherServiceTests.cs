using System;
using System.IO;
using System.Linq;
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
        private readonly ToolbarCommand[] _tortoiseCommands = EnumHelper.GetValues<ToolbarCommand>().Where(t => t != ToolbarCommand.Bash && t != ToolbarCommand.RebaseContinue).ToArray();
        private IProcessManagerService _processManagerService;

        [SetUp]
        public void Setup()
        {
            _processManagerService = Substitute.For<IProcessManagerService>();
        }

        [TestCaseSource("_tortoiseCommands")]
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

        [Test]
        public void Launch_git_bash([Values(true,false)] bool solutionOpen)
        {
            var solution = solutionOpen ? GetOpenSolution() : GetClosedSolution();
            var tortoiseGitLauncherService = Substitute.For<TortoiseGitLauncherService>(_processManagerService, solution);
            const ToolbarCommand command = ToolbarCommand.Bash;

            tortoiseGitLauncherService.ExecuteTortoiseProc(command);

            _processManagerService.Received().GetProcess(
                GetExpectedCommand(command),
                GetExpectedParameters(command),
                PathConfiguration.GetSolutionPath(solution)
            );
        }

        [Test]
        public void Launch_rebase_continue_in_git_bash()
        {
            var solution = GetOpenSolution();
            var tortoiseGitLauncherService = Substitute.For<TortoiseGitLauncherService>(_processManagerService, solution);
            const ToolbarCommand command = ToolbarCommand.RebaseContinue;

            tortoiseGitLauncherService.ExecuteTortoiseProc(command);

            _processManagerService.Received().GetProcess(
                GetExpectedCommand(command),
                GetExpectedParameters(command),
                PathConfiguration.GetSolutionPath(solution)
            );
        }

        [Test]
        public void Get_solution_folder_traverses_parents_till_git_folder_found()
        {
            var solution = GetOpenSolution();
            var solutionPath = PathConfiguration.GetSolutionPath(solution);
            Assert.That(Directory.Exists(Path.Combine(solutionPath, ".git")), "Returned solution path is not the repository root.");
        }

        private static readonly string TestFilePath = System.IO.Path.Combine(Environment.CurrentDirectory, "test.txt");
        private const int CurrentLine = 42;

        private static Solution2 GetOpenSolution()
        {
            var solution = Substitute.For<Solution2>();
            solution.IsOpen.Returns(true);
            solution.FullName.Returns(Environment.CurrentDirectory + "\\file.sln");
            // I can't find a way to get working the following:
            // solution.DTE.ActiveDocument.Selection.CurrentLine.Returns(CurrentLine);
            // for dynamic Selection. Hence I created DocumentMock
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

            throw new InvalidOperationException(string.Format("You need to define an expected test process command result for {0}.", toolbarCommand));
        }

        private static string GetExpectedParameters(ToolbarCommand toolbarCommand)
        {
            switch (toolbarCommand)
            {
                case ToolbarCommand.Bash:
                    return "--login -i";
                case ToolbarCommand.RebaseContinue:
                    return @"--login -i -c 'echo; echo ""Running git rebase --continue""; echo; git rebase --continue; echo; echo ""Please review the output above and press enter to continue.""; read'";
                case ToolbarCommand.Commit:
                    return string.Format(@"/command:commit /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.Log:
                    return string.Format(@"/command:log /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.Pull:
                    return string.Format(@"/command:pull /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.Push:
                    return string.Format(@"/command:push /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.Switch:
                    return string.Format(@"/command:switch /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.Cleanup:
                    return string.Format(@"/command:cleanup /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.Fetch:
                    return string.Format(@"/command:fetch /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.Revert:
                    return string.Format(@"/command:revert /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.Sync:
                    return string.Format(@"/command:sync /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.Merge:
                    return string.Format(@"/command:merge /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.Resolve:
                    return string.Format(@"/command:resolve /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.StashSave:
                    return string.Format(@"/command:stashsave /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.StashPop:
                    return string.Format(@"/command:stashpop /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.StashList:
                    return string.Format(@"/command:reflog /path:""{0}"" /ref:""refs/stash""", Environment.CurrentDirectory);
                case ToolbarCommand.Rebase:
                    return string.Format(@"/command:rebase /path:""{0}""", Environment.CurrentDirectory);
                case ToolbarCommand.FileBlame:
                    return string.Format(@"/command:blame /path:""{0}"" /line:{1}", TestFilePath, CurrentLine);
                case ToolbarCommand.FileDiff:
                    return string.Format(@"/command:diff /path:""{0}""", TestFilePath);
                case ToolbarCommand.FileLog:
                    return string.Format(@"/command:log /path:""{0}""", TestFilePath);
            }

            throw new InvalidOperationException(string.Format("You need to define an expected test process parameters result for {0}.", toolbarCommand));
        }
    }
}
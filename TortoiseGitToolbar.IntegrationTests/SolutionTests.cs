using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace TortoiseGitToolbar.IntegrationTests
{
    [TestClass]
    public class SolutionTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [HostType("VS IDE")]
        public void CreateEmptySolution()
        {
            UIThreadInvoker.Invoke((ThreadInvoker) delegate
            {
                var testUtils = new TestUtils();
                testUtils.CloseCurrentSolution(__VSSLNSAVEOPTIONS.SLNSAVEOPT_NoSave);
                testUtils.CreateEmptySolution(TestContext.TestDir, "EmptySolution");
            });
        }

        private delegate void ThreadInvoker();
    }
}
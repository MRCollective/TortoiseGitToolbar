using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;

namespace TortoiseGitToolbar.IntegrationTests
{
    [TestClass]
    public class SolutionTests
    {
        private delegate void ThreadInvoker();

        public TestContext TestContext { get; set; }

        [TestMethod]
        [HostType("VS IDE")]
        public void CreateEmptySolution()
        {
            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                TestUtils testUtils = new TestUtils();
                testUtils.CloseCurrentSolution(__VSSLNSAVEOPTIONS.SLNSAVEOPT_NoSave);
                testUtils.CreateEmptySolution(TestContext.TestDir, "EmptySolution");
            });
        }

    }
}

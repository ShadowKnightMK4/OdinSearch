using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine;
using OdinSearchEngine.OdinSearch_OutputConsumerTools.CmdProcessorTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NonSqlUnitTests
{
    [TestClass]
    public class ExternalPowerShellCmdBash_test
    {
        [TestMethod]
        public void TestCmd_LaunchExplorerPointedtoCmdExe_run4Times()
        {
            string cmd = "explorer.exe /select,\"{0}\"";
            OdinSearch demo = new OdinSearch();
            SearchTarget t = new();
            t.FileName.Add("*.txt");
            SearchAnchor st = new(Environment.GetFolderPath(Environment.SpecialFolder.System));

            OdinSearch_OutputConsumer_CmdProcessor WindowsTest = new();
            WindowsTest.CommandToExecute = cmd;

            Assert.IsTrue(cmd == WindowsTest.CommandToExecute);
            Assert.IsTrue(WindowsTest.WasCommandSet);


            demo.AddSearchAnchor(st);
            demo.AddSearchTarget(t);

            demo.Search(WindowsTest);


            while (demo.IsZombied == false)
            {
                Thread.Sleep(0);
            }

            Console.WriteLine("Explorer should have opened pointed to cmd.exe");
        }
        /// <summary>
        /// you should see explorer open up and point to cmd.exe once this test runs.
        /// </summary>
        [TestMethod]
        public void TestCmd_LaunchExplorerPointedToCmdExe_runonce()
        {
            string cmd = "explorer.exe /select,\"{0}\"";
            OdinSearch demo = new OdinSearch();
            SearchTarget t = new();
            t.FileName.Add("cmd.exe");
            SearchAnchor st = new(Environment.GetFolderPath(Environment.SpecialFolder.System));

            OdinSearch_OutputConsumer_CmdProcessor WindowsTest = new();
            WindowsTest.CommandToExecute =cmd ;
            Assert.IsTrue(cmd == WindowsTest.CommandToExecute);
            Assert.IsTrue(WindowsTest.WasCommandSet);


            demo.AddSearchAnchor(st);
            demo.AddSearchTarget(t);

            demo.Search(WindowsTest);

            
            while (demo.IsZombied ==false)
            {
                Thread.Sleep(0);
            }

            Console.WriteLine("Explorer should have opened pointed to cmd.exe");
        }
    }
}

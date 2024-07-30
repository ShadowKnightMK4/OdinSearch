using Microsoft.VisualStudio.TestTools.UnitTesting;
using NonSqlUnitTests.OlderTests.TestTools;
using OdinSearchEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NonSqlUnitTests.OlderTests.TestFlags_SoleMatch
{
    [TestClass]
    public class OdinSearch_SoleMatch_Tests
    {
        FileItemCache test = new();

        string my_test_folder;
        [TestInitialize]
        public void InitTest()
        {
            my_test_folder = Path.Combine("C:\\ScratchPad", Path.Combine(nameof(OdinSearch_SoleMatch_Tests)));
            FileItemCache.EnsureFolderExists(my_test_folder);
            string tmp = Path.Combine(my_test_folder, "First");
            FileItemCache.EnsureFolderExists(tmp);
            tmp = Path.Combine(my_test_folder, "Second");
            FileItemCache.EnsureFolderExists(tmp);
            test.MakeFile("C:\\ScratchPad", Path.Combine(nameof(OdinSearch_SoleMatch_Tests), "First\\test1.dat"), FileAttributes.Normal);
            test.MakeFile("C:\\ScratchPad", Path.Combine(nameof(OdinSearch_SoleMatch_Tests), "Second\\test1.dat"), FileAttributes.Normal);
        }

        [TestCleanup]
        public void CleanTest()
        {
            test.Purge();
        }
        [TestMethod]
        public void Test_SoleMatchLive_ForFolder_SoleMatchIsTrue_MatchOnly1()
        {
            Test_SoleMatchLive_ForFolder_common(true, 1);
        }

        [TestMethod]
        public void Test_SoleMatchLive_ForFolder_SoleMatchIsFalse_MatchBothOfThem()
        {
            Test_SoleMatchLive_ForFolder_common(false, 2);
        }

        private void Test_SoleMatchLive_ForFolder_common(bool flag, int assert_count)
        {
            OdinSearch TestSearch = new();
            SearchAnchor start = new SearchAnchor(false);
            start.AddAnchor(my_test_folder);
            start.EnumSubFolders = true;
            SearchTarget lookfor = new SearchTarget();
            lookfor.FileName.Add("test1.dat");


            TestSearch.AddSearchAnchor(start);
            TestSearch.AddSearchTarget(lookfor);
            TestSearch.SoleMatch = flag;
            var Results = new OdinSearchEngine.OdinSearch_OutputConsumerTools.OdinSearch_OutputConsumerGatherResults();


            while (true)
            {
                TestSearch.Search(Results);
                Thread.Sleep(200);
                if (TestSearch.HasActiveSearchThreads)
                    TestSearch.WorkerThreadJoin();
                else
                    break;

            }
            Assert.IsTrue(Results.Results.Count == assert_count);
        }

        [TestMethod]
        public void TestForFile()
        {

        }
    }
}

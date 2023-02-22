using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OdinSearchEngine;

namespace NonSqlUnitTests
{
    /// <summary>
    /// Class is were we synthisi serveral data points to look for. The other class <seealso cref="UnitTest_OdinSearchBasics"/> is more for can be spin up the class and start seaching  The name Chalice is reference to a certain game
    /// </summary>
    [TestClass]
    public class UnitTest_OdinSearch_ChaliceTests
    {
        OdinSearch Demo;
        // Test creates files/folders there and then cleans up at the end of the run
        string TestFolderFullLocation;
        const string TestScrubFolderName = "TestScrubLocation";

        static void MakeFolder(string path1, string path2)
        {
            MakeFolder(path1, path2, FileAttributes.Directory);
        }
        static void MakeFolder(string path1, string path2, FileAttributes attributes)
        {
            string targ = Path.Combine(path1, path2);
            Directory.CreateDirectory(targ);

            File.SetAttributes(targ, attributes);
        }

        static void MakeFile(string path1, string path2, FileAttributes attributes)
        {
            MakeFile(path1, path2, attributes, 0);
        }

        static void MakeFile(string path1, string path2, FileAttributes attributes, int len)
        {
            string targ = Path.Combine(path1, path2);
            using (var fn = File.OpenWrite(targ))
            {
                fn.SetLength(len);
            }
            File.SetAttributes(targ, attributes);

        }



        /// <summary>
        /// Make 3 folders in the scrub folder named folder1-3 with no attributes other than folder.   Wildcard wants to match all 3 
        /// 
        /// </summary>
        void test1_populate()
        {
            string targ = TestFolderFullLocation;
            string folder1 = Path.Combine(targ, "test1\\folder1");
            string folder2 = Path.Combine(targ, "test1\\folder2");
            string folder3 = Path.Combine(targ, "test1\\folder3");

            MakeFolder(folder1, "");
            MakeFolder(folder2, "");
            MakeFolder(folder3, "");
        }

        [TestCategory("AVD: Finding under controlled scenerios")]
        [TestMethod]
        public void Find_3_folders_no_special_needs__TEST1()
        {
            OdinSearch_OutputConsumerGatherResults testresults = new OdinSearch_OutputConsumerGatherResults();
            Demo.KillSearch();
            Demo.ClearSearchAnchorList();
            Demo.ClearSearchTargetList();

            SearchAnchor start = new SearchAnchor();
            start.AddAnchor(Path.Combine(TestFolderFullLocation, "test1"));
            SearchTarget lookfor = new SearchTarget();
            lookfor.FileName.Add("folder*");


            Demo.AddSearchAnchor(start);
            Demo.AddSearchTarget(lookfor);

            Demo.Search(testresults);
            Demo.WorkerThreadJoin();

            Assert.IsTrue(testresults.Results.Count == 3);

            
            Demo.KillSearch();


            Demo.ClearSearchAnchorList();
            Demo.ClearSearchTargetList();
        }

        /*
         * We create/copy stuff into the folder pointed to by TestFolderFullLocaiton based on known stats.
         */
        void populuate_for_tests()
        {
            test1_populate();
        }


        [TestInitialize]
        public void Init()
        {
            string target = Path.Combine(Path.GetPathRoot(Assembly.GetExecutingAssembly().Location), TestScrubFolderName);

            if (!Directory.Exists(target)) { 
                Directory.CreateDirectory(target);
            }
            TestFolderFullLocation = target;

            Demo = new OdinSearch();


            populuate_for_tests();
        }



        /// <summary>
        /// we delete the folder we created.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(TestFolderFullLocation))
            {
                Directory.Delete(TestFolderFullLocation, true);
            }
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OdinSearchEngine;
using OdinSearchEngine.OdinSearch_OutputConsumerTools;
using System.Threading;
using System.Diagnostics;

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

        string test2_original;
        /// <summary>
        /// Copy the first file we match in C: windows
        /// </summary>
        void test2_populate()
        {
            
            string targ = TestFolderFullLocation;
            //string copysource = Path.Combine(Environment.GetEnvironmentVariable("%WINDIR%"),"\\");
            MakeFolder(targ, "test2");
            test2_original = Path.Combine(targ, "test2", "bob.tmp");
            MakeFile(string.Empty, test2_original, FileAttributes.Temporary);
          
        }

        void test3_populate()
        {
            string targ = TestFolderFullLocation;
            string file1 = Path.Combine(targ, "test3", "nonamematchme.dat");
            string file2 = Path.Combine(targ, "test3", "nonamematchme2.dat");
            string file3 = Path.Combine(targ, "test3", "nonameDONTmatch.dat");
            string file4 = Path.Combine(targ, "test3","nonameDONTmatch2.dat");
            MakeFolder(targ, "test3");
            MakeFile(string.Empty,file1 , FileAttributes.Archive);
            MakeFile(string.Empty, file2, FileAttributes.Archive);
            MakeFile(string.Empty, file3, FileAttributes.Archive | FileAttributes.Hidden);
            MakeFile(string.Empty, file4, FileAttributes.Archive | FileAttributes.Hidden);

        }

        [TestCategory("AVD: Finding under controlled scenerios")]
        [TestMethod]
        public void File_recently_created_file_no_older_than_1_day_TEST2()
        {
            OdinSearch_OutputConsumerGatherResults testresults = new OdinSearch_OutputConsumerGatherResults();
            Demo.KillSearch();
            Demo.ClearSearchTargetList();
            Demo.ClearSearchAnchorList();

            SearchAnchor start = new SearchAnchor(false);
            start.AddAnchor(Path.Combine(TestFolderFullLocation, "test2"));
            SearchTarget lookfor = new SearchTarget();
            lookfor.FileName.Add(Path.GetFileName(test2_original));
            lookfor.CreationAnchorCheck1 = SearchTarget.MatchStyleDateTime.NoEarlierThanThis;
            lookfor.CreationAnchor = DateTime.Now;
            Thread.Sleep(1000);
            lookfor.CreationAnchor = lookfor.CreationAnchor.AddSeconds(10);

            Demo.AddSearchAnchor(start);
            Demo.AddSearchTarget(lookfor);

            Demo.Search(testresults);
            Demo.WorkerThreadJoin();

            Assert.IsTrue(testresults.Results.Count == 0);
            Demo.KillSearch();

            testresults.Results.Clear();


            Demo.Search(testresults);
            Demo.WorkerThreadJoin();

            lookfor.CreationAnchorCheck1 = SearchTarget.MatchStyleDateTime.Disable;

            

        }

        [TestCategory("AVD: Finding under controlled scenerios")]
        [TestMethod]
        public void FileFilesMatching_ArchiveButNotHidden__TEST3()
        {
            OdinSearch_OutputConsumerGatherResults testresults = new OdinSearch_OutputConsumerGatherResults();
            Demo.KillSearch();
            Demo.ClearSearchAnchorList();
            Demo.ClearSearchTargetList();

            SearchAnchor start = new SearchAnchor(false);
            start.AddAnchor(Path.Combine(TestFolderFullLocation, "test3"));
            SearchTarget lookfor = new SearchTarget();
            lookfor.AttributeMatching1 = FileAttributes.Archive;
            lookfor.AttributeMatching2 = FileAttributes.Hidden;
            lookfor.AttribMatching1Style = SearchTarget.MatchStyleString.MatchAll;
            lookfor.AttribMatching2Style = SearchTarget.MatchStyleString.MatchAll | SearchTarget.MatchStyleString.Invert;
            lookfor.FileNameMatching = SearchTarget.MatchStyleString.Skip;

            Demo.AddSearchAnchor(start);
            Demo.AddSearchTarget(lookfor);

            Demo.Search(testresults);
            Demo.WorkerThreadJoin();

            Assert.IsTrue(testresults.Results.Count == 2);

        }
        [TestCategory("AVD: Finding under controlled scenerios")]
        [TestMethod]
        public void Find_3_folders_no_special_needs__TEST1()
        {
            OdinSearch_OutputConsumerGatherResults testresults = new OdinSearch_OutputConsumerGatherResults();
            Demo.KillSearch();
            Demo.ClearSearchAnchorList();
            Demo.ClearSearchTargetList();

            SearchAnchor start = new SearchAnchor(false);
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
            test2_populate();
            test3_populate();
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

            /* some tests are wanting to poss 'impossible' values such as looking for a file creeated no earlier than 2 seconds from now (test2)
             *  Not Disabling this, means those tests may fail
             */
            Demo.SkipSanityCheck = true;
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

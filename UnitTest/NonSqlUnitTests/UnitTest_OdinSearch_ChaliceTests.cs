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
using static OdinSearchEngine.SearchTarget;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;

namespace NonSqlUnitTests
{
    [TestClass]
    public class UnitTest_OdinSearch_ConsumerClassTests
    {
        [TestMethod]
        public void TestSymbolicLinkConsumer()
        {
            SearchAnchor Start = new SearchAnchor(false);
            SearchTarget Target = new SearchTarget();
            Target.FileName.Add(SearchTarget.MatchAnyFile);
            Start.AddAnchor(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            OdinSearch_SortByExt TestOutput = new OdinSearch_SortByExt();
            TestOutput[OdinSearch_SortByExt.OutputFolderArgument] = "C:\\TestScrubLocation\\SymbolicSearch";
            TestOutput[OdinSearch_SortByExt.CreateSubfoldersOption] = "SURE";

            OdinSearch Demo = new OdinSearch();
            Demo.AddSearchAnchor(Start); Demo.AddSearchTarget(Target);

            Demo.Search(TestOutput);
            Demo.WorkerThreadJoin();


            return;
        }
    }
    /// <summary>
    /// This unit testing puts the OdinSearch class thru a variety of search scenerios to see if it works. 
    /// It 
    /// </summary>
    /// <remarks>This class creates a folder located in C:\\TestScrubLocation that it populates with test scenerioes for the said scenerios at run and deletes it at the cleanup. </remarks>
    [TestClass]
    public class UnitTest_OdinSearch_ChaliceTests
    {
        /// <summary>
        /// This list of files/folders gets set to normal in cleanup and then deleted.
        /// </summary>
         List<string> AttribPurgeList = new List<string>();
        
        // Test creates files/folders there and then cleans up at the end of the run
        string TestFolderFullLocation;
        const string TestScrubFolderName = "TestScrubLocation";
        #region creation and cleanup
        #region init and cleanup
        [TestInitialize]
        public void Init()
        {
            string target = Path.Combine(Path.GetPathRoot(Assembly.GetExecutingAssembly().Location), TestScrubFolderName);

            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }
            TestFolderFullLocation = target;

            //populate_for_tests();
        }



        /// <summary>
        /// we delete the folder we created.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {

            if (Directory.Exists(TestFolderFullLocation))
            {
                // a few tests make readonly files that .NET throws an  exception if we attempt to delete.  Set those in the list to normal and the delete thing should be ok
                foreach (string b in this.AttribPurgeList)
                {
                    File.SetAttributes(b, FileAttributes.Normal);

                }
                Directory.Delete(TestFolderFullLocation, true);
            }
        }
        #endregion
        #region Creation Tools
        /// <summary>
        /// Make a folder
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
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
        #endregion
#endregion

        #region test scenerio population
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

        /// <summary>
        /// This is set with <see cref="test2_populate"/> and is used as the name of the file to look for in <see cref="TEST2_File_recently_created_file_no_older_than_1_day"/>
        /// </summary>
        string test2_filetolookfor_name;
        /// <summary>
        /// Copy the first file we match in C: windows
        /// </summary>
        void test2_populate()
        {
            
            string targ = TestFolderFullLocation;
            //string copysource = Path.Combine(Environment.GetEnvironmentVariable("%WINDIR%"),"\\");
            MakeFolder(targ, "test2");
            test2_filetolookfor_name = Path.Combine(targ, "test2", "bob.tmp");
            MakeFile(string.Empty, test2_filetolookfor_name, FileAttributes.Temporary);

            AttribPurgeList.Add(test2_filetolookfor_name);
        }

        /// <summary>
        /// create the stuff in  the test3 folder to look for.  We create 4 files, set all 4 to <see cref="FileAttributes.Archive"/> but set too of the 4 to also be <see cref="FileAttributes.Hidden"/>. Test looks for files that *only* have the <see cref="FileAttributes.Archive"/> set
        /// </summary>
        void test3_populate()
        {
            string targ = TestFolderFullLocation;
            // make our test folder to look in
            MakeFolder(targ, "test3");
            // should match
            string file1 = Path.Combine(targ, "test3", "nonamematchme.dat");
            MakeFile(string.Empty, file1, FileAttributes.Archive);

            // should match
            string file2 = Path.Combine(targ, "test3", "nonamematchme2.dat");
            MakeFile(string.Empty, file2, FileAttributes.Archive);

            // match should find this file also
            string file5 = Path.Combine(targ, "test3", "nonamematchme_too.dat");
            MakeFile(string.Empty, file5, FileAttributes.Archive | FileAttributes.ReadOnly);
            AttribPurgeList.Add(file5);

            // match should not find this file
            string file3 = Path.Combine(targ, "test3", "nonameDONTmatch.dat");
            MakeFile(string.Empty, file3, FileAttributes.Archive | FileAttributes.Hidden);

            // match should not find this file
            string file4 = Path.Combine(targ, "test3","nonameDONTmatch2.dat");
            MakeFile(string.Empty, file4, FileAttributes.Archive | FileAttributes.Hidden);

            
        }

       

        /// <summary>
        /// Test scenerio ran with <see cref="TEST4_FindFilesMatching_ArchiveAndHidden__EXACTING_flag_set"/>.  We create 4 very similar files.  and look for 1 of the 4 files that match the specified exacting attribute
        /// </summary>
        void test4_populate()
        {
            // make our test folder
            string targ = TestFolderFullLocation;
            MakeFolder(targ, "test4");

            // should not be matched
            string file1 = Path.Combine(targ, "test4", "DontMatchMe.dat");
            MakeFile(string.Empty, file1, FileAttributes.Archive);

            // shouild not be matched
            string file2 = Path.Combine(targ, "test4", "DontMatchMeToo.dat");
            MakeFile(string.Empty, file2, FileAttributes.Archive);

            // this file closley but does NOT match the test
            string file3 = Path.Combine(targ, "test4", "AlsomostMatchButNo.dat");
            MakeFile(string.Empty, file3, FileAttributes.Archive | FileAttributes.Hidden | FileAttributes.ReadOnly);
            /// windows platform .NET complains about deleting read only files.  Files added to this list are set to <see cref="FileAttributes.Normal"/> first before being deleted on cleanup
            AttribPurgeList.Add(file3);


            // the test should only match this file
            string file4 = Path.Combine(targ, "test4", "MatchThis.dat");
            MakeFile(string.Empty, file4, FileAttributes.Archive | FileAttributes.Hidden);

           


        }

        /// <summary>
        /// test5 tests if the code for the DiretoryMatching works.  We create 2 subfolders in the test folder with identicle files. The match will look in both folders BUT should fail the ones in the forbidden path
        /// </summary>
        void test5_populate()
        {
            void populate_files(string root)
            {
                string file1 = Path.Combine(root, "MatchMe.tmp");
                MakeFile(string.Empty, file1, FileAttributes.Normal);
                string file2 = Path.Combine(root, "MatchThisOneToo.tmp");
                MakeFile(string.Empty, file2, FileAttributes.Normal);
                string file3 = Path.Combine(root, "SoYouThingYouCanMatch.tmp");
                MakeFile(string.Empty, file3, FileAttributes.Normal);
                string file4 = Path.Combine(root, "LetsMatch.tmp");
                MakeFile(string.Empty, file4, FileAttributes.Normal);

            }
            string targ = TestFolderFullLocation;
            MakeFolder(targ, "test5");
            string WantedBranch = Path.Combine(targ, "test5", "AllowedPath");
            string ForbiddenBranch = Path.Combine(targ, "test5", "NopePath");
            MakeFolder(string.Empty, WantedBranch);
            MakeFolder(string.Empty, ForbiddenBranch);

            populate_files(WantedBranch);
            populate_files(ForbiddenBranch);
        }

        #endregion

        #region File Creation Date/time tests
        [TestCategory("File Creation Date/time tests")]
        [TestMethod]
        public void TEST2_File_recently_created_file_no_older_than_1_day()
        {
            test2_populate();
            var Demo = new OdinSearch();
            OdinSearch_OutputConsumerGatherResults testresults = new OdinSearch_OutputConsumerGatherResults();
            Demo.KillSearch();
            Demo.ClearSearchTargetList();
            Demo.ClearSearchAnchorList();
/* some tests are wanting to poss 'impossible' values such as looking for a file creeated no earlier than 2 seconds from now (test2)
 *  Not Disabling this, means those tests may fail
 */
            Demo.SkipSanityCheck = true;

            SearchAnchor start = new SearchAnchor(false);
            start.AddAnchor(Path.Combine(TestFolderFullLocation, "test2"));
            SearchTarget lookfor = new SearchTarget();
            lookfor.FileName.Add(Path.GetFileName(test2_filetolookfor_name));
            lookfor.CreationAnchorCheck1 = MatchStyleDateTime.NoEarlierThanThis;
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

            lookfor.CreationAnchorCheck1 = MatchStyleDateTime.Disable;

            

        }
        #endregion

        #region File Attribute tests
        /// <summary>
        /// This match looks for files with archive and hidding attributes set. Test should not find files that don't match that exactly. It tests if the matching good for the exacting flag works
        /// </summary>
        [TestCategory("File Attribute tests")]
        [TestMethod]
        public void TEST4_FindFilesMatching_ArchiveAndHidden__EXACTING_flag_set()
        {
            test4_populate();
            var Demo = new OdinSearch();
            OdinSearch_OutputConsumerGatherResults testresults = new OdinSearch_OutputConsumerGatherResults();
            Demo.KillSearch();
            Demo.ClearSearchAnchorList();
            Demo.ClearSearchTargetList();

            SearchAnchor start = new SearchAnchor(false);
            start.AddAnchor(Path.Combine(TestFolderFullLocation, "test4"));
            SearchTarget lookfor = new SearchTarget();
            lookfor.AttributeMatching1 = FileAttributes.Archive| FileAttributes.Hidden;
            lookfor.AttribMatching1Style = MatchStyleFileAttributes.MatchAll | MatchStyleFileAttributes.Exacting;
            lookfor.AttribMatching2Style = MatchStyleFileAttributes.Skip;


            lookfor.FileNameMatching = MatchStyleString.Skip;

            Demo.AddSearchAnchor(start);
            Demo.AddSearchTarget(lookfor);

            Demo.Search(testresults);
            Demo.WorkerThreadJoin();

            Assert.IsTrue(testresults.Results.Count == 1);

        }
        /// <summary>
        /// Test looks for files in that have the archive flag BUT NOT the Hidding flag.  
        /// </summary>
        [TestCategory("File Attribute tests")]
        [TestMethod]
        public void TEST3_FindFilesMatching_ArchiveButNotHidden__NOEXACTING_flag()
        {
            test3_populate();
            var Demo = new OdinSearch();
            OdinSearch_OutputConsumerGatherResults testresults = new OdinSearch_OutputConsumerGatherResults();
            Demo.KillSearch();
            Demo.ClearSearchAnchorList();
            Demo.ClearSearchTargetList();

            SearchAnchor start = new SearchAnchor(false);
            start.AddAnchor(Path.Combine(TestFolderFullLocation, "test3"));
            SearchTarget lookfor = new SearchTarget();
            lookfor.AttributeMatching1 = FileAttributes.Archive;
            lookfor.AttributeMatching2 = FileAttributes.Hidden;
            lookfor.AttribMatching1Style = MatchStyleFileAttributes.MatchAll;
            lookfor.AttribMatching2Style = MatchStyleFileAttributes.MatchAll | MatchStyleFileAttributes.Invert;
            lookfor.FileNameMatching = MatchStyleString.Skip;

            // fail matches that are in a diretory branch matching this.
            lookfor.DirectoryPath.Add("NopePath");
            lookfor.DirectoryMatching = MatchStyleString.MatchAll | MatchStyleString.Invert;
            Demo.AddSearchAnchor(start);
            Demo.AddSearchTarget(lookfor);

            Demo.Search(testresults);
            Demo.WorkerThreadJoin();

            Assert.IsTrue(testresults.Results.Count == 3);

        }
        #endregion

        #region FileName Tests

        [TestCategory("FileName Tests")]
        [TestMethod]
        public void TEST5_FileMatchingFiles_in_One_Branch_BUT_not_other()
        {
            test5_populate();
            var Demo = new OdinSearch();
            OdinSearch_OutputConsumerGatherResults testresults = new OdinSearch_OutputConsumerGatherResults();
            Demo.KillSearch();
            Demo.ClearSearchAnchorList();
            Demo.ClearSearchTargetList();

            SearchAnchor start = new SearchAnchor(false);
            start.AddAnchor(Path.Combine(TestFolderFullLocation, "test5"));
            SearchTarget lookfor= new SearchTarget();
            lookfor.FileName.Add(SearchTarget.MatchAnyFile); // match any.
            lookfor.DirectoryPath.Add("*NopePath*"); 
            lookfor.DirectoryMatching = MatchStyleString.Invert;

            Demo.AddSearchAnchor(start);
            Demo.AddSearchTarget(lookfor);
            Demo.Search(testresults);
            Demo.WorkerThreadJoin();

            Assert.IsTrue(testresults.Results.Count == 4);


        }

        [TestCategory("FileName Tests")]
        [TestMethod]
        public void TEST1_Find_3_folders_no_special_needs()
        {
            test1_populate();
            var Demo = new OdinSearch();
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
        #endregion
        /*
         * We create/copy stuff into the folder pointed to by TestFolderFullLocaiton based on known stats.
         */
        void populate_for_tests()
        {
            /*test1_populate();
            test2_populate();
            test3_populate();
            test4_populate();
            test5_populate();*/
        }



    }
}

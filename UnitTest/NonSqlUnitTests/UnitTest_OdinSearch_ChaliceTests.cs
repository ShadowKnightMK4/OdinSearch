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
    /// <summary>
    /// folder / file cache to purge things created as needed for the unit tests. Disospoe() or Purge() triggers cleanup
    /// </summary>
    sealed class FileItemCache: IDisposable
    {
        /// <summary>
        /// Any item added to this is set to file attribute normal before deletion.
        /// </summary>
        public List<FileSystemInfo> Items = new();
        public void Purge()
        {
            List<FileSystemInfo> Folders = new();
            List<FileSystemInfo> FileDeletes = new();
            foreach (FileSystemInfo item in Items)
            {
                if (item.Attributes.HasFlag(FileAttributes.Directory) == false)
                {
                    File.SetAttributes(item.FullName, FileAttributes.Normal);
                    FileDeletes.Add(item);
                }
                else
                {
                    Folders.Add(item);
                }
            }

            foreach (FileSystemInfo item in FileDeletes)
            {
                File.Delete(item.FullName);
            }


            foreach (FileSystemInfo item in Folders)
            {
                try
                {
                    Directory.Delete(item.FullName, true);
                }
                catch (DirectoryNotFoundException)
                {
                    // it's probably fine.  Its possible it was just deleted by another delete. The scrub folder location should also be deleted at class cleanup automatically too by unit test classes
                }
            }
        }
        #region Creation Tools
        /// <summary>
        /// Make a folder and add it our  list of cleanup items
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        public void MakeFolder(string path1, string path2)
        {
            MakeFolder(path1, path2, FileAttributes.Directory);
        }

        /// <summary>
        /// Make a folder and add it our  list of cleanup items
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <param name="attributes">set folder's attributes</param>
        public void MakeFolder(string path1, string path2, FileAttributes attributes)
        {
            string targ = Path.Combine(path1, path2);
            Directory.CreateDirectory(targ);

            File.SetAttributes(targ, attributes);
            Items.Add(new DirectoryInfo(targ));
            
        }

        /// <summary>
        /// Make a file and set its attributes. Sets length to 0
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <param name="attributes"></param>
        public void MakeFile(string path1, string path2, FileAttributes attributes)
        {
            MakeFile(path1, path2, attributes, 0);
        }

        /// <summary>
        /// Make a file, set attributes amnd length
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <param name="attributes"></param>
        /// <param name="len"></param>
        public void MakeFile(string path1, string path2, FileAttributes attributes, int len)
        {
            string targ = Path.Combine(path1, path2);
            using (var fn = File.OpenWrite(targ))
            {
                fn.SetLength(len);
            }
            File.SetAttributes(targ, attributes);
            Items.Add(new FileInfo(targ));
        }

        
        /// <summary>
        /// If this is a file or folder, it's deleted at dispoe
        /// </summary>
        /// <param name="Location"></param>
        public void AddItem(string Location)
        {

        }
        void IDisposable.Dispose()
        {
            Purge();
            Items.Clear();
        }
        #endregion
    }
    /// <summary>
    /// The Chalice tests are indented to put the class library OdinSearch thru passes
    /// to ensure it actually supporst the flags, settings, features beyond just can we
    /// set it and does it work.
    /// </summary>
    [TestClass]
    public class UnitTest_OdinSearch_ConsumerClassTests
    {
        [TestMethod]
        public void TestConsoleConsumer()
        {

            SearchAnchor Start = new SearchAnchor(false);
            SearchTarget Target = new SearchTarget();
            Target.FileName.Add(SearchTarget.MatchAnyFileName);
            Start.AddAnchor(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            OdinSearch_OutputSimpleConsole TestOutput = new OdinSearch_OutputSimpleConsole();
            TestOutput[OdinSearch_SymbolicLinkSort.OutputFolderArgument] = "C:\\TestScrubLocation\\SymbolicSearch";
            TestOutput[OdinSearch_SymbolicLinkSort.CreateSubfoldersOption] = "SURE";

            OdinSearch Demo = new OdinSearch();
            Demo.AddSearchAnchor(Start); Demo.AddSearchTarget(Target);

            Demo.Search(TestOutput);
            Demo.WorkerThreadJoin();


            return;
        }
        [TestMethod]
        public void TestSymbolicLinkConsumer()
        {
            SearchAnchor Start = new SearchAnchor(false);
            SearchTarget Target = new SearchTarget();
            Target.FileName.Add(SearchTarget.MatchAnyFileName);
            Start.AddAnchor(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            OdinSearch_SymbolicLinkSort TestOutput = new OdinSearch_SymbolicLinkSort();
            TestOutput[OdinSearch_SymbolicLinkSort.OutputFolderArgument] = "C:\\TestScrubLocation\\SymbolicSearch";
            TestOutput[OdinSearch_SymbolicLinkSort.CreateSubfoldersOption] = "SURE";

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
        
        // Test creates files/folders there and then cleans up at the end of the run. Each test is reposible for making an FileItemCache that clears itself up.
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
                Directory.Delete(TestFolderFullLocation, true);
            }
        }
        #endregion
        
#endregion

        #region test scenerio population
        /// <summary>
        /// Make 3 folders in the scrub folder named folder1-3 with no attributes other than folder.   Wildcard wants to match all 3 
        /// 
        /// </summary>
        void test1_populate(FileItemCache cache, string scrublocation)
        {
            string targ = scrublocation;
            cache.MakeFolder(targ, "test1");
            string folder1 = Path.Combine(targ, "test1\\folder1");
            string folder2 = Path.Combine(targ, "test1\\folder2");
            string folder3 = Path.Combine(targ, "test1\\folder3");

            cache.MakeFolder(folder1, "");
            cache.MakeFolder(folder2, "");
            cache.MakeFolder(folder3, "");
        }

        /// <summary>
        /// This is set with <see cref="test2_populate"/> and is used as the name of the file to look for in <see cref="TEST2_File_recently_created_file_no_older_than_1_day"/>
        /// </summary>
        string test2_filetolookfor_name;
        /// <summary>
        /// Copy the first file we match in C: windows
        /// </summary>
        void test2_populate(FileItemCache cache, string scrublocation)
        {
            
            string targ = scrublocation;
            //string copysource = Path.Combine(Environment.GetEnvironmentVariable("%WINDIR%"),"\\");
            cache.MakeFolder(targ, "test2");
            test2_filetolookfor_name = Path.Combine(targ, "test2", "bob.tmp");
            cache.MakeFile(string.Empty, test2_filetolookfor_name, FileAttributes.Temporary);

            
        }

        /// <summary>
        /// create the stuff in  the test3 folder to look for.  We create 4 files, set all 4 to <see cref="FileAttributes.Archive"/> but set too of the 4 to also be <see cref="FileAttributes.Hidden"/>. Test looks for files that *only* have the <see cref="FileAttributes.Archive"/> set
        /// </summary>
        void test3_populate(FileItemCache cache, string scrublocation)
        {
            string targ = scrublocation;
            // make our test folder to look in
            cache.MakeFolder(targ, "test3");
            // should match
            string file1 = Path.Combine(targ, "test3", "nonamematchme.dat");
            cache.MakeFile(string.Empty, file1, FileAttributes.Archive);

            // should match
            string file2 = Path.Combine(targ, "test3", "nonamematchme2.dat");
            cache.MakeFile(string.Empty, file2, FileAttributes.Archive);

            // match should find this file also
            string file5 = Path.Combine(targ, "test3", "nonamematchme_too.dat");
            cache.MakeFile(string.Empty, file5, FileAttributes.Archive | FileAttributes.ReadOnly);

            // match should not find this file
            string file3 = Path.Combine(targ, "test3", "nonameDONTmatch.dat");
            cache.MakeFile(string.Empty, file3, FileAttributes.Archive | FileAttributes.Hidden);

            // match should not find this file
            string file4 = Path.Combine(targ, "test3","nonameDONTmatch2.dat");
            cache.MakeFile(string.Empty, file4, FileAttributes.Archive | FileAttributes.Hidden);

            
        }

       

        /// <summary>
        /// Test scenerio ran with <see cref="TEST4_FindFilesMatching_ArchiveAndHidden__EXACTING_flag_set"/>.  We create 4 very similar files.  and look for 1 of the 4 files that match the specified exacting attribute
        /// </summary>
        void test4_populate(FileItemCache cache, string scrublocation)
        {
            // make our test folder
            string targ = scrublocation;
            cache.MakeFolder(targ, "test4");

            // should not be matched
            string file1 = Path.Combine(targ, "test4", "DontMatchMe.dat");
            cache.MakeFile(string.Empty, file1, FileAttributes.Archive);

            // shouild not be matched
            string file2 = Path.Combine(targ, "test4", "DontMatchMeToo.dat");
            cache.MakeFile(string.Empty, file2, FileAttributes.Archive);

            // this file closley but does NOT match the test
            string file3 = Path.Combine(targ, "test4", "AlsomostMatchButNo.dat");
            cache.MakeFile(string.Empty, file3, FileAttributes.Archive | FileAttributes.Hidden | FileAttributes.ReadOnly);
            


            // the test should only match this file
            string file4 = Path.Combine(targ, "test4", "MatchThis.dat");
            cache.MakeFile(string.Empty, file4, FileAttributes.Archive | FileAttributes.Hidden);

           


        }

        /// <summary>
        /// test5 tests if the code for the DiretoryMatching works.  We create 2 subfolders in the test folder with identicle files. The match will look in both folders BUT should fail the ones in the forbidden path
        /// </summary>
        void test5_populate(FileItemCache cache, string scrublocation)
        {
            void populate_files(string root)
            {
                string file1 = Path.Combine(root, "MatchMe.tmp");
                cache.MakeFile(string.Empty, file1, FileAttributes.Normal);
                string file2 = Path.Combine(root, "MatchThisOneToo.tmp");
                cache.MakeFile(string.Empty, file2, FileAttributes.Normal);
                string file3 = Path.Combine(root, "SoYouThingYouCanMatch.tmp");
                cache.MakeFile(string.Empty, file3, FileAttributes.Normal);
                string file4 = Path.Combine(root, "LetsMatch.tmp");
                cache.MakeFile(string.Empty, file4, FileAttributes.Normal);

            }
            string targ = scrublocation;
            cache.MakeFolder(targ, "test5");
            string WantedBranch = Path.Combine(targ, "test5", "AllowedPath");
            string ForbiddenBranch = Path.Combine(targ, "test5", "NopePath");
            cache.MakeFolder(string.Empty, WantedBranch);
            cache.MakeFolder(string.Empty, ForbiddenBranch);

            populate_files(WantedBranch);
            populate_files(ForbiddenBranch);
            return;
        }

        #endregion

        #region File Creation Date/time tests
        [TestCategory("File Creation Date/time tests")]
        [TestMethod]
        public void TEST2_File_recently_created_file_no_older_than_1_day()
        {
            using (var handler = new FileItemCache())
            {
                test2_populate(handler, TestFolderFullLocation);
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
            using (var Handler = new FileItemCache())
            {
                test4_populate(Handler, TestFolderFullLocation);
                var Demo = new OdinSearch();
                OdinSearch_OutputConsumerGatherResults testresults = new OdinSearch_OutputConsumerGatherResults();

                SearchAnchor start = new SearchAnchor(false);
                start.AddAnchor(Path.Combine(TestFolderFullLocation, "test4"));
                SearchTarget lookfor = new SearchTarget();
                lookfor.AttributeMatching1 = FileAttributes.Archive | FileAttributes.Hidden;
                lookfor.AttribMatching1Style = MatchStyleFileAttributes.MatchAll | MatchStyleFileAttributes.Exacting;
                lookfor.AttribMatching2Style = MatchStyleFileAttributes.Skip;


                lookfor.FileNameMatching = MatchStyleString.Skip;

                Demo.AddSearchAnchor(start);
                Demo.AddSearchTarget(lookfor);

                Demo.Search(testresults);
                Demo.WorkerThreadJoin();

                Assert.IsTrue(testresults.Results.Count == 1);
            }
        }
        /// <summary>
        /// Test looks for files in that have the archive flag BUT NOT the Hidding flag.  
        /// </summary>
        [TestCategory("File Attribute tests")]
        [TestMethod]
        public void TEST3_FindFilesMatching_ArchiveButNotHidden__NOEXACTING_flag()
        {
            using (var handler = new FileItemCache())
            {
                test3_populate(handler, TestFolderFullLocation);
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
        }
        #endregion

        #region FileName Tests

        [TestMethod]
        public void TestNULL_BackRegEx_OnInput_bad_pattern_SafetyOn()
        {
            TestNULL_BackRegEx_OnInput_bad_pattern_private(true);
        }
        [TestMethod]
        public void TestNULL_BackRegEx_OnInput_bad_pattern_SafetyOff()
        {
            TestNULL_BackRegEx_OnInput_bad_pattern_private(false);
        }
        void TestNULL_BackRegEx_OnInput_bad_pattern_private(bool RegExSafety)
        {
            bool AutoNotify_called = false;
            void err(Thread t, Exception e)
            {
                AutoNotify_called = true;
                Console.WriteLine("Auto Notify called: " + e.Message);
            }
            const string offend = @"[";
            var Demo = new OdinSearch();
            OdinSearch_OutputConsumerGatherResults testresults = new OdinSearch_OutputConsumerGatherResults();
            SearchTarget target = new SearchTarget();
            target.FileName.Add(offend);
            target.RegSaftyMode = false;
            target.FileNameMatching = MatchStyleString.RawRegExMode;
            Demo.AddSearchTarget(target);
            Demo.AddSearchAnchor(new SearchAnchor(false));
            Demo.GetSearchAnchorsAsArray()[0].roots.Add(new DirectoryInfo("C:\\"));

            Demo.Search(testresults, err);
            //var test = Demo.GetWorkerThreadList();

            
            Demo.WorkerThreadJoin();

            //Assert.IsTrue(Demo.WorkerThreadCount == 1);

            var testme = Demo.GetWorkerThreadException();

            if (target.RegSaftyMode)
            {
                Console.WriteLine("RegSaftyMode on: That usings Regex.Escape() to help prevent bad Regex reaching worker threads.");
                Assert.IsTrue(Demo.WorkerThreadCrashed == false);
                Assert.IsTrue(testme.Keys.Count == 0);
                Console.WriteLine("The worker thread didn't crash. Callback error routine not needed");
            }
            else
            {
                Console.WriteLine("RegSaftyMode off: Regex is passed as is to the worker thread from the caller without validation. This should trigger it crashing.");
                Assert.IsTrue(Demo.WorkerThreadCrashed == true);
                Assert.IsTrue(testme.Keys.Count == 1);
                var keyo = testme.Keys.First();
                Assert.IsTrue(testme[keyo].Count == 1);
                Console.WriteLine($"The worker thread crashed. Call back routine called  \"{AutoNotify_called}\"");
            }
            return;
        }

        [TestCategory("FileName Tests")]
        [TestMethod]
        public void TEST5_FileMatchingFiles_in_One_Branch_BUT_not_other()
        {
            using (var handler = new FileItemCache())
            {
                test5_populate(handler, TestFolderFullLocation);
                var Demo = new OdinSearch();
                OdinSearch_OutputConsumerGatherResults testresults = new OdinSearch_OutputConsumerGatherResults();
                Demo.KillSearch();
                Demo.ClearSearchAnchorList(); 
                Demo.ClearSearchTargetList();

                SearchAnchor start = new SearchAnchor(false);
                start.EnumSubFolders = true;
                start.AddAnchor(Path.Combine(TestFolderFullLocation, "test5"));
                SearchTarget lookfor = new SearchTarget();
                lookfor.FileName.Add(SearchTarget.MatchAnyFileName); // match any.
                lookfor.DirectoryPath.Add("*NopePath*");
                lookfor.DirectoryMatching = MatchStyleString.Invert;

                Demo.AddSearchAnchor(start);
                Demo.AddSearchTarget(lookfor);
                Demo.Search(testresults);
                Demo.WorkerThreadJoin();

                Assert.IsTrue(testresults.Results.Count == 5);
            }
            

        }

        [TestCategory("FileName Tests")]
        [TestMethod]
        public void TEST1_Find_3_folders_no_special_needs()
        {
            OdinSearch Demo = new();
            using (var handler = new FileItemCache())
            {
                test1_populate(handler, TestFolderFullLocation);
                OdinSearch_OutputConsumerGatherResults testresults = new OdinSearch_OutputConsumerGatherResults();

                SearchAnchor start = new SearchAnchor(false);
                start.AddAnchor(Path.Combine(TestFolderFullLocation, "test1"));
                SearchTarget lookfor = new SearchTarget();
                lookfor.FileName.Add("folder*");


                Demo.AddSearchAnchor(start);
                Demo.AddSearchTarget(lookfor);
                Demo.Search(testresults);
                Demo.WorkerThreadJoin();

                Assert.IsTrue(testresults.Results.Count == 3);
            }
            
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

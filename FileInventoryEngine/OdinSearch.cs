using OdinSearchEngine.OdinSearch_OutputConsumerTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using static OdinSearchEngine.SearchTarget;
using OdinSearchEngine.OdinSearch_ContainerSystems;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using ThreadState = System.Threading.ThreadState;
using System.ComponentModel.DataAnnotations;

namespace OdinSearchEngine
{

    /// <summary>
    /// Search the local system for files/folders 
    /// </summary>
    public class OdinSearch
    {
        #region DEBUG_AIDS
#if DEBUG
        public bool DebugVerboseMode
        {
             set
            {
                DebugVerboseModeHandle = value;
            }
            get
            {
                return DebugVerboseModeHandle;
            }
        }

        private bool DebugVerboseModeHandle = true;
#else
    
        public bool DebugVerboseMode
        {
            set
            {
                DebugVerboseModeHandle = value;
            }
            get
            {
                return DebugVerboseModeHandle;
            }
        }
        private bool DebugVerboseModeHandle = false;
#endif

        #endregion
        #region Public Class Variables and Properties
        /// <summary>
        /// False Means we don't lock a object to aid synching when sending output to a <see cref="OdinSearch_OutputConsumerBase"/> based class.  
        /// </summary>
        public bool ThreadSynchResults
        {
            get
            {
                return ThreadSynchResultsBacking;
            }
            set
            {
                ThreadSynchResultsBacking = value;
            }
        }

        /// <summary>
        /// Get the current number of worker thread
        /// </summary>
        public int WorkerThreadCount { get { return WorkerThreads.Count; } }

        /// <summary>
        /// When starting a search, if this is true, the call to <see cref="SanityChecks(WorkerThreadArgs)"/> is skipped. Currently, that routine does nothing, but is intended to be a way to guard against silly/impossible things such as searching for a file that's also a folder for example
        /// </summary>
        public bool SkipSanityCheck = true;

        #endregion

        #region Protected or Private Class Variables / properties

        /// <summary>
        /// Backing Varible for <see cref="ThreadSynchResults"/>
        /// </summary>
        protected bool ThreadSynchResultsBacking = true;
        /// <summary>
        /// Used to guard against a thread exception in <see cref="WorkerThreadJoin"/> if it is called begin <see cref="Search(OdinSearch_OutputConsumerBase)"/>. Search sets this to true and <see cref="WorkerThreadJoin"/> will refuse to work if not
        /// </summary>
        bool SearchCalled = false;



        /// <summary>
        /// Locked when sending a match to the output aka one <see cref="OdinSearch_OutputConsumerBase"/> derived class and only if <see cref="ThreadSynchResults"/> is true
        /// </summary>
        readonly object ResultsLock = new object();

        /// <summary>
        /// Add Where to look here. Note that each anchor gets a worker thread.
        /// </summary>
        readonly List<SearchAnchor> Anchors = new List<SearchAnchor>();

        #endregion

        #region Worker Thread Routines
        /// <summary>
        /// Loop thru the worker threads and if any aren't alive, call <see cref="OdinSearch_OutputConsumerBase.ResolvePendingActions"/> on them
        /// </summary>
        public void WorkerThread_ResolveComs()
        {
            foreach (var T in WorkerThreads)
            {
                if (T.Thread.IsAlive == false)
                {
                    T.Args.Coms.ResolvePendingActions();
                }
            }
        }
        #endregion

        #region ExceptionMessages
        /// <summary>
        /// <see cref="InvalidOperationException"/> message when <see cref="WorkerThreadJoin"/> is called with an empty worker thread list.
        /// </summary>
        const string JoinEmptyWorkerThreadListExceptionMessage = "Error: Can't Join Worker Threads. Reason: No Known Threads in list.";
        /// <summary>
        /// <see cref="InvalidOperationException"/> message when <see cref="WorkerThreadJoin"/> is called before calling <see cref="Search(OdinSearch_OutputConsumerBase)"/>. Search sets the <see cref="SearchCalled"/> bool to true when it's called
        /// </summary>
        const string JoinNonEmptyWorkerThreadListBeforeCallingSearch = "Error: Attempt to Join Worker Threads before beginning search.";
        /// <summary>
        /// <see cref="InvalidOperationException"/> message when <see cref="Search(OdinSearch_OutputConsumerBase)"> is called and there are no Anchors in the <see cref="Anchors"/> list
        /// </summary>
        const string NonEmptyAnchorListEmptySplitRoots = "Ensure at list one Anchor in the list has a folder starting point.";
        /// <summary>
        /// <see cref = "InvalidOperationException" /> message when <see cref="Search(OdinSearch_OutputConsumerBase)"/> is called and its call to <see cref="SearchAnchor.SplitRoots"/> retursn an empty list.  
        /// </summary>
        const string EmptyAnchorList = "Specify where to start search.";
        /// <summary>
        /// <see cref = "InvalidOperationException" /> message when <see cref="Search(OdinSearch_OutputConsumerBase)"/> is called while a search is active i.e. when the worker thread list is not empty
        /// </summary>
        const string CantStartNewSearchWhileSearching = "Search in Progress. Workerthread != 0";
        /// <summary>
        /// <see cref = "InvalidOperationException" /> message when <see cref="Search(OdinSearch_OutputConsumerBase)"/> is called when the <see cref="SanityChecks(WorkerThreadArgs)"/> routine returns false.  
        /// </summary>
        const string SanityCheckFailureMessage = "Sanity Check test failed. Search May not work as intended.  To disable Sanity Check set the DisableSanityCheck flag to true";
        #endregion

        #region Dealing with the Search

        /// <summary>
        /// Reset to functionally a new blank instance
        /// </summary>
        public void Reset()
        {
            KillSearch();
            ClearSearchAnchorList();
            ClearSearchTargetList();

            SkipSanityCheck = true;
            ThreadSynchResultsBacking = false;
            SearchCalled = false;
        }


        #endregion

        #region Internal Classes to this class
        /// <summary>
        /// pairing a Thread with a a cancilation token.
        /// </summary>
        internal class WorkerThreadWithCancelToken
        {
            public Thread Thread;
            public CancellationTokenSource Token;
            public WorkerThreadArgs Args;
        }

        /// <summary>
        /// This class is used for storing both a <see cref="SearchTarget"/> with a predone <see cref="Regex"/> list containing the regex to match file
        /// </summary>
        internal class SearchTargetPreDoneRegEx
        {
            public SearchTargetPreDoneRegEx(SearchTarget SearchTarget)
            {
                PreDoneRegExFileName = SearchTarget.ConvertFileNameToRegEx();
                this.SearchTarget = SearchTarget;

                PreDoneRegExDirectoryName = SearchTarget.ConvertDirectoryPathToRegEx();
            }
            public SearchTarget SearchTarget;
            public List<Regex> PreDoneRegExFileName;
            public List<Regex> PreDoneRegExDirectoryName;
        }
        /// <summary>
        /// This class is passed to the worker thread as an argument
        /// </summary>
        internal class WorkerThreadArgs
        {
            /// <summary>
            /// used in the workerthread code. This contains a single start point gotton from a list generated by <see cref="SearchAnchor.SplitRoots()"/>
            /// </summary>
            public SearchAnchor StartFrom;
            /// <summary>
            /// used in the workerthread code. This contains a list of what to look for
            /// </summary>
            public readonly List<SearchTarget> Targets = new();
            /// <summary>
            /// Used to let outside (of the thread anyway) to be able to tell the worker thread to quit. It's checked every file.
            /// </summary>
            public CancellationToken Token;
            /// <summary>
            /// Used in the worker thread.  This is how said thread will send messages and results outside of its world.
            /// </summary>
            public OdinSearch_OutputConsumerBase Coms;

            public Semaphore ComTalk;

            
            //public ReadOnlyCollection<OdinSearchContainer_GenericItem> ContainerList { get; internal set; }
        }
        #endregion

        #region Worker Thread stuff

        #region Worker Thread Routine

        /// <summary>
        /// Unpack the WorkerThreadArgs and go to work. Not intended to to called without having done by its own thread
        /// </summary>
        /// <param name="Args"></param>
        void WorkerThreadProc(object Args)
        {
            

            if (Args == null) throw new ArgumentNullException();
            
            Queue<DirectoryInfo> FolderList = new Queue<DirectoryInfo>();
            //Queue<OdinSearch_ContainerSystemItem> FolderList = new Queue<OdinSearch_ContainerSystemItem>();

            List<SearchTargetPreDoneRegEx> TargetWithRegEx = new List<SearchTargetPreDoneRegEx>();
            WorkerThreadArgs TrueArgs = Args as WorkerThreadArgs;
            //Thread.CurrentThread.Name = TrueArgs.StartFrom.roots[0].ToString() + " Scanner";
#if DEBUG
            if (DebugVerboseModeHandle)
                Debug.WriteLine(Thread.CurrentThread.Name + " is working with " + TrueArgs.StartFrom.roots[0]);
#endif

            if (TrueArgs != null)
            {
                if (TrueArgs.Targets.Count > 0)
                {
                    if (TrueArgs.StartFrom != null)
                    {

                        // prececulate the search target info
                        foreach (SearchTarget Target in TrueArgs.Targets)
                        {
                            TargetWithRegEx.Add(new SearchTargetPreDoneRegEx(Target));
                        }

                        // add root[0] to the queue to pull from
                        FolderList.Enqueue(TrueArgs.StartFrom.roots[0]);

                    // label is used as a starting point to loop back to for looking at subfolders when we get
                    // looping
                    Reset:


                        if (FolderList.Count > 0)
                        {
                            // should an exception happen during getting folder/file names, this is set
                            bool ErrorPrune = false;

                            DirectoryInfo CurrentLoc = FolderList.Dequeue();

                            // files in the CurrentLoc
                            FileInfo[] Files = null;
                            // folders in the CurrentLoc
                            DirectoryInfo[] Folders = null;
                            try
                            {
                                Files = CurrentLoc.GetFiles();
                                Folders = CurrentLoc.GetDirectories();

                            }
                            catch (IOException e)
                            {
                                try
                                {
                                    //LockThisAccess(TrueArgs.ComTalk);
                                    lock (TrueArgs.ComTalk)
                                    {
                                        TrueArgs.Coms.Messaging("Unable to get file or listing for folder at " + CurrentLoc.FullName + " Reason: " + e.Message);
                                        TrueArgs.Coms.Blocked(CurrentLoc.ToString());
                                    }
                                }
                                finally
                                {
                                    //UnlockThisAccess(TrueArgs.ComTalk);
                                }
                                ErrorPrune = true;
                            }
                            catch (UnauthorizedAccessException)
                            {
                                try
                                {
                                    //LockThisAccess(TrueArgs.ComTalk);
                                    lock (TrueArgs.ComTalk)
                                    {
                                        TrueArgs.Coms.Messaging("Unable to get file or listing for folder at " + CurrentLoc.FullName + " Reason Access Denied");
                                        TrueArgs.Coms.Blocked(CurrentLoc.ToString());
                                    }
                                }
                                finally
                                {
                                    //UnlockThisAccess(TrueArgs.ComTalk);
                                }

                                ErrorPrune = true;
                            }



                            if (!ErrorPrune)

                                foreach (SearchTargetPreDoneRegEx Target in TargetWithRegEx)
                                {
                                    bool Pruned = false;

                                    // skip this compare if we're  looking for a directory

                                    if (Target.SearchTarget.AttributeMatching1 != 0)
                                    {
                                        if (Target.SearchTarget.AttributeMatching1.HasFlag(FileAttributes.Directory))
                                        {
                                            Pruned = true;
                                        }
                                    }


                                    if (!Pruned)
                                    {
                                        // file check


                                        foreach (FileInfo Possible in Files)
                                        {
                                            bool isMatched = MatchThis(Target, Possible);
                                            if (isMatched)
                                            {
                                                if (!ThreadSynchResults)
                                                {
                                                    TrueArgs.Coms.Match(Possible);
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        lock (TrueArgs.ComTalk)
                                                        {
                                                            TrueArgs.Coms.Match(Possible);
                                                        }
                                                    }
                                                    finally
                                                    {

                                                    }


                                                }
                                            }
                                        }
                                    }
                                    Pruned = false;

                                    if (!Pruned)
                                    {
                                        // folder check
                                        foreach (DirectoryInfo Possible in Folders)
                                        {

                                            try
                                            {
#if DEBUG
                                                if (DebugVerboseModeHandle)
                                                {
                                                    lock (TrueArgs.ComTalk)
                                                    {
                                                        TrueArgs.Coms.Messaging("DEBUG: attempt to match folder " + Targets[0].FileName.ToString() + " against " + Possible.Name);
                                                    }
                                                }
#endif
                                            }
                                            finally
                                            {

                                            }

                                            bool isMatched = MatchThis(Target, Possible);
                                            if (isMatched)
                                            {
                                                if (!ThreadSynchResults)
                                                {

                                                    try
                                                    {
                                                        lock (TrueArgs.ComTalk)
                                                        {
                                                            TrueArgs.Coms.Match(Possible);
                                                        }
                                                    }
                                                    finally
                                                    {

                                                    }

                                                    
                                                }
                                                else
                                                {
                                                    lock (TrueArgs.ComTalk)
                                                    {
                                                        TrueArgs.Coms.Match(Possible);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            if (TrueArgs.StartFrom.EnumSubFolders)
                            {
                                if (!ErrorPrune)
                                    foreach (DirectoryInfo Folder in Folders)
                                    {
                                        FolderList.Enqueue(Folder);
                                    }
                            }
                        }

                        if (FolderList.Count > 0)
                        {
                            goto Reset;
                        }
                    }
                }
            }
        }
        #endregion
        #region Worker Thread Pending Action Thing

        void WorkerThreadPendingAction_Spawner(WorkerThreadArgs Args)
        {
            Thread SpawnThis = new Thread(p => { WorkerThreadPendingAction(Args); });
            SpawnThis.Name = "Scanner Pending Action Resolved";
            SpawnThis.Start();
        }
        void WorkerThreadPendingAction(WorkerThreadArgs Args)
        {
            bool KeepGoing = true;
            Thread.Sleep(1000);
            if (WorkerThreads.Count > 0)
            {
               while (KeepGoing)
                {
                    KeepGoing = false;
                    for (int step =0; step < WorkerThreads.Count;step++)
                    {
                        if (WorkerThreads[step].Args.Coms.HasPendingActions())
                        {
                            KeepGoing = true;
                            WorkerThreads[step].Args.Coms.ResolvePendingActions();
                        }
                        else
                        {
                            if (WorkerThreads[step].Thread.IsAlive == true)
                            {
                                KeepGoing = true;
                                break;
                            }
                        }
                    }
                }
            }

        }
        #endregion

        #region WatchDog Thread Code

        /// <summary>
        /// Spawn the routine <see cref="WatchdogFireAllDoneWorkerThead(WorkerThreadArgs)"/> in a new thread
        /// </summary>
        /// <param name="Args">Arguments to the worker threads. Coms is used.</param>
        void WatchdogFireAllDoneSpawner(WorkerThreadArgs Args)
        {
            Thread SpawnThis = new Thread(p => { WatchdogFireAllDoneWorkerThead(Args); });
            SpawnThis.Name = "Scanner Watchdog AllDone() Fire";
            SpawnThis.Start();
        }
        /// <summary>
        /// This checks the threads in the list, if all finished, fires off a call to the <see cref="OdinSearch_OutputConsumerBase.AllDone"/> routine before reseting the <see cref="SearchCalled"/> flag and clearing the worker thread list
        /// </summary>
        /// <param name="Args"></param>
        void WatchdogFireAllDoneWorkerThead(WorkerThreadArgs Args)
        {
            //if (Args == null)
              //  throw new ArgumentNullException(nameof(Args));
            //else
            {
                if (WorkerThreads.Count > 0)
                {
                    WorkerThreadJoin();
                    if (WorkerThreads.Count > 0)
                    {
                        if (WorkerThreads[0].Args.Coms.HasPendingActions() == false)
                        {
                            WorkerThreads[0].Args.Coms.AllDone();
                            SearchCalled = false;
                            WorkerThreads.Clear();
                        }

                    }
                }
            }
        }

        #endregion
        /// <summary>
        /// During the search, this contains all worker thread instances we've spone off.
        /// </summary>
        List<WorkerThreadWithCancelToken> WorkerThreads = new List<WorkerThreadWithCancelToken>();



        /// <summary>
        /// Call Thread.Join() for all worker threads spawned in the list. Your code will functionally be awaiting until it is done
        /// </summary>
        /// <exception cref="ThreadStart">Can potentially trigger if a thread has not started yet.</exception>
        /// <exception cref=">"
        /// <remarks></remarks>
        public void WorkerThreadJoin()
        {
            if (WorkerThreads.Count == 0)
            {
                throw new InvalidOperationException(JoinEmptyWorkerThreadListExceptionMessage);
            }
            if (SearchCalled == false)
            {
                throw new InvalidOperationException(JoinNonEmptyWorkerThreadListBeforeCallingSearch);
            }
            // This is here to also guard against premature starting
            Thread.Sleep(200);
            WorkerThreads.ForEach(
                p => {
                    if (p.Thread.ThreadState == System.Threading.ThreadState.Running)
                    {
                        p.Thread.Join();
                    }
                });
        }


        /// <summary>
        /// Zombied State means no more living threads BUT at least one 
        /// <see cref="OdinSearch_OutputConsumerBase"/> communcation class reports pending actions. 
        /// Place a call to <see cref="OdinSearch.WorkerThread_ResolveComs"/> to call the
        /// <see cref="OdinSearch_OutputConsumerBase.ResolvePendingActions"/> routine for each worker thread
        /// instance of the class
        /// </summary>
        /// <remarks>If your <see cref="OdinSearch_OutputConsumerBase"/> does not do anything beyond
        /// the default <see cref="OdinSearch_OutputConsumerBase.HasPendingActions"/> where it always returns 
        /// false, this property should be false</remarks>
        public bool IsZombied
        {
            get
            {
                if (WorkerThreads.Count == 0)
                    return false;

                int running_count = 0;
                for (int step = 0; step < WorkerThreads.Count; step++)
                {
                    {
                        if (!WorkerThreads[step].Thread.IsAlive)
                        {
                            running_count++;
                            break;
                        }
                    }
                }
                return running_count > 0;
            }
        }

        /// <summary>
        /// Has active search threads that are alived or the <see cref="OdinSearch_OutputConsumerBase"/> coms class reports it has pending actions.
        /// </summary>
        public bool HasActiveSearchThreads
        {
            get
            {
                if (WorkerThreads.Count == 0)
                    return false;

                int running_count = 0;
                for (int step = 0; step < WorkerThreads.Count; step++)
                {
                    {
                        if (WorkerThreads[step].Thread.ThreadState == (ThreadState.Running))
                        {
                            running_count++;
                            break;
                        }
                        else
                        {
                            if (WorkerThreads[step].Args.Coms.HasPendingActions())
                            {
                                running_count++;
                                break;
                            }
                        }
                    }
                }
                return running_count > 0;
            }
        }



        #endregion

        #region MatchThis Routines
        /// <summary>
        /// Compares if the specified thing to look for matches this possible file system item
        /// </summary>
        /// <param name="SearchTarget">A class containing both the <see cref="SearchTarget"/> and the predone <see cref="Regex"/> stuff</param>
        /// <param name="Info">look for this</param>
        /// <returns>true if matchs and false if not.</returns>
        bool MatchThis(SearchTargetPreDoneRegEx SearchTarget, FileSystemInfo Info)
        {
            bool FinalMatch = true;
            bool MatchedOne = false;
            bool MatchedFailedOne = false;

            bool StringCheck(SearchTarget.MatchStyleString HowToCompare, List<Regex> TestValues, string TestAgainst, out bool MatchAll, out bool MatchAny)
            {
                uint MatchCount = 0;
                bool CompareMe = false;
                MatchAll = false;
                MatchAny = false;
                if ((MatchStyleString.MatchAll | MatchStyleString.MatchAny | HowToCompare) == 0)
                {
                    HowToCompare |= MatchStyleString.MatchAll;
                }
                // a plan regex list for this code means we sucessfully match any file. Also skip.
                if ((HowToCompare == MatchStyleString.Skip) || (TestValues.Count == 0))
                {
                    MatchAll = MatchAny = true;
                    return true;
                }
                else
                {
                    foreach (Regex comparethis in TestValues)
                    {
                        if (comparethis.IsMatch(TestAgainst))
                        {
                            MatchCount++;
                            MatchAny = true;
                        }
                    }

                    if (HowToCompare.HasFlag(MatchStyleString.MatchAll))
                    {
                        if (MatchCount > 0)
                            MatchAny = true;
                        if (MatchCount != TestValues.Count)
                        {
                            MatchAll = false;
                        }
                        else
                        {
                            MatchAll = true;
                        }
                    }
                    if (HowToCompare.HasFlag(MatchStyleString.Invert))
                    {
                        MatchAny = (MatchAny != true);
                        MatchAll = (MatchAll != true);
                    }

                    if (HowToCompare.HasFlag(MatchStyleString.MatchAll))
                    {
                        return MatchAll;
                    }
                    if (HowToCompare.HasFlag(MatchStyleString.MatchAny))
                    {
                        return MatchAny;
                    }
                    return false;
                }
            }
            bool DateCheck(SearchTarget.MatchStyleDateTime HowToCompare, DateTime SearchTargetCompare, DateTime FileInfoCompare)
            {
                if (HowToCompare != OdinSearchEngine.SearchTarget.MatchStyleDateTime.Disable)
                {
                    switch (HowToCompare)
                    {
                        case OdinSearchEngine.SearchTarget.MatchStyleDateTime.NoEarlierThanThis:
                            {
                                if (Info.CreationTime.CompareTo(SearchTarget.SearchTarget.CreationAnchor) < 0)
                                {
                                    //FinalMatch = false;// goto exit
                                    return false;
                                }
                                break;
                            }
                        case OdinSearchEngine.SearchTarget.MatchStyleDateTime.NoLaterThanThis:
                            {
                                if (Info.CreationTime.CompareTo(DateTime.MinValue) < 0)
                                {
                                    //FinalMatch = false;// goto exit;
                                    return false;
                                }
                                break;
                            }
                        case OdinSearchEngine.SearchTarget.MatchStyleDateTime.Disable:
                        default:
                            break;
                    }
                }
                return true;
            }
            bool AttribCheck(SearchTarget.MatchStyleFileAttributes HowToCompare, FileAttributes SearchTargetCompare, FileAttributes FileInfoCompare)
            {
                bool CompareMe = false;

                // treat check if true if no attributeres were specified or normal was
                if ((HowToCompare == MatchStyleFileAttributes.Skip) || ((SearchTargetCompare == FileAttributes.Normal) || (SearchTargetCompare == 0)))
                {
                    return true;
                }
                else
                {
                    if (HowToCompare.HasFlag(MatchStyleFileAttributes.MatchAll))
                    {
                        if (HowToCompare.HasFlag(MatchStyleFileAttributes.Exacting))
                        {
                            if (SearchTargetCompare == FileInfoCompare)
                            {
                                CompareMe = true;
                            }
                        }
                        else
                        {
                            if ((SearchTargetCompare & FileInfoCompare) == (SearchTargetCompare))
                            {
                                CompareMe = true;
                            }
                        }
                    }
                    else
                    {
                        if (HowToCompare.HasFlag(MatchStyleFileAttributes.MatchAny))
                        {
                            if ((SearchTargetCompare & FileInfoCompare) != 0)
                            {
                                CompareMe = true;
                            }
                        }
                    }


                    if (HowToCompare.HasFlag(MatchStyleFileAttributes.Invert))
                    {
                        CompareMe = (CompareMe != true);
                    }
                    return CompareMe;
                }
            }
            // ensure both the SearchTarget and what we're comparing against are not null
            if (SearchTarget == null)
            {
                throw new ArgumentNullException(nameof(SearchTarget));
            }
            if (Info == null)
            {
                throw new ArgumentNullException(nameof(Info));
            }

            if ((SearchTarget.SearchTarget.AttributeMatching1 != FileAttributes.Normal) && (SearchTarget.SearchTarget.AttributeMatching1 != 0))
            {
                bool result = AttribCheck(SearchTarget.SearchTarget.AttribMatching1Style, SearchTarget.SearchTarget.AttributeMatching1, Info.Attributes);
                if (!result)
                {
                    FinalMatch = false;
                    goto exit;
                }
            }

            if ((SearchTarget.SearchTarget.AttributeMatching2 != FileAttributes.Normal) && (SearchTarget.SearchTarget.AttributeMatching2 != 0))
            {
                bool result = AttribCheck(SearchTarget.SearchTarget.AttribMatching2Style, SearchTarget.SearchTarget.AttributeMatching2, Info.Attributes);
                if (!result)
                {
                    FinalMatch = false;
                    goto exit;
                }
            }


            // if the filename check has not been disabled
            if (!SearchTarget.SearchTarget.FileNameMatching.HasFlag(MatchStyleString.Skip))
            {
                bool MatchAny, MatchAll;
                bool result = StringCheck(SearchTarget.SearchTarget.FileNameMatching, SearchTarget.PreDoneRegExFileName, Info.Name, out MatchAny, out MatchAll);
                if (!result)
                {
                    FinalMatch = false;
                    goto exit;
                }

                // This is a pecial case in <the Convert to RegEx routine.
                // Empty list means we're functionally matching ALL file name combinations
                /*
                if (SearchTarget.PreDoneRegEx.Count == 0)
                {
                    MatchedOne = true;
                }
                else
                {
                    foreach (var Regs in SearchTarget.PreDoneRegEx)
                    {
                        if (Regs.IsMatch(Info.Name))
                        {
                            MatchedOne = true;
                        }
                        else
                        {
                            MatchedFailedOne = true;
                        }
                    }
                }*/
                /*
                 * MatchOne & MatchFailOne true means we're not a match all
                 * 
                 * MatchOne true and MatchFailOne false means at least one matched but not all
                 * 
                 * MatchOne false and MatchFail false means nothing matched
                 * */
                /*
                if (SearchTarget.SearchTarget.FileNameMatching.HasFlag(OdinSearchEngine.SearchTarget.MatchStyleString.MatchAll))
                {
                    if (SearchTarget.SearchTarget.FileNameMatching.HasFlag(OdinSearchEngine.SearchTarget.MatchStyleString.Invert))
                    {
                        if (MatchedOne)
                        {
                            FinalMatch = false;
                            goto exit;
                        }
                    }
                    else
                    {
                        if (MatchedFailedOne)
                        {
                            FinalMatch = false;
                            goto exit;
                        }
                    }
                }

                if (SearchTarget.SearchTarget.FileNameMatching.HasFlag(OdinSearchEngine.SearchTarget.MatchStyleString.MatchAny))
                {
                    if (SearchTarget.SearchTarget.FileNameMatching.HasFlag(OdinSearchEngine.SearchTarget.MatchStyleString.Invert))
                    {
                        if (!MatchedFailedOne)
                        {
                            FinalMatch = false;
                            goto exit;
                        }
                    }
                    else
                    {
                        if (!MatchedOne)
                        {
                            FinalMatch = false;
                            goto exit;
                        }
                    }
                }*/
            }


            // if the pareny check has not been diabled
            if (!SearchTarget.SearchTarget.DirectoryMatching.HasFlag(MatchStyleString.Skip))
            {
                bool MatchAny, MatchAll;
                bool result = StringCheck(SearchTarget.SearchTarget.DirectoryMatching, SearchTarget.PreDoneRegExDirectoryName, Info.FullName, out MatchAny, out MatchAll);
                if (!result)
                {
                    FinalMatch = false;
                    goto exit;
                }
            }
            MatchedOne = false;
            MatchedFailedOne = false;



            if (SearchTarget.SearchTarget.AccessAnchorCheck1 != MatchStyleDateTime.Disable)
            {
                bool result = DateCheck(SearchTarget.SearchTarget.AccessAnchorCheck1, SearchTarget.SearchTarget.AccessAnchor, Info.LastAccessTime);
                if (!result)
                {
                    FinalMatch = false;
                    goto exit;
                }
            }

            if (SearchTarget.SearchTarget.AccessAnchorCheck2 != MatchStyleDateTime.Disable)
            {
                bool result = DateCheck(SearchTarget.SearchTarget.AccessAnchorCheck2, SearchTarget.SearchTarget.AccessAnchor, Info.LastAccessTime);
                if (!result)
                {
                    FinalMatch = false;
                    goto exit;
                }
            }

            if (SearchTarget.SearchTarget.WriteAnchorCheck1 != MatchStyleDateTime.Disable)
            {
                bool result = DateCheck(SearchTarget.SearchTarget.WriteAnchorCheck1, SearchTarget.SearchTarget.CreationAnchor, Info.LastWriteTime);
                if (!result)
                {
                    FinalMatch = false;
                    goto exit;
                }
            }

            if (SearchTarget.SearchTarget.WriteAnchorCheck2 != MatchStyleDateTime.Disable)
            {
                bool result = DateCheck(SearchTarget.SearchTarget.WriteAnchorCheck2, SearchTarget.SearchTarget.CreationAnchor, Info.LastWriteTime);
                if (!result)
                {
                    FinalMatch = false;
                    goto exit;
                }
            }

            if (SearchTarget.SearchTarget.CreationAnchorCheck1 != MatchStyleDateTime.Disable)
            {
                bool result = DateCheck(SearchTarget.SearchTarget.CreationAnchorCheck1, SearchTarget.SearchTarget.CreationAnchor, Info.CreationTime);
                if (!result)
                {
                    FinalMatch = false;
                    goto exit;
                }
            }

            if (SearchTarget.SearchTarget.CreationAnchorCheck2 != MatchStyleDateTime.Disable)
            {
                bool result = DateCheck(SearchTarget.SearchTarget.CreationAnchorCheck2, SearchTarget.SearchTarget.CreationAnchor, Info.CreationTime);
                if (!result)
                {
                    FinalMatch = false;
                    goto exit;
                }
            }


            /*
            if (SearchTarget.SearchTarget.CreationAnchorCheck != OdinSearchEngine.SearchTarget.DateTimeMatching.Disable)
            {
                switch (SearchTarget.SearchTarget.CreationAnchorCheck)
                {
                    case OdinSearchEngine.SearchTarget.DateTimeMatching.NoEarlierThanThis:
                        {
                            if (Info.CreationTime.CompareTo(SearchTarget.SearchTarget.CreationAnchor) < 0) 
                            { 
                                FinalMatch = false; goto exit;
                            }
                            break;
                        }
                    case OdinSearchEngine.SearchTarget.DateTimeMatching.NoLaterThanThis:
                        {
                            if (Info.CreationTime.CompareTo(DateTime.MinValue) < 0)
                            {
                                FinalMatch = false; goto exit;
                            }
                            break;
                        }
                    case OdinSearchEngine.SearchTarget.DateTimeMatching.Disable:
                    default:
                        break;
                }
            }
            */

            MatchedOne = false;
            MatchedFailedOne = false;
        exit:
            return FinalMatch;
        }

        /// <summary>
        /// Compares if the specified thing to look for matches this possible file system item
        /// </summary>
        /// <param name="SearchTarget">A class containing both the <see cref="SearchTarget"/> and the predone <see cref="Regex"/> stuff</param>
        /// <param name="Info">look for this</param>
        /// <returns>true if matchs and false if not.</returns>
        bool MatchThis(SearchTargetPreDoneRegEx SearchTarget, FileInfo Info)
        {
            return MatchThis(SearchTarget, Info as FileSystemInfo);
        }
        /// <summary>
        /// Compares if the specified thing to look for matches this possible file system item
        /// </summary>
        /// <param name="SearchTarget">A class containing both the <see cref="SearchTarget"/> and the predone <see cref="Regex"/> stuff</param>
        /// <param name="Info">look for this</param>
        /// <returns>true if matchs and false if not.</returns>
        bool MatchThis(SearchTargetPreDoneRegEx SearchTarget, DirectoryInfo Info)
        {
            return MatchThis(SearchTarget, Info as FileSystemInfo);
        }

        #endregion

        #region Code for dealing with setting targets
        /// <summary>
        /// Add what to look for here.
        /// </summary>
        readonly List<SearchTarget> Targets = new List<SearchTarget>();

        /// <summary>
        /// Add a new thing to look for
        /// </summary>
        /// <param name="target"></param>
        public void AddSearchTarget(SearchTarget target)
        {
            Targets.Add(target);
        }

        /// <summary>
        /// Clear the Search target list
        /// </summary>
        public void ClearSearchTargetList()
        {
            Targets.Clear();
        }
        /// <summary>
        /// Fetch the current list of SearchTargets as an array.
        /// </summary>
        /// <returns>returns copy of the SearchTarget list as an array</returns>
        public SearchTarget[] GetSearchTargetsAsArray()
        {
            return Targets.ToArray();
        }


        /// <summary>
        /// Return a read only copy of the SearchTargetList
        /// </summary>
        /// <returns>Returns the Search Target list in ready only form</returns>
        public ReadOnlyCollection<SearchTarget> GetSearchTargetsReadOnly()
        {
            return Targets.AsReadOnly();
        }
        #endregion
        #region Code for dealing with setting anchors
        

        /// <summary>
        /// Add a new SearchAnchor
        /// </summary>
        /// <param name="Anchor">the new starting point to begin looking for files</param>
        public void AddSearchAnchor(SearchAnchor Anchor)
        {
            Anchors.Add(Anchor);
        }

        /// <summary>
        /// Clear the SearchAnchor List
        /// </summary>
        public void ClearSearchAnchorList()
        {
            Anchors.Clear();
        }


        /// <summary>
        /// Fetch the current list of SearchAnchors as an array.
        /// </summary>
        /// <returns>returns copy of the SearchAnchors list as an array</returns>
        public SearchAnchor[] GetSearchAnchorsAsArray()
        {
            return Anchors.ToArray();
        }

        /// <summary>
        /// Get SearchAnchor list
        /// </summary>
        /// <returns>Returns the Anchor list in ready only form</returns>
        public ReadOnlyCollection<SearchAnchor> GetSearchAnchorReadOnly()
        {
            return Anchors.AsReadOnly();
        }

        #endregion

        #region Search Dealings
        #region Search Starting

        /// <summary>
        /// Start the search rolling. 
        /// </summary>
        /// <param name="Coms">This class is how the search communicates with your code. Cannot be null</param>
        /// <exception cref="IOException">Is thrown if Search is called while searching. </exception>
        /// <exception cref="ArgumentNullException">Is thrown if the Coms argument is null</exception>
        /// <remarks>Note that the search uses a default class <see cref="OdinSearch_ContainerFileInfo"/> if there's not container class</remarks>
        public void Search(OdinSearch_OutputConsumerBase Coms)
        {
            if (Anchors.Count <= 0)
            {
                throw new InvalidOperationException(EmptyAnchorList);
            }
            if (Coms == null)
            {
                throw new ArgumentNullException(nameof(Coms));
            }
            else
            {
                if (WorkerThreads.Count != 0)
                {
                    throw new InvalidOperationException(CantStartNewSearchWhileSearching);
                }
                else
                {
                    Semaphore LockThis = new(0, 1);
                    WorkerThreadArgs Args = null;
                    foreach (SearchAnchor Anchor in Anchors)
                    {
                        var AnchorList = Anchor.SplitRoots();
                        if (AnchorList.Length == 0)
                        {
                            throw new InvalidOperationException(NonEmptyAnchorListEmptySplitRoots);
                        }
                        for (int smallstep = 0; smallstep < AnchorList.Length; smallstep++)
                        //foreach (SearchAnchor SmallAnchor in AnchorList)
                        {
                            Args = new();
                            Args.StartFrom = AnchorList[smallstep];
                            Args.Targets.AddRange(Targets);
                            Args.Coms = Coms;
                            //Args.ContainerList = null;

                            if (!SkipSanityCheck)
                            {
                                // TODO: SanityCheck is there to catch theority inpossible matching.
                                if (!SanityChecks(Args))
                                {
                                    throw new InvalidOperationException(SanityCheckFailureMessage);
                                }
                            }

                            WorkerThreadWithCancelToken Worker = new WorkerThreadWithCancelToken();
                            //Worker.Thread = new Thread(() => WorkerThreadProc(Args));
                            Worker.Thread = new Thread(WorkerThreadProc);
                            Worker.Thread.Name = AnchorList[smallstep].roots[0].ToString();
                            Worker.Token = new CancellationTokenSource();
                            Worker.Args = Args;
                            Args.Token = Worker.Token.Token;
                            Args.ComTalk = LockThis;

                            WorkerThreads.Add(Worker);
                            Args = null;
                        }
                    }


                    // we loop thru and call search begin for each thread.
                    // if it returns true, we prematurly quit looping.
                    bool KeepCallingForThreads = true;
                    foreach (WorkerThreadWithCancelToken t in WorkerThreads)
                    {
                        if (KeepCallingForThreads)
                        {
                            KeepCallingForThreads = Coms.SearchBegin(DateTime.Now);
                        }
                        t.Thread.Start(t.Args);
                    }
                    SearchCalled = true;
                    WatchdogFireAllDoneSpawner(null);
                    WorkerThreadPendingAction_Spawner(null);
                }
            }
        }
        #endregion

        /// <summary>
        /// Politely ask the worker threads to end and remove them from our list
        /// </summary>
        public void KillSearch()
        {
            if (WorkerThreads.Count != 0)
            {
                foreach (var workerThread in WorkerThreads)
                {
                    workerThread.Token.Cancel();
                    
                }
            }

            WorkerThreads.Clear();
        }

        /// <summary>
        /// Search specs must pass this before search is go. We are looking to just fail impossible combinations
        /// </summary>
        /// <param name="Arg"></param>
        /// <returns></returns>
        /// <remarks>Honstestly just returns true with this current build.</remarks>
        bool SanityChecks(WorkerThreadArgs Arg)
        {
            // TODO:  Ensure conflicting filename and DirectoryName can actually match. For example, we're not attempting to compare contrarray
            //  settings in the filename array and directory path
            // TODO: Ensure we can have allowable file attributes. For example we're not wanting something that's botha file and a file.
            System.Diagnostics.Debug.Write("Add code SanityCheck() routine");
            return true;
        }

        

        
        #endregion
        #region Container Handling
        /// <summary>
        /// When called, your routine should do what it needs to do to see if there's a class to handle this location.
        /// </summary>
        /// <param name="location">Someone in a file system. May be nested as a container like C:\\Something.zip\\Somethinginside.txt</param>
        /// <returns>If your callback knows the current handler for this location, return the typeof(classname). If not, return null</returns>
        public delegate Type ContainerCheckFileCallback(string location);
        /// <summary>
        /// When called, your routine should do what it needs to do to see if there's a class to handle this location.
        /// </summary>
        /// <param name="location">Someone in a file system. May be nested as a container like C:\\Something.zip\\Somethinginside.txt</param>
        /// <returns>If your callback knows the current handler for this location, return the typeof(classname). If not, return null</returns>
        public delegate Type ContainerCheckDirectoryCallback(string location);

        readonly List<ContainerCheckDirectoryCallback> DirectoryContainerList = new();
        readonly List<ContainerCheckFileCallback> FileContainerList = new();

        public void AddFileContainerCallback(ContainerCheckDirectoryCallback ContainerCheckDirectoryCallback)
        {
            DirectoryContainerList.Add(ContainerCheckDirectoryCallback);
        }
        public void AddDirectoryContainerCallback(ContainerCheckFileCallback ContainerCheckFileCallback)
        {
            FileContainerList.Add(ContainerCheckFileCallback);
        }

        public void ClearFileContainerCallback()
        {
            FileContainerList.Clear();
        }

        public void ClearDirectoryContainerCallback()
        {
            DirectoryContainerList.Clear();
        }

        
        #endregion

        
    }
}

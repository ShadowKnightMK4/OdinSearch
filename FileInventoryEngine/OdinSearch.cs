﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using OdinSearchEngine.OdinSearch_OutputConsumerTools;
using System.Runtime.Intrinsics.X86;

namespace OdinSearchEngine
{

    /// <summary>
    /// Search the local system for files/folders 
    /// </summary>
    public class OdinSearch
    {
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
        #endregion
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
                PreDoneRegEx = SearchTarget.ConvertFileNameToRegEx();
                this.SearchTarget = SearchTarget;
            }
            public SearchTarget SearchTarget;
            public List<Regex> PreDoneRegEx;
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
        }


        /// <summary>
        /// Used to guard against a thread exception in <see cref="WorkerThreadJoin"/> if it is called begin <see cref="Search(OdinSearch_OutputConsumerBase)"/>. Search sets this to true and <see cref="WorkerThreadJoin"/> will refuse to work if not
        /// </summary>
        bool SearchCalled = false;

        /// <summary>
        /// Locked when sending a match to the output aka one <see cref="OdinSearch_OutputConsumerBase"/> derived class and only if <see cref="ThreadSynchResults"/> is true
        /// </summary>
        readonly object ResultsLock = new object();

        #region Worker Thread stuff
        /// <summary>
        /// During the search, this contains all worker thread instances we've spone off.
        /// </summary>
        List<WorkerThreadWithCancelToken> WorkerThreads = new List<WorkerThreadWithCancelToken>();


        /// <summary>
        /// Get the current number of worker thread
        /// </summary>
        public int WorkerThreadCount { get { return WorkerThreads.Count; } }

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
                    if (p.Thread.ThreadState == ThreadState.Running)
                    {
                        p.Thread.Join();
                    }
                });
        }
        /// <summary>
        /// get if any of the worker threads are alive and running still.
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
                    if (WorkerThreads[step].Thread.ThreadState == (ThreadState.Running))
                    {
                        running_count++;
                        break;
                    }
                }
                return running_count > 0;
            }
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
        /// Add Where to look here. Note that each anchor gets a worker thread.
        /// </summary>
        readonly List<SearchAnchor> Anchors = new List<SearchAnchor>();

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
        /// Add a new SearchAnchor
        /// </summary>
        /// <returns>Returns the Anchor list in ready only form</returns>
        public ReadOnlyCollection<SearchAnchor> GetSearchAnchorReadOnly()
        {
            return Anchors.AsReadOnly();
        }

        #endregion

        #region Code with Dealing with threads

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
        protected bool ThreadSynchResultsBacking = true;

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
        /// <remarks>Honstestly just returns true with this current build</remarks>
        bool SanityChecks(WorkerThreadArgs Arg)
        {
            return true;
        }

        /// <summary>
        /// If True, SanityCheck that looks for impossible combinations must pass before starting the search
        /// </summary>
        public bool SkipSanityCheck = true;
        /// <summary>
        /// Compares if the specified thing to look for matches this possible file system item
        /// </summary>
        /// <param name="SearchTarget">A class containing both the <see cref="SearchTarget"/> and the predone <see cref="Regex"/> stuff</param>
        /// <param name="Info">look for this</param>
        /// <returns>true if matchs and false if not.</returns>
        bool MatchThis(SearchTargetPreDoneRegEx SearchTarget, FileSystemInfo Info)
        {
            bool FinalMatch= true;
            bool MatchedOne = false;
            bool MatchedFailedOne = false;

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

            // ensure both the SearchTarget and what we're comparing against are not null
            if (SearchTarget == null)
            {
                throw new ArgumentNullException(nameof(SearchTarget));
            }
            if (Info == null)
            {
                throw new ArgumentNullException(nameof(Info));
            }


            // if the filename check has not been disabled
            if (!SearchTarget.SearchTarget.FileNameMatching.HasFlag(OdinSearchEngine.SearchTarget.MatchStyleString.Skip))
            {
                /* This is a pecial case in <the Convert to RegEx routine.
                 * Empty list means we're functionally matching ALL file name combinations
                 */
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
                }
                /*
                 * MatchOne & MatchFailOne true means we're not a match all
                 * 
                 * MatchOne true and MatchFailOne false means at least one matched but not all
                 * 
                 * MatchOne false and MatchFail false means nothing matched
                 * */
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
                }
            }

            MatchedOne = false;
            MatchedFailedOne = false;
            if (!SearchTarget.SearchTarget.AttribMatching1Style.HasFlag(OdinSearchEngine.SearchTarget.MatchStyleString.Skip))
            {
                if ((SearchTarget.SearchTarget.AttributeMatching1.HasFlag(FileAttributes.Normal) != true) && 
                   (SearchTarget.SearchTarget.AttributeMatching1 != 0))
                {
                    if (SearchTarget.SearchTarget.AttribMatching1Style.HasFlag(OdinSearchEngine.SearchTarget.MatchStyleString.MatchAll))
                    {
                        if (SearchTarget.SearchTarget.AttributeMatching1 == Info.Attributes)
                        {
                            if ((SearchTarget.SearchTarget.AttribMatching1Style.HasFlag(OdinSearchEngine.SearchTarget.MatchStyleString.Invert)))
                            {
                                FinalMatch = false;
                                goto exit;
                            }
                        }
                    }

                    if (SearchTarget.SearchTarget.AttribMatching1Style.HasFlag(OdinSearchEngine.SearchTarget.MatchStyleString.MatchAny))
                    {
                        if ( (SearchTarget.SearchTarget.AttributeMatching1 & Info.Attributes) == 0)
                        {
                            FinalMatch = false;
                            goto exit;
                        }
                    }

                }
            }

            
            if (SearchTarget.SearchTarget.AccessAnchorCheck1 != OdinSearchEngine.SearchTarget.MatchStyleDateTime.Disable)
            {
                bool result = DateCheck(SearchTarget.SearchTarget.AccessAnchorCheck1, SearchTarget.SearchTarget.AccessAnchor, Info.LastAccessTime);
                if (!result)
                {
                    FinalMatch = false;
                    goto exit;
                }
            }

            if (SearchTarget.SearchTarget.AccessAnchorCheck2 != OdinSearchEngine.SearchTarget.MatchStyleDateTime.Disable)
            {
                bool result = DateCheck(SearchTarget.SearchTarget.AccessAnchorCheck2, SearchTarget.SearchTarget.AccessAnchor, Info.LastAccessTime);
                if (!result)
                {
                    FinalMatch = false;
                    goto exit;
                }
            }

            if (SearchTarget.SearchTarget.WriteAnchorCheck1 != OdinSearchEngine.SearchTarget.MatchStyleDateTime.Disable)
            {
                bool result = DateCheck(SearchTarget.SearchTarget.WriteAnchorCheck1, SearchTarget.SearchTarget.CreationAnchor, Info.LastWriteTime);
                if (!result)
                {
                    FinalMatch = false;
                    goto exit;
                }
            }

            if (SearchTarget.SearchTarget.WriteAnchorCheck2 != OdinSearchEngine.SearchTarget.MatchStyleDateTime.Disable)
            {
                bool result = DateCheck(SearchTarget.SearchTarget.WriteAnchorCheck2, SearchTarget.SearchTarget.CreationAnchor, Info.LastWriteTime);
                if (!result)
                {
                    FinalMatch = false;
                    goto exit;
                }
            }

            if (SearchTarget.SearchTarget.CreationAnchorCheck1 != OdinSearchEngine.SearchTarget.MatchStyleDateTime.Disable)
            {
                bool result = DateCheck(SearchTarget.SearchTarget.CreationAnchorCheck1, SearchTarget.SearchTarget.CreationAnchor, Info.CreationTime);
                if (!result)
                {
                    FinalMatch = false;
                    goto exit;
                }
            }

            if (SearchTarget.SearchTarget.CreationAnchorCheck2 != OdinSearchEngine.SearchTarget.MatchStyleDateTime.Disable)
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
            if (Args == null)
                throw new ArgumentNullException(nameof(Args));
            else
            {
                if (WorkerThreads.Count > 0)
                {
                    WorkerThreadJoin();
                    if (WorkerThreads.Count > 0)
                    {
                        WorkerThreads[0].Args.Coms.AllDone();
                    }
                    SearchCalled = false;
                    WorkerThreads.Clear();
                }
            }
        }
        /// <summary>
        /// Unpack the WorkerThreadArgs and go to work. Not intended to to called without having done by its own thread
        /// </summary>
        /// <param name="Args"></param>
        void ThreadWorkerProc(object Args)
        {
            Queue<DirectoryInfo> FolderList= new Queue<DirectoryInfo>();
            List<SearchTargetPreDoneRegEx> TargetWithRegEx = new List<SearchTargetPreDoneRegEx>();
            WorkerThreadArgs TrueArgs = Args as WorkerThreadArgs;
            Thread.CurrentThread.Name = TrueArgs.StartFrom.roots[0].ToString() + " Scanner";
            
            
            
           if (TrueArgs != null ) 
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
                                TrueArgs.Coms.Messaging("Unable to get file or listing for folder at " + CurrentLoc.FullName + " Reason: " + e.Message);
                                TrueArgs.Coms.Blocked(CurrentLoc.ToString());
                                ErrorPrune = true;
                            }
                            catch (UnauthorizedAccessException)
                            {
                                TrueArgs.Coms.Messaging("Unable to get file or listing for folder at " + CurrentLoc.FullName + " Reason Access Denied");
                                TrueArgs.Coms.Blocked(CurrentLoc.ToString());
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
                                                lock (ResultsLock)
                                                {
                                                    TrueArgs.Coms.Match(Possible);
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
                                        bool isMatched = MatchThis(Target, Possible);
                                        if (isMatched)
                                        {
                                            if (!ThreadSynchResults)
                                            {
                                                TrueArgs.Coms.Match(Possible);
                                            }
                                            else
                                            {
                                                lock (ResultsLock)
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
        /// <summary>
        /// Start the search rolling. 
        /// </summary>
        /// <param name="Coms">This class is how the search communicates with your code. Cannot be null</param>
        /// <exception cref="IOException">Is thrown if Search is called while searching. </exception>
        /// <exception cref="ArgumentNullException">Is thrown if the Coms argument is null</exception>
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
                    WorkerThreadArgs Args=null;
                    foreach (SearchAnchor Anchor in Anchors)
                    {
                        var AnchorList = Anchor.SplitRoots();
                        if (AnchorList.Length== 0)
                        {
                            throw new InvalidOperationException(NonEmptyAnchorListEmptySplitRoots);
                        }
                        foreach (SearchAnchor SmallAnchor in AnchorList)
                        {
                            Args = new();
                            Args.StartFrom = SmallAnchor;
                            Args.Targets.AddRange(Targets);
                            Args.Coms = Coms;

                            WorkerThreadWithCancelToken Worker = new WorkerThreadWithCancelToken();
                            Worker.Thread = new Thread(() => ThreadWorkerProc(Args));
                            Worker.Token = new CancellationTokenSource();
                            Worker.Args = Args;
                            Args.Token = Worker.Token.Token;


                            WorkerThreads.Add(Worker);
                        }
                    }
                    /*

                        foreach (SearchAnchor Anchor in Anchors)
                    {
                        WorkerThreadArgs Args = new();
                        Args.StartFrom = Anchor;
                        
                        Args.Targets.AddRange(Targets);
                        Args.Coms = Coms;

                        
                        WorkerThreadWithCancelToken Worker = new WorkerThreadWithCancelToken();
                        Worker.Thread = new Thread(() => ThreadWorkerProc(Args)); 
                        Worker.Token = new CancellationTokenSource();

                        Args.Token = Worker.Token.Token;
                        

                        WorkerThreads.Add(Worker);
                    }
                    */
                    
                    bool DoNotNotifyRest = false;
                    foreach (WorkerThreadWithCancelToken t in WorkerThreads)
                    {
                        if (!DoNotNotifyRest)
                        {
                            DoNotNotifyRest = t.Args.Coms.SearchBegin(DateTime.Now);
                        }
                        t.Thread.Start();
                    }
                    SearchCalled = true;
                    WatchdogFireAllDoneSpawner(Args);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace OdinSearchEngine
{
    
    public class OdinSearch
    {
        /// <summary>
        /// reset before we started searching
        /// </summary>
        public void Reset()
        {
            KillSearch();
            ClearSearchAnchorList();
            ClearSearchTargetList();
        }
        

        /// <summary>
        /// pairing a Thread with a a cancilation token.
        /// </summary>
        internal class WorkerThreadWithCancelToken
        {
            public Thread Thread;
            public CancellationTokenSource Token;
        }

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
        internal class WorkerThreadArgs
        {
            /// <summary>
            /// used in the workerthread code. This contains a single start point
            /// </summary>
            public SearchAnchor StartFrom;
            /// <summary>
            /// used in the workerthread code. THis contains a list of what to look for
            /// </summary>
            public readonly List<SearchTarget> Targets = new List<SearchTarget>();
            /// <summary>
            /// Used to let outside (of the thread anyway) to be able to tell the worker thread to quit
            /// </summary>
            public CancellationToken Token;
            /// <summary>
            /// Used in the worker thread.  This is how it will send messages and results outside of it.
            /// </summary>
            public OdinSearch_OutputConsumerBase Coms;
        }
        readonly object TargetLock = new object();
        /// <summary>
        /// Locked when sending a match to the output
        /// </summary>
        readonly object ResultsLock = new object();
        List<WorkerThreadWithCancelToken> WorkerThreads = new List<WorkerThreadWithCancelToken>();

        #region Code for dealing with setting targets
        /// <summary>
        /// Add what to look for here.
        /// </summary>
        readonly List<SearchTarget> Targets = new List<SearchTarget>();

        public void AddSearchTarget(SearchTarget target)
        {
            Targets.Add(target);
        }

        public void ClearSearchTargetList()
        {
            Targets.Clear();
        }
        public SearchTarget[] GetSearchTargetsAsArray()
        {
            return Targets.ToArray();
        }

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

        public void AddSearchAnchor(SearchAnchor Anchor)
        {
            Anchors.Add(Anchor);
        }

        public void ClearSearchAnchorList()
        {
            Anchors.Clear();
        }

        public SearchAnchor[] GetSearchAnchorsAsArray()
        {
            return Anchors.ToArray();
        }

        public ReadOnlyCollection<SearchAnchor> GetSearchAnchorReadOnly()
        {
            return Anchors.AsReadOnly();
        }

        #endregion

        #region Code with Cealing with threads


        /// <summary>
        /// Call Thread.Join() for all worker threads spawned in the list
        /// </summary>
        public void WorkerThreadJoin()
        {
            WorkerThreads.ForEach(p => { p.Thread.Join(); });
        }
        /// <summary>
        /// get if any of the worker threads are alive and running still.
        /// </summary>
        public bool HasActiveSearchThreads
        { 
            get
            {
                bool ret = false;
                WorkerThreads.ForEach(p => { 
                            if (ret == true)
                          if (p.Thread.ThreadState.HasFlag(ThreadState.Running))
                          {
                                ret = true;
                                
                          }
                        });
                return ret;
            }
        }
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
        bool SanityChecks(WorkerThreadArgs Arg)
        {
            return true;
        }

        bool MatchThis(SearchTargetPreDoneRegEx SearchTarget, FileSystemInfo Info)
        {
            bool FinalMatch= true;
            bool MatchedOne = false;
            bool MatchedFailedOne = false;

            // if the filename check has not been disabled
            if (!SearchTarget.SearchTarget.FileNameMatching.HasFlag(OdinSearchEngine.SearchTarget.MatchStyleString.Skip))
            {
                // special case in <the Convert to RegEx routine.  Empty list means we've matched all
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
            
            MatchedOne = false;
            MatchedFailedOne = false;
            exit:
            return FinalMatch;
        }

        bool MatchThis(SearchTargetPreDoneRegEx SearchTarget, FileInfo Info)
        {
            return MatchThis(SearchTarget, Info as FileSystemInfo);
        }
        bool MatchThis(SearchTargetPreDoneRegEx SearchTarget, DirectoryInfo Info)
        {
            return MatchThis(SearchTarget, Info as FileSystemInfo);
        }
        /// <summary>
        /// Unpack the WorkerThreadArgs and go to work
        /// </summary>
        /// <param name="Args"></param>
        void ThreadWorkerProc(object Args)
        {
            Queue<DirectoryInfo> FolderList= new Queue<DirectoryInfo>();
            List<SearchTargetPreDoneRegEx> TargetWithRegEx = new List<SearchTargetPreDoneRegEx>();
            WorkerThreadArgs TrueArgs = Args as WorkerThreadArgs;

           if (TrueArgs != null ) 
            { 
                if (TrueArgs.Targets.Count > 0)
                {
                    if (TrueArgs.StartFrom != null)
                    {
                        foreach (SearchTarget Target in TrueArgs.Targets)
                        {
                            TargetWithRegEx.Add(new SearchTargetPreDoneRegEx(Target));
                        }
                        
                        FolderList.Enqueue(TrueArgs.StartFrom.roots[0]);
                    Reset:

                        if (FolderList.Count > 0)
                        {
                            DirectoryInfo CurrentLoc = FolderList.Dequeue();
                            var Files = CurrentLoc.GetFiles();
                            var Folders = CurrentLoc.GetDirectories();


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
                                            TrueArgs.Coms.Match(Possible);
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
                                            TrueArgs.Coms.Match(Possible);
                                        }
                                    }
                                }
                            }

                            if (TrueArgs.StartFrom.EnumSubFolders)
                            {
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
        /// <param name="Coms"></param>
        public void Search(OdinSearch_OutputConsumerBase Coms)
        {
            if (Coms == null)
            {
                throw new ArgumentNullException(nameof(Coms));
            }
            else
            {
                if (WorkerThreads.Count != 0)
                {
                    throw new InvalidOperationException("Search in Progress. Workerthread != 0");
                }
                else
                {
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

                    foreach (WorkerThreadWithCancelToken t in WorkerThreads)
                    {
                        t.Thread.Start();
                    }
                }
            }
        }
    }
    /// <summary>
    /// Look for matches to a search criteria starting in SearchAchors. Default action is each SearchAnchor gets a worker thread
    /// </summary>
    public class MatchMaker
    {
        List<FileSystemInfo> ResultsContainer = new List<FileSystemInfo>();
        List<FileSystemInfo> BlockedContainer = new List<FileSystemInfo>();
        List<Thread> WorkerCollection = new List<Thread>();
        public readonly List<SearchTarget> SearchFor = new List<SearchTarget>();
        public readonly List<SearchAnchor> Anchors = new List<SearchAnchor>();
        readonly object TargetLock = new object();
        readonly object ResultsLock = new object();

        /// <summary>
        /// Optional:  This is called by the search routine whenever a file system entry is matched
        /// </summary>
        /// <param name="Match"></param>
        /// <returns></returns>
        public delegate void MatchMakerNotify(FileSystemInfo Match);
        /// <summary>
        /// Does not actually compile. It just converts the input filecard to regex to run against filenames.
        /// </summary>
        /// <param name="Target"></param>
        /// <returns>Should one of the <see cref="SearchTarget.FileName"/> contain an entry for "*", a blank list is returned.  This turns the comparing to always true off and matches everything.</returns>
        List<Regex> CompileRegExStuff(SearchTarget Target)
        {
            var ret = new List<Regex>();
            foreach (string s in Target.FileName)
            {
                string pattern;

                pattern = "^" + Regex.Escape(s) + "$";
                pattern = pattern.Replace("\\*", ".*").Replace("\\?", ".");
                if (pattern == "^.*$")  // this is the match anything regexpression.  returning a clear regex list disables the compare that will always be true
                {
                    ret.Clear();
                    return ret;
                }
                if (!Target.FileNameMatching.HasFlag(SearchTarget.MatchStyleString.CaseImportant))
                {
                    ret.Add(new Regex(pattern, RegexOptions.Singleline));
                }
                else
                {
                    ret.Add(new Regex(pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase));
                }

            }
            
            return ret;
        }


        /// <summary>
        /// Compare Entry Entry against Target Target. Returns true if it match and false if it did not. 
        /// </summary>
        /// <param name="Entry"></param>
        /// <param name="Target"></param>
        /// <param name="Names"></param>
        /// <param name="AnyFileName"></param>
        /// <returns></returns>
        bool CompareEntry(FileSystemInfo Entry, SearchTarget Target, List<Regex> Names, bool AnyFileName)
        {
            bool Eliminated = false;
  


            if (!(Target.FileNameMatching.HasFlag(SearchTarget.MatchStyleString.Skip))

                )
            {
                bool MatchedAll = true;
                bool MatchedAny = false;

                if (!AnyFileName)
                {
                    foreach (Regex Checkmate in Names)
                    {
                        if (Checkmate.IsMatch(Entry.FullName))
                        {
                            MatchedAny = true;

                        }
                        else
                        {

                            MatchedAll = false;
                        }
                    }
                }
                else
                {
                    MatchedAll = MatchedAny = true;
                }

                if (Target.FileNameMatching.HasFlag(SearchTarget.MatchStyleString.MatchAll))
                {
                    if ((Target.FileNameMatching.HasFlag(SearchTarget.MatchStyleString.Invert)))
                    {
                        if (MatchedAll)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!MatchedAny)
                        {
                            return false;
                        }
                    }

                }

                if (Target.FileNameMatching.HasFlag(SearchTarget.MatchStyleString.MatchAny))
                {
                    if (MatchedAny == false)
                    {
                        Eliminated = true;
                    }
                }


                if (!Target.AttribMatching1Style.HasFlag(SearchTarget.MatchStyleString.Skip))
                {

                    if (Target.AttribMatching1Style.HasFlag(SearchTarget.MatchStyleString.MatchAll))
                    {
                        if (Target.AttributeMatching1 != FileAttributes.Normal)
                        {
                            if ((Target.AttributeMatching1 & Entry.Attributes) != (Target.AttributeMatching1))
                            {
                                Eliminated = true;
                            }
                        }
                    }
                    if (Target.AttribMatching1Style.HasFlag(SearchTarget.MatchStyleString.MatchAny))
                    {
                        if (Target.AttributeMatching1 == FileAttributes.Normal)
                        {
                            if ((Target.AttributeMatching1 & Entry.Attributes) == 0)
                            {
                                Eliminated = true;
                            }
                        }
                    }
                }
            }


            if (Target.CheckFileSize)
            {
                if (!Entry.Attributes.HasFlag(FileAttributes.Directory))
                {
                    if ((Target.AttributeMatching1 == 0) || (Target.AttributeMatching1.HasFlag(FileAttributes.Directory) == false))
                    {
                        var SizeMe = (FileInfo)Entry;
                        {
                            if (((SizeMe.Length < Target.FileSizeMin) || (SizeMe.Length > Target.FileSizeMax)))
                            {
                                //  Eliminated = true;
                            }
                        }
                    }
                }
            }

            return Eliminated;
        }
        /// <summary>
        /// This one examins the Root contents to seew if any match and ends to results
        /// </summary>
        /// <param name="Root">search this anchor part</param>
        /// <param name="Target">look for this</param>
        /// <param name="CheckSubs">true if we're going to be checking subfolders also</param>
        /// <param name="Blocked">if not null, we add the file system items we can't access to this list</param>
        /// <param name="Matched">if not null, we call this routine with the file system item we match ok</param>
        /// <param name="lockthis">if not null, we lock(lockthis) before calling the matched routine</param>
        /// <param name="Results">put results here</param>
        void CheckContents(string Root, SearchTarget Target, ref List<FileSystemInfo> Results, ref List<FileSystemInfo> Blocked, bool CheckSubs, MatchMakerNotify Matched, object lockthis)
        {
            // if true comparing regex pressing with filename is functionally disabled.
            bool AnyFileName = false;
            bool Eliminated = false;
            List<FileSystemInfo> Subfolders = new List<FileSystemInfo>();
            var Entries = new DirectoryInfo(Root).GetFileSystemInfos();
            List<Regex> Names = CompileRegExStuff(Target);
         
      

            foreach (FileSystemInfo Entry in Entries)
            {
                if (CheckSubs)
                {
                    if (Entry.Attributes.HasFlag(FileAttributes.Directory))
                    {
                        Subfolders.Add(Entry);
                    }
                }
                Eliminated = CompareEntry(Entry, Target, Names, AnyFileName);
                if (lockthis != null)
                {
                    lock (lockthis)
                    {
                        Matched?.Invoke(Entry);
                    }
                }
                else
                {
                    Matched?.Invoke(Entry);
                }
                
                if ( (!Eliminated) && (Results != null))
                {
                    Results.Add(Entry);
                }
            }

            foreach (FileSystemInfo Info in Subfolders)
            {
                try
                {
                    CheckContents(Info.FullName, Target, ref Results, ref Blocked, CheckSubs, Matched, lockthis);
                }
                catch (UnauthorizedAccessException e)
                {
                    if (Blocked != null)
                    {
                        Blocked.Add(Info);
                    }
                }
                catch (IOException e)
                {
                    if (Blocked != null)
                    {
                        Blocked.Add(Info);
                    }
                }
            }


            
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Anchor">Starting location</param>
        /// <param name="Target">What to look for</param>
        /// <param name="Results">put the results here</param>
        void CheckContents(SearchAnchor Anchor, SearchTarget Target, ref List<FileSystemInfo> Results, ref List<FileSystemInfo> Blocked, MatchMakerNotify Matched)
        {
            foreach (DirectoryInfo Starter in Anchor.roots)
            {
                CheckContents(Starter.FullName, Target, ref Results,  ref Blocked, Anchor.EnumSubFolders, Matched, null);
            }
        }

        /// <summary>
        /// The worker thread code.  
        /// </summary>
        /// <param name="Arg"></param>
        /// <param name="Matched"></param>
        /// <param name="lockthis">we only care if lockthis is null or not.  If null, we don't lock(lockthis) when invoking Matched</param>
        void ThreadWorker(object Arg, MatchMakerNotify Matched, object lockthis)
        {
           
            List<FileSystemInfo> Results = new List<FileSystemInfo>();
            List<FileSystemInfo> Blocked = new List<FileSystemInfo>();
            SearchTarget[] SearchCriteria;
            SearchAnchor Anchor = (SearchAnchor)Arg;
            lock (TargetLock)
            {
                SearchCriteria = SearchFor.ToArray();
            }


            foreach (DirectoryInfo Root in Anchor.roots)
            {
                foreach (SearchTarget Target in SearchCriteria)
                {
                    CheckContents(Root.FullName, Target, ref Results, ref Blocked, Anchor.EnumSubFolders, Matched , lockthis);
                }
            }
            
            lock (ResultsLock)
            {
                ResultsContainer.AddRange(Results);
                BlockedContainer.AddRange(Blocked);
            }
            


        }

        public bool IsSearching
        {
            get
            {
                return (WorkerCollection.TrueForAll(p => { return ((p.IsAlive == true)); }));
            }
        }

        /// <summary>
        /// Get the file system items we attempted to access but were blocked for some reason.
        /// </summary>
        public FileSystemInfo[] BlockedResults
        {
            get
            {
                lock (ResultsLock)
                {
                    return BlockedContainer.ToArray();
                }
            }
        }

        /// <summary>
        /// Get the results so far.
        /// </summary>
        public FileSystemInfo[] Results
        {
            get
            {
                lock (ResultsLock)
                {
                    return ResultsContainer.ToArray();
                }
            }
        }

        public bool Search(MatchMakerNotify Matched)
        {
            return Search(Matched, null);
        }
        public bool Search(MatchMakerNotify Matched, object lockthis)
        {
            foreach (Thread t in WorkerCollection)
            {
                t.Abort();
            }
            WorkerCollection.Clear();
            foreach (SearchAnchor A in Anchors)
            {
                var ThreadArg = new Thread(p => { ThreadWorker(A, Matched, lockthis); });


                WorkerCollection.Add(ThreadArg);
                ThreadArg.Start();
            }

            bool Active = true;
            WorkerCollection.ForEach(p => { if (p.IsAlive == false) { Active = false; } });
            return Active;
        }
        public bool Search()
        {
            return Search(null);
        }
    }
}

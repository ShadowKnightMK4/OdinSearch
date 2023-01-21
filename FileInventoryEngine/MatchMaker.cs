using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
namespace FileInventoryEngine
{

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

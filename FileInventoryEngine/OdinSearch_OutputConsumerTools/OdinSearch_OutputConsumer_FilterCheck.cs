using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools
{ 
    /// <summary>
    /// Serves as a base class to filter returned results. <see cref="Match(FileSystemInfo)"/> puts them in <see cref="FilterQuery"/> which can be moved to <see cref="MatchedResults"/> via calling <see cref="CheckFileFilter"/> for one at a time or <see cref="ResolvePendingActions"/> to do that in one shot
    /// </summary>
    public class OdinSearch_OutputConsumer_FilterCheck: OdinSearch_OutputConsumerBase
    {
        /// <summary>
        /// Set to the value you want to pass for your provided <see cref="FilterHandleRoutine(FileSystemInfo)"/>. Unused for the default one
        /// </summary>
        public bool DesiredCheck;
        /// <summary>
        /// Default action is while the <see cref="FilterQuery"/> is not empty, pop an item off from it and pass it to <see cref="CheckFileFilter"/>
        /// </summary>
        /// <returns>Default always returns true.  You should return true if you sucessfully passed each item in the queue to <see cref="CheckFileFilter"/> </returns>
        public override bool ResolvePendingActions()
        {
            while (FilterQuery.Count > 0)
            {
                CheckFileFilter();
            }
            return true;
        }

        /// <summary>
        /// Check if the COMS class has pending actions it needs to do before finishing a search
        /// </summary>
        /// <returns>Your class should return true if there's stuff do it.  Default returns if <see cref="FilterQuery"/> is not empty</returns>
        public override bool HasPendingActions()
        {
            return FilterQuery.Count > 0;
        }
        /// <summary>
        /// Number of matches that failed the filter check.
        /// </summary>
        public UInt128 FilteredResults = 0;
        /// <summary>
        /// We place stuff in here that passes the Filter check <see cref="FilterHandleRoutine(FileSystemInfo)"/>
        /// </summary>
        public readonly List<FileSystemInfo> MatchedResults = new();

        /// <summary>
        /// This holds the stuff that was <see cref="Match(FileSystemInfo)"/> but has not had the <see cref="CheckFileFilter"/> ran against it
        /// </summary>
        protected Queue<FileSystemInfo> FilterQuery = new();

        /// <summary>
        /// Returns if the filter queue is empty
        /// </summary>
        public bool IsFilterQueueEmpty => FilterQuery.Count == 0;
        /// <summary>
        /// Ovvride to control what gets placed in <see cref="MatchedResults"/>. Your routine should return true to keep the item being placed <see cref="Info"/> and false to discard it
        /// </summary>
        /// <param name="Info">This is the item to check. Return true to let it be added <see cref="MatchedResults"/></param>
        /// <returns>Default always returns true. True means passed check, false means it failed check. </returns>
        public virtual bool FilterHandleRoutine(FileSystemInfo Info)
        {
            return true;
        }
        /// <summary>
        /// This is used to eval things not checked, if they're acceptable, we move to <see cref="MatchedResults"/>. This pop
        /// </summary>
        /// <returns>You should return true when moving something from <see cref="FilterQuery"/> to <see cref="MatchedResults"/></returns>
        /// <remarks>Default just pops off the next thing in the <see cref="FilterQuery"/> and moves it to  <see cref="MatchedResults</remarks> You should return false when getting an empty queue
        public virtual bool CheckFileFilter()
        {
            if (this.FilterQuery.Count > 0)
            {
                FileSystemInfo Target;
                try
                {
                    lock (FilterQuery)
                    {
                        Target = FilterQuery.Dequeue();
                    }
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
                bool PassedCheck = FilterHandleRoutine(Target);
                if (PassedCheck )
                {
                    MatchedResults.Add(Target);
                }
                else
                {
                    FilteredResults++;
                }
                return true;
            }
            return false;
        }


        public override void Match(FileSystemInfo info)
        {
            lock (FilterQuery)
            {
                FilterQuery.Enqueue(info);
            }
                base.Match(info);
        }
        public override void AllDone()
        {
            while (FilterQuery.Count > 0)
            {
                CheckFileFilter();
            }
            base.AllDone();
        }
    }
}

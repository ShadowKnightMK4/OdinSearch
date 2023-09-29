using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools
{
    public class OdinSearch_OutputConsumer_FilterCheck: OdinSearch_OutputConsumerBase
    {
        /// <summary>
        /// Stuff that we place here passed the filter;
        /// </summary>
        public readonly List<FileSystemInfo> MatchedResults = new();

        /// <summary>
        /// This holds the stuff that was <see cref="Match(FileSystemInfo)"/> but has not had the <see cref="FilterHandle"/> ran against it
        /// </summary>
        protected Queue<FileSystemInfo> FilterQuery = new();

        /// <summary>
        /// This is used to eval things not checked, if they're acceptable, we move to <see cref="MatchedResults"/>. This pop
        /// </summary>
        /// <returns>You should return true when moving something from <see cref="FilterQuery"/> to <see cref="MatchedResults"/></returns>
        /// <remarks>Default just pops off the next thing in the <see cref="FilterQuery"/> and moves it to  <see cref="MatchedResults/></remarks>
        public virtual bool FilterHandle()
        {
            if (FilterQuery.Count > 0)
            {
                MatchedResults.Add(FilterQuery.Dequeue());
                return true;
            }
            return false;
        }
        public override void Match(FileSystemInfo info)
        {
            FilterQuery.Enqueue(info);
        }
        public override void AllDone()
        {
            while (FilterQuery.Count > 0)
            {
                FilterHandle();
            }
        }
    }
}

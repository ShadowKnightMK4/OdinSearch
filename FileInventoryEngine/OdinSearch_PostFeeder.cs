using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OdinSearchEngine.OdinSearch_OutputConsumerTools;
using System.IO;

namespace OdinSearchEngine
{
    /// <summary>
    /// This class plans to present several ways to present a List of FileSystemInfos to a match <see cref="OdinSearch_OutputConsumerTools.OdinSearch_OutputConsumerBase"/> based class
    /// </summary>
    public static class OdinSearch_Post
    {
        /// <summary>
        /// Present a list of file system items to a Coms class
        /// </summary>
        /// <param name="matches">list of match items</param>
        /// <param name="Coms">class to fake matches for</param>
        /// <param name="SearchStart">time to start the 'match'. Use <see cref="DateTime.MinValue"/> for  <see cref="DateTime.Now"/></param>
        public static void PresentMatches(List<FileSystemInfo> matches, OdinSearch_OutputConsumerBase Coms, DateTime SearchStart)
        {
            if (SearchStart == DateTime.MinValue)
            {
                SearchStart = DateTime.Now;
            }
            Coms.SearchBegin(SearchStart);

            for (int step= 0;step < matches.Count; step++)
            {
                Coms.Match(matches[step]);
            }

            Coms.AllDone();
        }
    }
}

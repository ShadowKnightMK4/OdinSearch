using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine
{
    /// <summary>
    /// For when you just need to get the results of a search and want to look at them later.
    /// </summary>
    public class OdinSearch_OutputConsumerGatherResults: OdinSearch_OutputConsumerBase
    {
        public readonly List<FileSystemInfo> Results = new();
        public readonly List<string> BlockedResults = new();
        public readonly List<string> MessageResults = new();
        public override void Blocked(string Blocked)
        {
            BlockedResults.Add(Blocked);
            base.Blocked(Blocked);
        }

        public override void Match(FileSystemInfo info)
        {
            Results.Add(info);
            base.Match(info);
        }

        public override void Messaging(string Message)
        {
            MessageResults.Add(Message);
            base.Messaging(Message);
        }

        public override void WasNotMatched(FileSystemInfo info)
        {
            base.WasNotMatched(info);
        }

    }
}

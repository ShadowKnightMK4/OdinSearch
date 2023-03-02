using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools
{
    /// <summary>
    /// Serves as an example.  Matches are sent to stdout,  blocks are send to stderr
    /// </summary>
    public class OdinSearch_OutputSimpleConsole : OdinSearch_OutputConsumerBase
    {
        TextWriter stdout, stderr;
        public OdinSearch_OutputSimpleConsole()
        {
            stdout = Console.Out;
            stderr = Console.Error;
        }

        public override void WasNotMatched(FileSystemInfo info)
        {
            base.WasNotMatched(info);
        }
        public override void Match(FileSystemInfo info)
        {
            stdout.WriteLine("File Match: \"{0}\" @ \"{1}\"", info.Name, info.FullName);
            base.Match(info);
        }

        public override void Blocked(string Blocked)
        {
            stderr.WriteLine(Blocked);
            base.Blocked(Blocked);
        }

        public override void Messaging(string Message)
        {
            stdout.WriteLine(Message);
            base.Messaging(Message);
        }
    }
}

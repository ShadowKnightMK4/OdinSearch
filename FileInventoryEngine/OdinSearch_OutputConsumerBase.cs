using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine
{

    /// <summary>
    /// Serves as an example.  Matches are sent to stdout,  blocks are send to stderr
    /// </summary>
    public class OdinSearch_OutputSimpleConsole: OdinSearch_OutputConsumerBase
    {
        TextWriter stdout, stderr;
        public OdinSearch_OutputSimpleConsole()
        {
            stdout = Console.Out;
            stderr = Console.Error; 
        }

        public override void WasNotMatched(FileSystemInfo info)
        {
            // do nothiong
        }
        public override void Match(FileSystemInfo info)
        {
            stdout.WriteLine(info.Name);
        }

        public override void Blocked(string Blocked)
        {
            stderr.WriteLine(Blocked);
        }

        public override void Messaging(string Message)
        {
            stdout.WriteLine(Message);
        }
    }

    /// <summary>
    /// The OdinSearch class posts results and when it can't access something to a class of this type.  
    /// </summary>
    public abstract class OdinSearch_OutputConsumerBase: IDisposable
    {
        /// <summary>
        /// For Future. Set if you want the WasNotMatched called for each time. This does NOTHING Currently.
        /// </summary>
        public bool EnableNotMatchCall = false;

        /// <summary>
        /// For Future: OdinSearch does not call this.
        /// </summary>
        /// <param name="info"></param>
        /// <exception cref="NotImplementedException">Throws this</exception>
        public virtual void WasNotMatched(FileSystemInfo info)
        {
            throw new NotImplementedException(nameof(WasNotMatched));
        }

        /// <summary>
        /// This is called when a match is found
        /// </summary>
        /// <param name="info"></param>
        public abstract void Match(FileSystemInfo info);

        /// <summary>
        /// This is called when the search encounters an exception when attempting to examine a file/folder
        /// </summary>
        /// <param name="Blocked"></param>
        public abstract void Blocked(string Blocked);

        /// <summary>
        /// Text output that's now a block or a file match
        /// </summary>
        /// <param name="Message"></param>
        public abstract void Messaging(string Message);

        /// <summary>
        /// Default class needs not dispose 
        /// </summary>
        public virtual void Dispose()
        {
            
        }
    }
}

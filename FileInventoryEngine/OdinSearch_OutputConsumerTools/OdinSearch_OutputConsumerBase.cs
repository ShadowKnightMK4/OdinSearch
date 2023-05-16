using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools
{

    

    /// <summary>
    /// The OdinSearch class Search Threads use this class to send output/communications to your code.
    /// </summary>
    public abstract class OdinSearch_OutputConsumerBase : IDisposable
    {
        public bool Disposed { get; protected set; }
        /// <summary>
        /// For Future. Set if you want the WasNotMatched called for each time. This does NOTHING Currently.
        /// </summary>
        public bool EnableNotMatchCall = false;
        public bool SearchOver = false;
        /// <summary>
        /// Base routine increments this by 1 on each call
        /// </summary>
        public UInt128 TimesMatchCalled = 0;
        /// <summary>
        /// Base routine increments this by 1 on each call
        /// </summary>
        public UInt128 TimesNoMatchCalled = 0;
        /// <summary>
        /// Base routine increments this by 1 on each call
        /// </summary>
        public UInt128 TimesBlockCalled = 0;
        /// <summary>
        /// Base routine increments this by 1 on each call
        /// </summary>

        public UInt128 TimesMessageCalled = 0;
        /// <summary>
        /// For Future: OdinSearch does not call this.
        /// </summary>
        /// <param name="info"></param>
        /// <exception cref="NotImplementedException">Throws this</exception>
        public virtual void WasNotMatched(FileSystemInfo info)
        {
            if (TimesNoMatchCalled != UInt128.MaxValue)
                TimesNoMatchCalled++;
        }

        /// <summary>
        /// This is called when a match is found
        /// </summary>
        /// <param name="info"></param>
        public virtual void Match(FileSystemInfo info)
        {
            if (TimesMatchCalled != UInt128.MaxValue)
            TimesMatchCalled++;
        }

        /// <summary>
        /// This is called when the search encounters an exception when attempting to examine a file/folder
        /// </summary>
        /// <param name="Blocked"></param>
        public virtual void Blocked(string Blocked)
        {
            if (TimesBlockCalled != UInt128.MaxValue)
                TimesBlockCalled++;
        }

        /// <summary>
        /// Text output that's now a block or a file match
        /// </summary>
        /// <param name="Message"></param>
        public virtual void Messaging(string Message)
        {
            if (TimesMessageCalled!= UInt128.MaxValue)
                TimesMessageCalled++;
        }

        /// <summary>
        /// Called once for each thread when the search is started by OdinSearch. It's called shortly before each thread starts
        /// </summary>
        /// <param name="Start">DateTime of call</param>
        /// <returns>Your routine should return true to continue being called for the rest of the threads or false if just a single notify is enough</returns>
        public virtual bool SearchBegin(DateTime Start)
        {
            return false;
        }

        /// <summary>
        /// FUTURE: Search calls this when all threads searching are done. Base just sets variable <see cref="SearchOver"/> to true
        /// </summary>
        public virtual void AllDone()
        {
            SearchOver = true;
        }
        /// <summary>
        /// Default class needs not dispose.  
        /// </summary>
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
            Disposed = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools
{

    /// <summary>
    /// A thinly wrapped <see cref="KeyNotFoundException"/> for argument access. Thrown when attempting to access an argument via the indexer that is not existing
    /// </summary>
    public class ArgumentNotFoundException : KeyNotFoundException
    { 
        public ArgumentNotFoundException() { }  
        public ArgumentNotFoundException(string message) : base(message) { }
    }


    /// <summary>
    /// The OdinSearch class Search Threads use this class to send output/communications to your code.
    /// </summary>
    public abstract class OdinSearch_OutputConsumerBase : IDisposable
    {

        /// <summary>
        /// If the class has pending stuff to do before reporting search is over, override this to give confirmation it's over.
        /// </summary>
        /// <returns>return true if this class has something to do before finishing search and false if it's ready to end. Default always returns false to end it</returns>
        public virtual bool HasPendingActions()
        {
            return false;
        }

        /// <summary>
        /// Fully Resolve any pending actions this class has do to.
        /// </summary>
        /// <returns>return true if all actions are resolved </returns>
        public virtual bool ResolvePendingActions()
        {
            return true;
        }
        protected bool ArgCheck(string RequiredArg)
        {
            if (RequiredArg == null)
                return true;
            else
                return CustomParameters.Keys.Contains(RequiredArg);
        }

        /// <summary>
        /// Return if these strings are in our custom argument list
        /// </summary>
        /// <param name="RequiredArgs"></param>
        /// <returns></returns>
        protected bool ArgCheck(string[] RequiredArgs)
        {
            if (RequiredArgs.Length == 0)
            {
                return true;
            }
            foreach (string s in RequiredArgs)
            {
                if (!ArgCheck(s))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get the names of set custom parameters for your class. 
        /// </summary>
        /// <returns></returns>
        public string[] GetCustomParameterNames()
        {
            return CustomParameters.Keys.ToArray();
        }
        /// <summary>
        /// Set a custom argument.
        /// </summary>
        /// <param name="name">argument name</param>
        /// <param name="val">argument value</param>
        public void SetCustomParameter(string name, object val)
        {
            CustomParameters[name] = val;
        }

        /// <summary>
        /// Get custom argument
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNotFoundException">Is thown if its not there.</exception>
        public object GetCustomParameter(string name)
        {
            try
            {
                return CustomParameters[name];
            }
            catch (KeyNotFoundException e)
            {
                throw new ArgumentNotFoundException(e.Message);
            }
        }

        /// <summary>
        /// Acess or set any custom arguments for this <see cref="OdinSearch_OutputConsumerBase"/> class type
        /// </summary>
        /// <param name="ArgName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNotFoundException"></exception>
        public object this[string ArgName]
        {
            get
            {
                try
                {
                    var ret = CustomParameters[ArgName];
                    return ret;
                }
                catch (KeyNotFoundException)
                {
                    throw new ArgumentNotFoundException(ArgName);
                }
            }
            set
            {
                CustomParameters[ArgName] = value;
            }
        }


        /// <summary>
        /// Custom arguments are stored here.  Should they require a call to Dipose when done, implement Dispose in your subclass to do this.
        /// </summary>
        protected readonly Dictionary<string, object> CustomParameters = new();
        public bool Disposed { get; protected set; }
        /// <summary>
        /// For Future. Set if you want the WasNotMatched called for each time. This does NOTHING Currently.
        /// </summary>
        public bool EnableNotMatchCall = false;
        /// <summary>
        /// This is set by the <see cref="AllDone"/> to be true when called.
        /// </summary>
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
        /// Text output that's not a block or a file match
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
        /// <exception cref="InvalidOperationException">A subclass may throw Exceptions if a required custom arg is not set. That will stop the search from started</exception>
        /// <remarks>Note that exceptions triggered will abort the search starting.</remarks>
        public virtual bool SearchBegin(DateTime Start)
        {
            return false;
        }

        /// <summary>
        /// OdinSearch calls this when all threads searching are done. Base just sets variable <see cref="SearchOver"/> to true
        /// </summary>
        public virtual void AllDone()
        {
            SearchOver = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Default class disposal. Invokes resolve pending actions. Argument is false within the finalizer (
        /// </summary>
        /// <param name="disposing">true means called by public disposal (managed and unmanaged dispose please), false means explicit call to Dispose (do not deal with managed resources, deal with unmanaged resources)</param>
        protected virtual void Dispose(bool disposing)
        {
            ResolvePendingActions();
            Disposed = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools
{
    /// <summary>
    /// Load a Dll holding an implementaion of our <see cref="OdinSearch_OutputConsumerBase"/> class with a few changes
    /// 
    /// </summary>
    public class OdinSearch_OutputConsumer_ExternUnmangedPlugin: OdinSearch_OutputConsumerBase
    {

        /// <summary>
        /// This is the DLL or Shared libraruy to load From
        /// </summary>

        public const string SetDllTarget = "SetDllTarget";
        
        private nint GetProcAddr(nint Handle, string name)
        {
            var ret = GetProcAddress(Handle, name);
            return ret;
        }
        nint DllHandle = 0;
        private bool LoadLibrary()
        {
            string Target = this[SetDllTarget] as string;
         
            if (Target != null)
            {
                DllHandle = LoadLibraryW(Target);
            }
            return DllHandle != 0;
        }

        private void FreeLibrary(nint Handle)
        {
            FreeLibraryUnmanged(Handle);
        }
#if WINDOWS

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern nint GetProcAddress(nint hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern nint LoadLibraryW(string Target);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint ="FreeLibrary")]
        static extern bool FreeLibraryUnmanged(nint Target);
#endif
        
        public bool ResolvePointers(string TargetDll)
        {
#if WINDOWS
            if (!LoadLibrary()) { return false; }

#else
            throw new NotImplementedException("Need to add code to load the shared library/ DLL.");
            return;
#endif

            ExternAllDone = Marshal.GetDelegateForFunctionPointer<AllDonePtr>(GetProcAddr(DllHandle, "AllDone"));
            ExternBlock = Marshal.GetDelegateForFunctionPointer<BlockedPtr>(GetProcAddr(DllHandle, "Blocked"));
            ExternPending = Marshal.GetDelegateForFunctionPointer<HasPendingActionsPtr>(GetProcAddr(DllHandle, "HasPendingActions"));
            ExternMatch = Marshal.GetDelegateForFunctionPointer<MatchPtr>(GetProcAddr(DllHandle, "Match"));
            ExternMessage = Marshal.GetDelegateForFunctionPointer<MessagePtr>(GetProcAddr(DllHandle, "Messaging"));
            ExternResolve = Marshal.GetDelegateForFunctionPointer<ResolveActionPtr>(GetProcAddr(DllHandle, "ResolvePendingActions"));
            ExternSearch = Marshal.GetDelegateForFunctionPointer<SeachBeginPtr>(GetProcAddress(DllHandle, "SearchBegin"));
            ExternWasNotMatched = Marshal.GetDelegateForFunctionPointer<WasNotMatchedPtr>(GetProcAddr(DllHandle, "WasNotMatched"));
            return true;
        }

        delegate void AllDonePtr();

        AllDonePtr ExternAllDone = null;
        public override void AllDone()
        {
            if ( (Disposed))
            {
                throw new ObjectDisposedException("This instance was disposed and no longer has access to the external plugin");
            }
            if (ExternAllDone != null)
                ExternAllDone();
            base.AllDone();
        }

        delegate void BlockedPtr([MarshalAs(UnmanagedType.LPWStr)] string s);
        BlockedPtr ExternBlock = null;
        public override void Blocked(string Blocked)
        {
            if ((Disposed))
            {
                throw new ObjectDisposedException("This instance was disposed and no longer has access to the external plugin");
            }

            if (ExternBlock != null)
                ExternBlock(Blocked);
            base.Blocked(Blocked);
        }

        delegate bool HasPendingActionsPtr();
        HasPendingActionsPtr ExternPending = null;
        public override bool HasPendingActions()
        {
            if ((Disposed))
            {
                throw new ObjectDisposedException("This instance was disposed and no longer has access to the external plugin");
            }

            if (ExternPending != null)
                return ExternPending();
            return base.HasPendingActions();
        }


        delegate void MatchPtr([MarshalAs(UnmanagedType.LPWStr)] string info);
        MatchPtr ExternMatch = null;
        /// <summary>
        /// Match for the C level plugin. Important, the external code will get an Unicode string to the full location rather than a <see cref="FileSystemInfo"/>
        /// </summary>
        /// <param name="info">matched file</param>
        /// <remarks>Passes the full path as a unicode string. It's up to the external routine to do what it needs to do</remarks>
        public override void Match(FileSystemInfo info)
        {
            if ((Disposed))
            {
                throw new ObjectDisposedException("This instance was disposed and no longer has access to the external plugin");
            }

            if (ExternMatch != null)
                ExternMatch(info.FullName);
            base.Match(info);
        }
        delegate void MessagePtr(string msg);
        MessagePtr ExternMessage = null;
        public override void Messaging(string Message)
        {
            if ((Disposed))
            {
                throw new ObjectDisposedException("This instance was disposed and no longer has access to the external plugin");
            }

            if (ExternMessage != null)
                ExternMessage(Message);
            else
                base.Messaging(Message);
        }

        delegate bool ResolveActionPtr();
        ResolveActionPtr ExternResolve = null;
        public override bool ResolvePendingActions()
        {
            if ((Disposed))
            {
                throw new ObjectDisposedException("This instance was disposed and no longer has access to the external plugin");
            }

            if (ExternResolve != null)
                return ExternResolve();
            return base.ResolvePendingActions();
        }

        delegate bool SeachBeginPtr(Int64 Univeral);
        SeachBeginPtr ExternSearch = null;
        /// <summary>
        /// SearchBegin for the C level plugin.  The Ticks value of the DateTime start is passed
        /// </summary>
        /// <param name="Start"></param>
        /// <returns></returns>
        public override bool SearchBegin(DateTime Start)
        {
            if ((Disposed))
            {
                throw new ObjectDisposedException("This instance was disposed and no longer has access to the external plugin");
            }

            if (ExternSearch == null)
            {
                this.ResolvePointers(GetCustomParameter(SetDllTarget) as string);
            }

            if (ExternSearch != null)
                return ExternSearch(Start.Ticks);
            return base.SearchBegin(Start);
        }

        delegate void WasNotMatchedPtr(string Info);
        WasNotMatchedPtr ExternWasNotMatched = null;

        /// <summary>
        /// The not match value,  The FullName to the file system item is passed
        /// </summary>
        /// <param name="info"></param>
        public override void WasNotMatched(FileSystemInfo info)
        {
            if ((Disposed))
            {
                throw new ObjectDisposedException("This instance was disposed and no longer has access to the external plugin");
            }

            ExternWasNotMatched?.Invoke(info.FullName);

            base.WasNotMatched(info);
        }

        protected override void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    if (DllHandle != 0)
                    {
                        FreeLibrary(DllHandle);
                    }
                    base.Dispose();
                }
            }
            base.Dispose(disposing);
        }

    }
}

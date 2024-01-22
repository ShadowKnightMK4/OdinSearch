using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

using static OdinSearchEngine.OdinSearch_OutputConsumerTools.ExternalBased.OdinSearch_OutputConsumer_PluginDelegates_Unmanaged1;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools.ExternalBased
{

    /// <summary>
    /// This is thrown by <see cref="OdinSearch_OutputConsumer_UnmanagedPlugin"/> when the plugin reports it's not supported
    /// </summary>
    public class UnsupportedPluginException : Exception
    {
        public UnsupportedPluginException(string message) : base(message)
        {
        }
    }
    /// <summary>
    /// This thing is reponsible for loading and handling the plugin protocol.
    /// </summary>
    internal sealed class UnmanagedPlugin_Loader : IDisposable
    {
        public enum PluginVersion : int
        {
            Version1 = 0
        }

        public readonly PluginVersion Version = PluginVersion.Version1;

        public UnmanagedPlugin_Loader(string TargetDll)
        {
            DllContainer = NativeLibrary.Load(TargetDll);
            nint ptr = NativeLibrary.GetExport(DllContainer, "PluginInit");


            if (ptr != 0)
            {
                InitPlugin = Marshal.GetDelegateForFunctionPointer<InitPluginPtr>(ptr);
                if (InitPlugin(Version) == 0)
                {
                    throw new UnsupportedPluginException("Plugin reports it does not support our protocol.");
                }
                else
                {
                    resolve_plugin();
                }
            }
        }

        nint DllContainer;

        public void Dispose()
        {
            if (DllContainer != 0)
            {
                ExternCleanupPluginPtr?.Invoke();

                NativeLibrary.Free(DllContainer);
                DllContainer = 0;
            }
        }


        #region version1

        public AllDonePtr ExternAllDone = null;
        public BlockedPtr ExternBlocked = null;
        public HasPendingActionsPtr ExternPending = null;
        public MatchPtr ExternMatch = null;
        public MessagePtr ExternMessage = null;
        public ResolveActionPtr ExternResolve = null;
        public SeachBeginPtr ExternSearch = null;
        public WasNotMatchedPtr ExternWasNotMatched = null;

        void resolve_plugin()
        {
            ExternAllDone = Marshal.GetDelegateForFunctionPointer<AllDonePtr>(NativeLibrary.GetExport(DllContainer, "AllDone"));
            ExternBlocked = Marshal.GetDelegateForFunctionPointer<BlockedPtr>(NativeLibrary.GetExport(DllContainer, "Blocked"));
            ExternPending = Marshal.GetDelegateForFunctionPointer<HasPendingActionsPtr>(NativeLibrary.GetExport(DllContainer, "HasPendingActions"));
            ExternMatch = Marshal.GetDelegateForFunctionPointer<MatchPtr>(NativeLibrary.GetExport(DllContainer, "Match"));
            ExternMessage = Marshal.GetDelegateForFunctionPointer<MessagePtr>(NativeLibrary.GetExport(DllContainer, "Messaging"));
            ExternResolve = Marshal.GetDelegateForFunctionPointer<ResolveActionPtr>(NativeLibrary.GetExport(DllContainer, "ResolvePendingActions"));
            ExternSearch = Marshal.GetDelegateForFunctionPointer<SeachBeginPtr>(NativeLibrary.GetExport(DllContainer, "SearchBegin"));
            ExternWasNotMatched = Marshal.GetDelegateForFunctionPointer<WasNotMatchedPtr>(NativeLibrary.GetExport(DllContainer, "WasNotMatched"));
        }

        #endregion


        #region delgates types
        /// <summary>
        /// 
        /// </summary>
        /// <param name="supportedversion">min version the caller (that's us) supports</param>
        /// <returns>your code should return non zero if ok and 0 if unsupported </returns>
        delegate uint InitPluginPtr(PluginVersion supportedversion);

        delegate void CleanUpPluginPtr();
        CleanUpPluginPtr ExternCleanupPluginPtr = null;
        #endregion

        InitPluginPtr InitPlugin;
    }




    /// <summary>
    /// load and deal with an implementation of the communcation class - unmanaged protocol.
    /// </summary>
    public class OdinSearch_OutputConsumer_UnmanagedPlugin : OdinSearch_OutputConsumerBase
    {
        public OdinSearch_OutputConsumer_UnmanagedPlugin()
        {

        }

        public OdinSearch_OutputConsumer_UnmanagedPlugin(string location)
        {
            SetPluginLocation(location);
        }

        public override void AllDone()
        {
            ExternHandler.ExternAllDone();
            base.AllDone();
        }

        public override void Match(FileSystemInfo info)
        {
            ExternHandler.ExternMatch(info.FullName);
        }

        public override void WasNotMatched(FileSystemInfo info)
        {
            ExternHandler.ExternWasNotMatched(info.FullName);
        }


        public override bool ResolvePendingActions()
        {
            if (ExternHandler.ExternPending != null)
            {
                return ExternHandler.ExternPending();
            }
            return base.ResolvePendingActions();
        }
        public override void Messaging(string Message)
        {
            ExternHandler.ExternMessage(Message);
        }
        public override bool SearchBegin(DateTime Start)
        {
            return ExternHandler.ExternSearch(Start.Ticks);
        }

        public override bool HasPendingActions()
        {
            if (ExternHandler.ExternPending != null)
            {
                return ExternHandler.ExternPending();
            }
            return base.HasPendingActions();
        }
        public override void Blocked(string Blocked)
        {
            ExternHandler.ExternBlocked(Blocked);
            base.Blocked(Blocked);
        }

        /// <summary>
        /// Tell this class where the plugin is located in the file systme
        /// </summary>
        /// <param name="PluginLocation">a shared library (Windows DLL or Linux SO)</param>
        public void SetPluginLocation(string PluginLocation)
        {
            ExternHandler?.Dispose();
            ExternHandler = new UnmanagedPlugin_Loader(PluginLocation);
        }

        UnmanagedPlugin_Loader ExternHandler;

        /// <summary>
        /// Release the loaded plugin
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                ExternHandler.Dispose();
                ExternHandler = null;
            }
            base.Dispose(disposing);
        }
    }



    /// <summary>
    /// Load a Dll holding an implementaion of our <see cref="OdinSearch_OutputConsumerBase"/> class with a few changes
    /// </summary>
    /// <remarks>This class is intended to be an Example. There's not plans to modify this currently unless there's issues / bug fixes. IMPORTANT: This does not check if the loaded dll/so file is signed</remarks>
    [Obsolete("Fixed point in time. Still ok to use but won't be extended. Use OdinSearch_OutputConsumer_UnmanagedPlugin() if needing extra")]
    public class OdinSearch_OutputConsumer_ExternUnmangedPlugin_Example : OdinSearch_OutputConsumerBase
    {

        /// <summary>
        /// This is the DLL or Shared libraruy to load From
        /// </summary>

        public const string SetDllTarget = "SetDllTarget";

        private nint GetProcAddr(nint Handle, string name)
        {
            var ret = NativeLibrary.GetExport(Handle, name);
            return ret;
        }
        nint DllHandle = 0;
        private bool LoadLibrary()
        {

            string Target = this[SetDllTarget] as string;

            if (Target != null)
            {
                DllHandle = NativeLibrary.Load(SetDllTarget);
            }
            return DllHandle != 0;
        }

        private void FreeLibrary(nint Handle)
        {
            NativeLibrary.Free(DllHandle);
        }
#if WINDOWS

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern nint GetProcAddress(nint hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern nint LoadLibraryW(string Target);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FreeLibrary")]
        static extern bool FreeLibraryUnmanged(nint Target);
#endif

        public bool ResolvePointers(string TargetDll)
        {
#if WINDOWS
            if (!LoadLibrary()) { return false; }

#else
            throw new NotImplementedException("Need to add code to load the shared library/ DLL.");
#endif

            ExternAllDone = Marshal.GetDelegateForFunctionPointer<AllDonePtr>(GetProcAddr(DllHandle, "AllDone"));
            ExternBlock = Marshal.GetDelegateForFunctionPointer<BlockedPtr>(GetProcAddr(DllHandle, "Blocked"));
            ExternPending = Marshal.GetDelegateForFunctionPointer<HasPendingActionsPtr>(GetProcAddr(DllHandle, "HasPendingActions"));
            ExternMatch = Marshal.GetDelegateForFunctionPointer<MatchPtr>(GetProcAddr(DllHandle, "Match"));
            ExternMessage = Marshal.GetDelegateForFunctionPointer<MessagePtr>(GetProcAddr(DllHandle, "Messaging"));
            ExternResolve = Marshal.GetDelegateForFunctionPointer<ResolveActionPtr>(GetProcAddr(DllHandle, "ResolvePendingActions"));
            ExternSearch = Marshal.GetDelegateForFunctionPointer<SeachBeginPtr>(GetProcAddr(DllHandle, "SearchBegin"));
            ExternWasNotMatched = Marshal.GetDelegateForFunctionPointer<WasNotMatchedPtr>(GetProcAddr(DllHandle, "WasNotMatched"));
            return true;
        }

        delegate void AllDonePtr();

        AllDonePtr ExternAllDone = null;
        public override void AllDone()
        {
            if (Disposed)
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
            if (Disposed)
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
            if (Disposed)
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
            if (Disposed)
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
            if (Disposed)
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
            if (Disposed)
            {
                throw new ObjectDisposedException("This instance was disposed and no longer has access to the external plugin");
            }

            if (ExternResolve != null)
                return ExternResolve();
            return base.ResolvePendingActions();
        }

        delegate bool SeachBeginPtr(long Univeral);
        SeachBeginPtr ExternSearch = null;
        /// <summary>
        /// SearchBegin for the C level plugin.  The Ticks value of the DateTime start is passed
        /// </summary>
        /// <param name="Start"></param>
        /// <returns></returns>
        public override bool SearchBegin(DateTime Start)
        {
            if (Disposed)
            {
                throw new ObjectDisposedException("This instance was disposed and no longer has access to the external plugin");
            }

            if (ExternSearch == null)
            {
                ResolvePointers(GetCustomParameter(SetDllTarget) as string);
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
            if (Disposed)
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
                    Dispose();
                }
            }
            base.Dispose(disposing);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools.ExternalBased
{
    /// <summary>
    /// Delegates for external plugin version 1
    /// </summary>
    static class OdinSearch_OutputConsumer_PluginDelegates_Unmanaged1
    {
        public delegate void AllDonePtr();
        public delegate void BlockedPtr([MarshalAs(UnmanagedType.LPWStr)] string s);
        public delegate bool HasPendingActionsPtr();
        public delegate void MatchPtr([MarshalAs(UnmanagedType.LPWStr)] string info);
        public delegate void MessagePtr([MarshalAs(UnmanagedType.LPWStr)] string msg);
        public delegate bool ResolveActionPtr();
        public delegate bool SeachBeginPtr(long Univeral);
        public delegate void WasNotMatchedPtr(string Info);
        
    }
}

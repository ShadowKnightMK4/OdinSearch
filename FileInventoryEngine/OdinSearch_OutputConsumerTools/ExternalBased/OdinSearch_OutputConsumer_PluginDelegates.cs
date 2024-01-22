using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
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


    public  class PluginCertificateCheckFailException: Exception
    {
        public PluginCertificateCheckFailException(string offender) : base ("This plugin did not pass the certificate test: " + offender )
        {
            
        }
    }
    /// <summary>
    /// this is used by <see cref="OdinSearch_OutputConsumer_UnmanagedPlugin"/> and <see cref="OdinSearch_OutputConsumer_ExternManaged"/> to
    /// act as a guard against loading unsigned certificates if unexpected.
    /// </summary>
    /// <remarks>Release build checks if executing code is signed, if so we pull both certificates and compare. Plugin is allowed to be loaded if the certiciates match BUT if unsigned or in DEBUG mode, that's disabled</remarks>

    public static class OdinSearch_OutputConsumer_PluginCheck
    {
        static OdinSearch_OutputConsumer_PluginCheck()
        {
            Init();
        }
        public static X509Certificate2 CheckAgainstThis = null;

        public static bool WeAreSigned
        {
            get
            {
                return CheckAgainstThis != null;
            }
        }

        
        /// <summary>
        /// Init this to load our certificate if any into CheckAgainstThis
        /// </summary>
        public static void Init()
        {
            if (CheckAgainstThis != null)
            {
                CheckAgainstThis = null;
            }
            try
            {
                CheckAgainstThis = (X509Certificate2)X509Certificate2.CreateFromSignedFile(Assembly.GetExecutingAssembly().Location);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                ; // its fine, likely just unsigned.
                CheckAgainstThis = null;
            }
        }
        /// <summary>
        /// We see if this file is signed with a copy of *our* certificate stored at <see cref="CheckAgainstThis"/>
        /// </summary>
        /// <param name="Location">file to load.</param>
        /// <returns>return if trusted by system and matches</returns>
        public static bool CheckForCertificate(string Location)
        {
            bool pass = false;
            X509Certificate2 TestAgainst = null;
            if (CheckAgainstThis == null)
            {
                return true;
            }

            try
            {
                TestAgainst= (X509Certificate2)X509Certificate.CreateFromSignedFile(Location);
                if (!TestAgainst.Verify() )
                {
                    return false;
                }
                if (TestAgainst.GetPublicKey().SequenceEqual(CheckAgainstThis.GetPublicKey()))
                {
                    pass = true;
                }
            }
            finally
            {
                TestAgainst?.Dispose();
            }
            return pass;
        }


    }
    
}

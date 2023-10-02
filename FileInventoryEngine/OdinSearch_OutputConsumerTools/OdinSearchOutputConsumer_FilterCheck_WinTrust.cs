using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools
{
    
    public static class FilterCheck_VerifyCertRoutine
    {
        /// <summary>
        /// Attempts to create a <see cref="X509Certificate2"/> based on the passed item
        /// </summary>
        /// <param name="Info"></param>
        /// <returns>returns true if <see cref="X509Certificate2.Verify"/> does and false if the cert cant be created or is not trusted</returns>
        public static bool TrustThis(FileSystemInfo Info)
        {
            X509Certificate2 cert = null;
            try
            {
                cert =  new X509Certificate2(Info.FullName);
                try
                {
                    return cert.Verify();
                }
                catch (System.Security.Cryptography.CryptographicException)
                {
                    return false;
                }
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                return false;
            }
            finally
            {
                cert?.Dispose();
            }
        }
    }

    /// <summary>
    /// Example of <see cref="OdinSearch_OutputConsumer_FilterCheck"/>.  This software attempts to load the certificate in the file and verify via <see cref="X509Certificate2.Verify"/> 
    /// </summary>
    public class OdinSearchOutputConsumer_FilterCheck_CertExample: OdinSearch_OutputConsumer_FilterCheck
    {
        /// <summary>
        /// If true, things that are trusted according to <see cref="FilterCheck_VerifyCertRoutine.TrustThis(FileSystemInfo)"/> are added to the list.  If false, those that aren't trusted are added to the list
        /// </summary>
        /// <remarks>Note this is backed in  <see cref="OdinSearch_OutputConsumer_FilterCheck.DesiredCheck"/></remarks>
        public bool WantTrusted { get => DesiredCheck; set => DesiredCheck = value; }

        /// <summary>
        /// returns if the passed file system item's return value of <see cref="FilterCheck_VerifyCertRoutine.TrustThis(FileSystemInfo)"/> matches <see cref="WantTrusted"/>
        /// </summary>
        /// <param name="Info">File system item to check</param>
        /// <returns>This returns if the routine called returns the WantTrusted Value</returns>
        public override bool FilterHandleRoutine(FileSystemInfo Info)
        {
            return (FilterCheck_VerifyCertRoutine.TrustThis(Info) == WantTrusted);
        }
    }
    
}

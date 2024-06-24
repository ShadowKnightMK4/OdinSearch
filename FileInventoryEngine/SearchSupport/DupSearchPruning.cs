using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Numerics;
namespace OdinSearchEngine.SearchSupport
{
    internal static class DupSearchPruning_Imports
    {

    }

    /// <summary>
    /// This class tracks input locations after following the link (and insuring we don't accidently loop).
    /// </summary>
    /// <remarks>Why Prune? I was watching Loki the TV when first starting this software</remarks>
    public class DupSearchPruning
    {
        class prune_entry
        {
            public prune_entry(string location, bool folder) 
            {
                this.location = location;
                SubFolders = folder;
            }
            public string location;
            public bool SubFolders;
        }
        SHA256 SHA256 = SHA256.Create();

        /// <summary>
        /// Follow Links, resolve reparse points.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string ResolveToRealPath(string path)
        {
            string ret = null;
            begin:
            FileSystemInfo Info = new FileInfo(path);
            if (Info.LinkTarget != null)
            {
                path = Info.LinkTarget;
                goto begin;
            }

            ret = Info.FullName;
            return ret;

        }
        public byte[] GetPathHash(string path)
        {
            BigInteger ret = 0;
            for (int i  = 0; i < path.Length; i++)
            {
                ret += i + path[i];
            }
            return Encoding.UTF8.GetBytes(path);
             //return path.GetHashCode();
            if (SHA256 == null)
                SHA256 = SHA256.Create();
            var ValAsData = Encoding.UTF8.GetBytes(path);
            var key = SHA256.ComputeHash(ValAsData, 0, ValAsData.Length);
            return key;
            int step = 0;
            Int128 test = 0;
            foreach (byte b in key)
            {
                test += b | step++;
            }
            return  Encoding.UTF8.GetBytes(test.ToString());
        }
        public bool CheckToPrune(string Path)
        {
            //            var key = GetPathHash(Path);

            //var ret = Links.ContainsKey(key);
            //var ret = Links.cont
            bool ret = false;

            var info = new DirectoryInfo(Path);
            var key = GetPathHash(info.FullName);

            lock (Links)
            {
                //ret = Links.ContainsKey(key);
                ret = Links.Contains(Path);
                var self = Environment.CurrentManagedThreadId;
                if (ret)
                {
                    
                    if (ret)
                    {
                        Debug.WriteLine($"Thread: {{{Thread.CurrentThread.Name}}} ID {self} {Path} already in dup list");
                        return true;
                    }
                }

                Debug.WriteLine($"Thread: {{{Thread.CurrentThread.Name}}} ID {self}  Adding {Path} to the dup list. Current Count {Links.Count}");
                //         Links[key] = Path;
                Links.Add(Path);
                return false;

            }

        }

        public ConcurrentBag<string> Links = new();
     //   public volatile ConcurrentDictionary<byte[],string> Links = new();
    }
}

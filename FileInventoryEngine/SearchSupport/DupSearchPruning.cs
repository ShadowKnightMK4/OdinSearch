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
        public int GetPathHash(string path)
        {
            return path.GetHashCode();
            if (SHA256 == null)
                SHA256 = SHA256.Create();
            var ValAsData = Encoding.UTF8.GetBytes(path);
            var key = SHA256.ComputeHash(ValAsData, 0, ValAsData.Length);
           // return key;
        }
        public bool CheckToPrune(string Path)
        {
            //            var key = GetPathHash(Path);

            //var ret = Links.ContainsKey(key);
            //var ret = Links.cont
            bool ret = false;
            
                var info = new DirectoryInfo(Path);
                var key = GetPathHash(info.FullName);
                ret = Links.ContainsKey(key);
            

            if (ret)
            {
                Debug.WriteLine($"{Path} already in dup list");
                return true;
            }
            else
            {
                foreach (int key_item in  Links.Keys)
                {
                    if (Links[key_item].ToString() == Path)
                    {
                        Debug.WriteLine($"{Path} already in dup list");
                        return true;
                    }
                }
            }
            Debug.WriteLine($"Adding {Path} to the dup list");
            Links[key] = Path;
            return false;

            

        }

     //   ConcurrentBag<string> Links = new();
        public ConcurrentDictionary<int, string> Links = new();
    }
}

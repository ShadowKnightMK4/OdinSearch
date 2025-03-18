using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools
{
    /// <summary>
    /// Make from Bing.com/s gpt4o
    /// </summary>
    public class FolderSizeCataloger: OdinSearch_OutputConsumerBase
    {
            private readonly List<(string FolderPath, long Size)> folderSizes = new();

            public List<(string FolderPath, long Size)> FolderSizes => folderSizes;

            public override void Match(FileSystemInfo info)
            {
                if (info is DirectoryInfo directoryInfo)
                {
                    long size = GetDirectorySize(directoryInfo);
                    folderSizes.Add((directoryInfo.FullName, size));
                }
            }

            private long GetDirectorySize(DirectoryInfo directoryInfo)
            {
                return directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
            }

            public override bool HasPendingActions()
            {
                return false;
            }

        public override bool ResolvePendingActions()
        {
            // Implement any additional logic needed to finalize the cataloging process
            return true;
        }
    }
}

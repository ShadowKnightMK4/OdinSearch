using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks.Dataflow;

namespace OdinSearchEngine
{


    /// <summary>
    /// Wrapper for <see cref="FileInfo"/> and <see cref="DirectoryInfo"/> so we can get size info.
    /// </summary>
    public class FileInfoExtract
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetFileSizeEx(IntPtr Handle, out long LargeInt);
        public FileInfoExtract(string Target)
        {
            Content = new FileInfo(Target);
            
        }

        /// <summary>
        /// Creation Time
        /// </summary>
        public DateTime CreationTime
        {
            get
            {
                return Content.CreationTime;
            }
        }

        /// <summary>
        /// Last time file/folder was accessed
        /// </summary>
        public DateTime AccessTime
        {
            get
            {
                return Content.LastAccessTime;
            }
        }

        /// <summary>
        /// Last time the file/folder written too
        /// </summary>
        public DateTime LastWriteTime
        {
            get
            {
                return Content.LastWriteTime;
            }
        }
        /// <summary>
        /// Gets the fullname of the directory or file
        /// </summary>
        public string FullName
        {
            get
            {
                return Content.FullName;
            }
        }

        /// <summary>
        /// Get either the directory name or the file name by itself
        /// </summary>
        public string Name
        {
            get
            {
                return Content.Name;
            }
        }

        [OdinSearchSqlSkipAttrib]
        /// <summary>
        /// Return how big it is in bytes (0 for Directory)
        /// </summary>
        public long SizeBytes
        {
            get
            {
                if (Content.Attributes.HasFlag(FileAttributes.Directory))
                {
                    return 0;
                }
                else
                {
                    long filesize = -1;
                    if (OperatingSystem.IsWindows())
                    {
                        try
                        {
                            SizeContentWindows = File.OpenHandle(Content.FullName);
                            if (SizeContentWindows.IsInvalid == false)
                            {
                                if (!GetFileSizeEx(SizeContentWindows.DangerousGetHandle(), out filesize))
                                {
                                    filesize = -1;
                                }
                            }

                        }
                        finally
                        {
                            if (SizeContentWindows != null)
                            {
                                SizeContentWindows.Close();
                            }
                        }
                    }
                    if (filesize == -1)
                    {
                        SizeContainer = new FileInfo(FullName);
                        filesize = SizeContainer.Length;
                    }
                        return filesize;
                }
            }
        }

        [OdinSearchSqlSkipAttrib]
        /// <summary>
        /// Return how big it is in kilobytes (0 for Directory)
        /// </summary>
        public long SizeKB
        {
            get
            {
                long ret = SizeBytes;
                if (ret != 0)
                {
                    ret = (long)(ret * 0.001);
                }
                return ret;
            }
        }
        [OdinSearchSqlSkipAttrib]
        public long SizeMB
        {
            get
            {
                long ret = SizeBytes;
                if (ret != 0)
                {
                    ret = (long)(ret * 0.000001);
                }
                return ret;
            }
        }
        [OdinSearchSqlSkipAttrib]
        public long SizeGB
        {
            get
            {
                long ret = SizeMB;
                if (ret != 0)
                {
                    ret = (long)(ret * 0.001);
                }
                return ret;
            }
        }

        [OdinSqlPreFab_TypeGen("bit", OverrideName ="FileExists")]
        public bool Exists
        {
            get
            {
                return Content.Exists;
            }
        }

        public string ParentLocationPath
        {
            get
            {
                return Path.GetDirectoryName(FullName);
            }
        }

        public DirectoryInfo ParentLocation
        {
            get
            {
                return new DirectoryInfo(ParentLocationPath);
            }
        }

        public FileAttributes FileAttributes
        {
            get
            {
                return Content.Attributes;
            }
        }


        

        /// <summary>
        /// Allocate resourcs to compute hash for file. If Folder, always returns null
        /// </summary>
        public byte[] GetSha512()
        {
            bool OK = false;
            byte[] bytes;

                if (Content.Attributes.HasFlag(FileAttributes.Directory))
                {
                    return null;
                }
            using (SHA512 once = SHA512.Create())
            {
                using (var FN = File.OpenRead(Content.FullName))
                {
                    bytes = new byte[once.HashSize/8];
                    bytes = once.ComputeHash(FN);
                    OK = true;
                }
            }
            if (OK == true)
                return bytes;
            return null;
        }

        public void Refresh()
        {
            if (Content != null)
            {
                Content.Refresh();
            }
            if (SizeContainer != null)
            { 
                SizeContainer.Refresh();
            }

            Sha512Buffer = GetSha512();

            if (OperatingSystem.IsWindows())
            {

            }
        }

        /// <summary>
        /// First pass, cacluate hash, rest of passes return said result.  To Update Call Refresh() or call <see cref="GetSha512"/> to get more recent hash
        /// </summary>
        public byte[] Sha512Hash
        {
            get
            {
                if (Sha512Buffer == null)
                {
                    Sha512Buffer = GetSha512();
                }
                return Sha512Buffer;
            }
        }

        byte[] Sha512Buffer;
        

        SafeFileHandle SizeContentWindows;
        /// <summary>
        /// common container for the file or folder we're there for.
        /// </summary>
        FileSystemInfo Content;
        /// <summary>
        /// Pretty much there just for getching file size's
        /// </summary>
        FileInfo SizeContainer;
    }
    
}

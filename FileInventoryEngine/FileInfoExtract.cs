using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace OdinSearchEngine
{


    /// <summary>
    /// Wrapper for <see cref="FileInfo"/> and <see cref="DirectoryInfo"/> so we can get size info.
    /// </summary>
    public class FileInfoExtract
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetFileSizeEx(IntPtr Handle, out long LargeInt);
        public FileInfoExtract(string Target)
        {
            Content = new FileInfo(Target);
            
        }

        public string FullName
        {
            get
            {
                return Content.FullName;
            }
        }

        public string Name
        {
            get
            {
                return Content.Name;
            }
        }

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
            if (OperatingSystem.IsWindows())
            {

            }
        }

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

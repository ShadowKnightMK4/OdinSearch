using System;
using System.IO;
namespace FileInventoryEngine
{
    /// <summary>
    /// Wrapper for <see cref="FileInfo"/> and <see cref="DirectoryInfo"/> so we can get size info.
    /// </summary>
    public class FileInfoExtract
    {
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
                    SizeContainer = new FileInfo(FullName);
                    return SizeContainer.Length;
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
        }

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

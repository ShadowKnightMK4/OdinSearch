using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonSqlUnitTests.OlderTests.TestTools

{
    /// <summary>
    /// folder / file cache to purge things created as needed for the unit tests. Disospoe() or Purge() triggers cleanup
    /// </summary>
    sealed class FileItemCache : IDisposable
    {
        public static void EnsureFolderExists(string target)
        {
            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }
        }
        public FileItemCache()
        {
        }

        public FileItemCache(string TestRootLocation, string TestClassName)
        {
            EnsureFolderExists(Path.Combine(TestRootLocation, TestClassName));
        }

        /// <summary>
        /// Any item added to this is set to file attribute normal before deletion.
        /// </summary>
        public List<FileSystemInfo> Items = new();
        public void Purge()
        {
            List<FileSystemInfo> Folders = new();
            List<FileSystemInfo> FileDeletes = new();
            foreach (FileSystemInfo item in Items)
            {
                if (item.Attributes.HasFlag(FileAttributes.Directory) == false)
                {
                    File.SetAttributes(item.FullName, FileAttributes.Normal);
                    FileDeletes.Add(item);
                }
                else
                {
                    Folders.Add(item);
                }
            }

            foreach (FileSystemInfo item in FileDeletes)
            {
                File.Delete(item.FullName);
            }


            foreach (FileSystemInfo item in Folders)
            {
                try
                {
                    Directory.Delete(item.FullName, true);
                }
                catch (DirectoryNotFoundException)
                {
                    // it's probably fine.  Its possible it was just deleted by another delete. The scrub folder location should also be deleted at class cleanup automatically too by unit test classes
                }
            }
        }
        #region Creation Tools
        /// <summary>
        /// Make a folder and add it our  list of cleanup items
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        public void MakeFolder(string path1, string path2)
        {
            MakeFolder(path1, path2, FileAttributes.Directory);
        }

        /// <summary>
        /// Make a folder and add it our  list of cleanup items
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <param name="attributes">set folder's attributes</param>
        public void MakeFolder(string path1, string path2, FileAttributes attributes)
        {
            string targ = Path.Combine(path1, path2);
            Directory.CreateDirectory(targ);

            File.SetAttributes(targ, attributes);
            Items.Add(new DirectoryInfo(targ));

        }

        /// <summary>
        /// Make a file and set its attributes. Sets length to 0
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <param name="attributes"></param>
        public void MakeFile(string path1, string path2, FileAttributes attributes)
        {
            MakeFile(path1, path2, attributes, 0);
        }

        /// <summary>
        /// Make a file, set attributes amnd length
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <param name="attributes"></param>
        /// <param name="len"></param>
        public void MakeFile(string path1, string path2, FileAttributes attributes, int len)
        {
            string targ = Path.Combine(path1, path2);
            using (var fn = File.OpenWrite(targ))
            {
                fn.SetLength(len);
            }
            File.SetAttributes(targ, attributes);
            Items.Add(new FileInfo(targ));
        }


        /// <summary>
        /// If this is a file or folder, it's deleted at dispoe
        /// </summary>
        /// <param name="Location"></param>
        public void AddItem(string Location)
        {

        }
        void IDisposable.Dispose()
        {
            Purge();
            Items.Clear();
        }
        #endregion
    }
}

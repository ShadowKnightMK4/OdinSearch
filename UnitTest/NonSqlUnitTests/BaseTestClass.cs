using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NonSqlUnitTests
{
    /// <summary>
    /// took the idea of the <see cref="FileItemCache"/> class and ran with it.
    /// </summary>
    
    public class CommonFileTestClass
    {
        /// <summary>
        /// set this to were we force all created files/folders to based off the <see cref="RootBath"/>
        /// </summary>
        protected readonly bool ForceRootPath = true;
        /// <summary>
        /// The base location we make files/folders off off. Note: For testing, we should be able to read and write to this folder (and any sub folders)
        /// </summary>
        protected string RootBath = "C:\\Scratchpad";

        /// <summary>
        /// list of things created by <see cref="MakeAFile(string, FileAttributes)"/> and <see cref="MakeAFolder(string, FileAttributes)"/>
        /// </summary>
        protected List<FileSystemInfo> fileSystemInfos;

        
        /// <summary>
        /// Anitialize this based off the passed type. Note type name should be a value system folder name
        /// </summary>
        /// <param name="T"></param>
        public void SetupState(Type T)
        {
            string myroot = Path.Combine(RootBath, T.Name);
            fileSystemInfos = new();
            Directory.CreateDirectory(myroot);
            RootBath = myroot;
        }


        /// <summary>
        /// Purge any created off the created path.
        /// </summary>        
        public void CleanState()
        {
            List<DirectoryInfo> DINFO = new();
            List<FileInfo> FINFO = new();
            foreach (FileSystemInfo info in fileSystemInfos)
            {
                if (info.Attributes.HasFlag(FileAttributes.Directory))
                    DINFO.Add(info as DirectoryInfo);
                else
                    FINFO.Add(info as FileInfo);
            }

            foreach (FileInfo Entry in FINFO)
            {
                if (Entry.Exists)
                {
                    Entry.Attributes = FileAttributes.Normal;
                    Entry.Delete();
                }
            }

            foreach (DirectoryInfo Entry in DINFO)
            {
                if (Entry.Exists)
                {
                    Entry.Attributes = FileAttributes.Directory;
                    Entry.Delete(true);
                }
            }

            if (Directory.Exists(RootBath))
                Directory.Delete(RootBath);
        }

        /// <summary>
        /// assist to autosplie a path with the required root path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string SplicePath(string path)
        {
            return Path.Combine(RootBath, path);
        }
        /// <summary>
        /// make a folder in the cache location with the passed attributes
        /// </summary>
        /// <param name="FolderName"></param>
        /// <param name="Attrib"></param>
        /// <exception cref="InvalidDataException"></exception>
        public void MakeAFolder(string FolderName, FileAttributes Attrib)
        {
            if (ForceRootPath)
                FolderName = SplicePath(FolderName);
            if (FolderName.Contains(RootBath) == false)
            {
                throw new InvalidDataException($"Must be a subfolder off of {RootBath}");
            }
            var item = Directory.CreateDirectory(FolderName);
            item.Attributes = Attrib;
            fileSystemInfos.Add(item);
        }

        /// <summary>
        /// make a folder in the cache location with the passed attributes
        /// </summary>
        /// <param name="FolderNamePart"></param>
        /// <param name="FolderNamePart2"></param>
        /// <param name="Attrib"></param>

        public void MakeAFolder(string FolderNamePart, string FolderNamePart2, FileAttributes Attrib) 
            {
                MakeAFolder(Path.Combine(FolderNamePart, FolderNamePart2), Attrib); 
            }


        /// <summary>
        /// make a file with the passed attributei in the cache location 
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Attrib"></param>
        /// <exception cref="InvalidDataException"></exception>
        public void MakeAFile(string FileName, FileAttributes Attrib)
            {

            if (ForceRootPath)
                FileName = SplicePath(FileName);
            if (FileName.Contains(RootBath) == false)
            {
                throw new InvalidDataException($"Must be a file located off this {RootBath}");
            }

            using (var finger = File.OpenWrite(FileName))
                {
                }
                FileInfo fn = new FileInfo(FileName);
                fn.Attributes = Attrib;

                fileSystemInfos.Add(fn);
            }
    }
}

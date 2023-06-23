﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using System.Diagnostics;
using static OdinSearchEngine.OdinSearch;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace OdinSearchEngine.OdinSearch_ContainerSystems
{
    
    
    /// <summary>
    /// A File contained within a zip file
    /// </summary>
    public class OdinSearch_ZippedFileInfo: OdinSearch_FileInfoGeneric
    {
        ZipArchive arch;
        ZipArchiveEntry entry;
        string SubContainerPart;
        FileInfo ArchProbe;

        /// <summary>
        /// Callback used by OdinSeach to ask if this handler can deal with zip files.
        /// </summary>
        /// <param name="location">locaiton of file item in question, may contain path like C:\\Zipthing.zip\\containedfile.dat.  <see cref="GetFileSystemLocation"/></param>
        /// <returns></returns>
        protected static Type CallbackHandler(string location)
        {
            bool Ok = false;
            FileInfo ziproot = GetFileSystemLocation(location);
            if (ziproot == null)
            {
                return null;
            }
            else
            {
                try
                {
                    using (Stream str = File.OpenRead(ziproot.FullName))
                    {
                        using (ZipArchive Arch = new ZipArchive(str, ZipArchiveMode.Read, true))
                        {
                            Ok = true;
                        }
                    }
                }
                catch (IOException)
                {
                    Ok = false;
                }
                catch (InvalidDataException)
                {
                    Ok = false;
                }

                if (Ok)
                {
                    return typeof(OdinSearch_ZippedFileInfo);
                }
                return null;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        public static ContainerCheckFileCallback CallbackChecker { get { return CallbackHandler; } }
        /// <summary>
        /// This tests if the entry is null and if not, gets the file the subcontainer part points too
        /// </summary>
        protected void SetArchivePointer()
        {
            if (entry == null)
                entry = arch.GetEntry(SubContainerPart);
        }

        /// <summary>
        /// Make an instance of this class pointing to the zip file and sub location
        /// </summary>
        /// <param name="location"></param>
        public OdinSearch_ZippedFileInfo(string location):base(location)
        {
            ArchProbe = GetFileSystemLocation(location);
            if (ArchProbe != null )
            {
                arch = new ZipArchive(File.Open(ArchProbe.FullName, FileMode.Open), ZipArchiveMode.Update);
                SubContainerPart = location.Substring(ArchProbe.FullName.Length+1);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (arch!= null)
            {
                arch.Dispose();
                arch = null;
            }
            base.Dispose(disposing);
        }
        public override void Delete()
        {
            throw new NotImplementedException();
        }
        public override FileAttributes Attributes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string ContainerLocation
        {
            get
            {
                return ArchProbe.FullName;
            }
        }
        public override bool Exists
        {
            get
            {
                SetArchivePointer();
                return entry != null;
            }
        }
        public override string FullName
        {
            get
            {
                return SubContainerPart;
            }
        }
        public override long Length
        {
            get
            {
                SetArchivePointer();
                if (entry != null)
                {
                    return entry.Length;
                }
                return 0;
            }
        }

        public override long GetEncodedLength
        {
            get
            {
                SetArchivePointer();
                if (entry != null)
                {
                    return entry.CompressedLength;
                }
                return 0;
            }
        }


        public override string Name
        {
            get
            {
                SetArchivePointer();
                if (entry != null)
                {
                    return entry.Name;
                }
                return null;
            }
        }


    }

    /// <summary>
    /// A folder contained within a zip file
    /// </summary>
    public class OdinSearch_ZippedDirectoryInfo: OdinSearch_DirectoryInfoGeneric
    {
        
        public override FileAttributes Attributes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string ContainerLocation => throw new NotImplementedException();
        public override void Delete()
        {
            throw new NotImplementedException();
        }
        public override bool Exists => throw new NotImplementedException();
        public override string Name => throw new NotImplementedException();
        public override OdinSearch_DirectoryInfoGeneric Parent => throw new NotImplementedException();
        public override OdinSearch_DirectoryInfoGeneric Root => throw new NotImplementedException();

        public override OdinSearch_DirectoryInfoGeneric[] GetDirectories()
        {
            throw new NotImplementedException();
        }

        public override OdinSearch_FileInfoGeneric[] GetFiles()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// handle thing to the zxip wer'e going
        /// </summary>
        protected ZipArchive Arch = null;
        protected string ContainerSubPath = null;
        protected override void Dispose(bool disposing)
        {
            if (Arch != null)
                Arch.Dispose();
            base.Dispose(disposing);
        }

        public OdinSearch_ZippedDirectoryInfo(string location):base(location) {
            FileInfo ArchProbe = GetFileSystemLocation(location);
            if (ArchProbe == null)
            {
                throw new FileNotFoundException(location);
            }
            Arch = new ZipArchive(File.Open(ArchProbe.FullName, FileMode.Open), ZipArchiveMode.Update);
            ContainerSubPath = location.Substring(ArchProbe.FullName.Length);
        }
        public override long Length
        {
            get
            {
                return 0;
            }
        }
        public override long GetEncodedLength
        {
            get
            {
                return 0;
            }
        }
        public override string FullName
        {
            get
            {
                return ContainerSubPath;
            }
        }
        
    }

}


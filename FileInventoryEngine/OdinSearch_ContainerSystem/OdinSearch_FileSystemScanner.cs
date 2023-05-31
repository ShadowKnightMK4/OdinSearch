using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Metadata.Ecma335;

namespace OdinSearchEngine.OdinSearch_ContainerSystems
{

    /// <summary>
    /// The Wrapper object for <see cref="OdinSearch_LocalFileSystem"/>.
    /// </summary>
    public class OdinSearch_FileSystemItem: OdinSearch_ContainerSystemItem
    {
        public OdinSearch_FileSystemItem(object Source): base(Source)
        {
            
        }
        public override string Name 
        {
            get
            {
                return Info.Name;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }
        public override DateTime CreationTime
        {
            get
            {
                return Info.CreationTime;
            }
            set
            {
                Info.CreationTime = value;
            }
        }

        public override DateTime CreationTimeUTC
        {
            get
            {
                return Info.CreationTimeUtc;
            }
            set
            {
                Info.CreationTimeUtc = value;
            }
        }

        public override bool Exists
        {
            get { return Info.Exists; }
        }

        public override string Extension
        {
            get
            {
                return Info.Extension;
            }
        }

        public override string FullName
        {
            get
            {
                return Info.FullName;
            }
        }


        public override DateTime LastAccessTimeUtc 
        {
            get
            {
                return Info.LastAccessTimeUtc;
            }
            set
            {
                Info.LastAccessTimeUtc = value;
            }
        }

        public override DateTime LastAccessTime
        {
            get
            {
                return Info.LastAccessTime;
            }
            set
            {
                Info.LastWriteTime = value;
            }
        }

        public override DateTime LastWriteTime
        {
            get
            {
                return Info.LastWriteTime;
            }
            set
            {
                Info.LastWriteTime = value;
            }
        }

        public override DateTime LastWriteTimeUtc
        {
            get
            {
                return Info.LastWriteTimeUtc;
            }
            set
            {
                Info.LastWriteTimeUtc = value;
            }
        }

        public override string LinkTarget
        {
            get
            {
                return Info.LinkTarget;
            }
        }
        

        protected FileSystemInfo Info => (FileSystemInfo)Source;
    }


    /// <summary>
    /// Used for local file systems. It's wrapper class is <see cref="OdinSearch_FileSystemItem"/>
    /// </summary>
    public class OdinSearch_LocalFileSystem: OdinSearch_ContainerGeneric
    {
        public override string[] GetContainerFiles(string Location)
        {
            return Directory.GetDirectories(Location);
        }
        public override string[] GetFiles(string Location)
        {
            
            return Directory.GetFiles(Location);
        }



        public override OdinSearch_ContainerSystemItem MakeInstance(string Name)
        {
            
            return new OdinSearch_FileSystemItem(new FileInfo(Name));
        }


    }
}

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
    public class OdinSearch_ContainerFileInfo: OdinSearch_FileInfoGeneric
    {
        FileInfo Cache = null;
        public OdinSearch_ContainerFileInfo(string location): base(location)
        {

        }
        public override FileAttributes Attributes 
        {
            get
            {
                return File.GetAttributes(Location);
            }
            set
            {
                File.SetAttributes(Location, value);
            }
        }

        public override string ContainerLocation
        {
            get
            {
                return FullName;
            }
        }
        public override bool Exists
        {
            get
            {
                return File.Exists(Location);
            }
        }

        public override string FullName
        {
            get
            {
                return Location;
            }
        }

        public override long Length
        {
            get
            {
                Cache = new FileInfo(Location);
                return Cache.Length;
            }
        }

        public override void Delete()
        {
            File.Delete(Location);
        }

        public override string Name
        {
            get
            {
                return Path.GetFileName(Location);
            }
            
        }
    }
    public class OdinSearch_ContainerDirectoryInfo : OdinSearch_DirectoryInfoGeneric
    {
        public override OdinSearch_DirectoryInfoGeneric Root
        {
            get
            {
                
                return new OdinSearch_ContainerDirectoryInfo(Directory.GetDirectoryRoot(Location));
            }
        }
        public override void Delete()
        {
            Directory.Delete(Location);
        }
        public OdinSearch_ContainerDirectoryInfo(string location) : base(location)
        {

        }
        FileSystemInfo Cache=null;
        
        public override bool Exists
        {
            get
            {
                return Directory.Exists(Location);
            }
        }

        public override FileAttributes Attributes 
        {
            get 
           {
                Cache = new FileInfo(Location);
                return Cache.Attributes;
            } 
            set 
            {
                Cache = new FileInfo(Location);
                Cache.Attributes = value;
            } 
        }

        public override string FullName
        {
            get
            {
                Cache= new FileInfo(Location);
                return Cache.FullName;
            }
        }

        public override long Length
        {
            get
            {
                return 0;
            }
        }

        public override string Name
        {
            get
            {
                Cache = new FileInfo(Location);
                return Cache.Name;
            }
        }

        public override OdinSearch_DirectoryInfoGeneric Parent
        {
            get
            {
                return new OdinSearch_ContainerDirectoryInfo(Path.GetDirectoryName(Location));
            }
        }
        public override string ContainerLocation
        {
            get
            {
                return FullName;
            }
        }
        public override OdinSearch_DirectoryInfoGeneric[] GetDirectories()
        {
            Cache = new FileInfo(Location);
            List<OdinSearch_DirectoryInfoGeneric> ret = new();
            var Passthru = Directory.GetDirectories(Cache.FullName);
            foreach ( var p in Passthru )
            {
                ret.Add(new OdinSearch_ContainerDirectoryInfo(p));
            }
            return ret.ToArray();
        }

        public override OdinSearch_FileInfoGeneric[] GetFiles()
        {
            Cache = new FileInfo(Location);
            List<OdinSearch_FileInfoGeneric> ret = new();
            var Passthru = Directory.GetDirectories(Cache.FullName);
            foreach (var p in Passthru)
            {
                ret.Add(new OdinSearch_ContainerFileInfo(p));
            }
            return ret.ToArray();
        }
    }
}

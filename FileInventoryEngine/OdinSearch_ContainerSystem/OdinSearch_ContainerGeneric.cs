using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OdinSearchEngine.OdinSearch_ContainerSystems
{

    /*
     * MiTM class must work just like fileinfo and directoryinfo.
     * We add a new thing called container that specifies where the file is located
     *      for default, same as fullname
     *      for zip files, zip location,
     *      for vhx mounted,  original vhx location a
     *      and so on.
     * 
     */
    
        public abstract class OdinSearch_FileInfoGeneric: IDisposable
        {
        /// <summary>
        /// Generial constructor
        /// </summary>
        /// <param name="location">location to read /deal with</param>
        protected OdinSearch_FileInfoGeneric(string location) { this.Location = location; }

        #region properties
        /// <summary>
        /// Get the name of this file or directy that 
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Get the full location of this file or directory. For containers, this is from the perspective that the top level part is the root drive
        /// </summary>
            public abstract string FullName { get; }
        /// <summary>
        /// Does this file or directory exist
        /// </summary>
            public abstract bool Exists { get; }
        /// <summary>
        /// Get the length of this file unencodinged. Directory always returns 0.
        /// </summary>
            public abstract long Length { get; }

        /// <summary>
        /// If the file is stored on disk in a different format/encoding (for example compression), return the compressed size. Directory returns 0
        /// </summary>
            public abstract long GetEncodedLength { get; }
        /// <summary>
        /// Get or set this file/directory's attributes
        /// </summary>
            public abstract FileAttributes Attributes { get; set; }
        /// <summary>
        /// Delete this file or directory. Note directory should be empty.
        /// </summary>
            public abstract void Delete();

        /// <summary>
        /// For Files/folders contained within things like zip files, full path to the file. For everything else, same as <see cref="FullName"/>
        /// </summary>
        public abstract string ContainerLocation { get; }
        /// <summary>
        /// location on the location file system where this is located.
        /// </summary>
        protected string Location;
        private bool disposedValue;
        #endregion

        /// <summary>
        /// This routine is what is used to reverse traill a zip path until we get  to the zip file
        /// </summary>
        /// <param name="path">Such as C:\\Something\fun.zip\\specialbooks\\thebestthingever.pdf.</param>
        /// <returns></returns>
        /// <example>For the path example "C:\\Something\fun.zip\\specialbooks\thebestthingever.pdf." this should return an instance of FileInfo pointing to fun.zip</example>
        protected static FileInfo GetFileSystemLocation(string path)
        {
            while (path != null)
            {
                if (Directory.Exists(path) || File.Exists(path))
                {
                    return new FileInfo(path);
                }
                else
                {
                    path = Path.GetDirectoryName(path);
                }
            }
            return null;
        }
        protected string GetHostPart(string path)
        {
            Uri uri= new Uri(path);
            return uri.Host;
        }

        protected string GetNetworkPart(string path)
        {
            Uri uri= new Uri(path);
            return uri.AbsolutePath;
        }

        protected string GetSharePart(string path)
        {
            Uri uri= new Uri(path);
            return uri.AbsolutePath.Trim('/');
        }

        protected string GetLocalPart(string path)
        {
            Uri uri = new Uri(path);
            return uri.LocalPath;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~OdinSearch_FileInfoGeneric()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public abstract class OdinSearch_DirectoryInfoGeneric :OdinSearch_FileInfoGeneric
    {
        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="location">should always be the phyisical file or folder location.</param>
        public OdinSearch_DirectoryInfoGeneric(string location) : base(location)
        {

        }
        /// <summary>
        /// Get a DirectoryInfo one level above this
        /// </summary>
        public abstract OdinSearch_DirectoryInfoGeneric Parent { get; }

        /// <summary>
        /// Get the root location of this
        /// </summary>
        public abstract OdinSearch_DirectoryInfoGeneric Root { get; }

        /// <summary>
        /// Get the files in this location
        /// </summary>
        /// <returns></returns>
        public abstract OdinSearch_FileInfoGeneric[] GetFiles();

        /// <summary>
        /// Get the directories in this location
        /// </summary>
        /// <returns></returns>
        public abstract OdinSearch_DirectoryInfoGeneric[] GetDirectories();
        
    }

}


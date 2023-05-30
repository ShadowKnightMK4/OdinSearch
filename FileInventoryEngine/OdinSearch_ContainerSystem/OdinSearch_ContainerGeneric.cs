using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OdinSearchEngine.OdinSearch_FileSystemScanner
{

    /// <summary>
    /// The base item the container classes deal with.
    /// </summary>
    public abstract class OdinSearch_ContainerSystemItem
    {
        public OdinSearch_ContainerSystemItem(object Source)
        {
            this.Source = Source;
        }
        public abstract string FullName { get; }
        public abstract string? LinkTarget { get; }
        public abstract DateTime LastWriteTimeUtc { get; set; }
        public abstract DateTime LastWriteTime { get; set; }
        public abstract DateTime LastAccessTimeUtc { get; set; }
        public abstract DateTime LastAccessTime { get; set; }
        public abstract string Extension { get;  }
        public abstract bool Exists { get; }
        public abstract DateTime CreationTimeUTC { get; set; }
        public abstract DateTime CreationTime { get; set; }
        public abstract string Name { get; set; }

        /// <summary>
        /// Get the object the properties come from.  It'll be container specific.
        /// </summary>
        protected object Source;
    }

    /// <summary>
    /// Prototype for the container class handlers.
    /// </summary>
    public abstract class OdinSearch_ContainerGeneric: IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// Get a list of strings from this location that can contain others.
        /// </summary>
        /// <param name="Location"></param>
        /// <returns></returns>
        /// <remarks>If you're doing a local drive. Think <see cref="Directory.GetDirectories(string)"/></remarks>
        public abstract string[] GetContainerFiles(string Location);
        /// <summary>
        /// Get a list of strings from this location that can contain others.
        /// </summary>
        /// <param name="Location"></param>
        /// <returns></returns>
        /// <remarks>If you're doing a local drive. Think <see cref="Directory.GetFiles(string)"/></remarks>
        public abstract string[] GetFiles(string Location);

        /// <summary>
        /// make an instance of the named file system item and return it having the wrapper to deal with it. 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>Returns a subclass of <see cref="OdinSearch_ContainerSystemItem"/> that deals with abstracting away how to interact with the item in a <see cref="System.IO.FileSystemInfo"/> like way</returns>
        public abstract OdinSearch_ContainerSystemItem MakeInstance(string Name);

        /// <summary>
        /// Default dipose does not need to actual do this. This is for subclasses
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

                disposedValue = true;
            }
        }

        
        ~OdinSearch_ContainerGeneric()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}

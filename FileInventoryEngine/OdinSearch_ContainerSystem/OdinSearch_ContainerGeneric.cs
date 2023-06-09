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
    
        public abstract class OdinSearch_FileInfoGeneric
        {
        /// <summary>
        /// Generial constructor
        /// </summary>
        /// <param name="location">location to read /deal with</param>
        protected OdinSearch_FileInfoGeneric(string location) { this.Location = location; }

        /// <summary>
        /// Get the name of this file or directory
        /// </summary>
            public abstract string Name { get; }
        /// <summary>
        /// Get the full location of this file or directory
        /// </summary>
            public abstract string FullName { get; }
        /// <summary>
        /// Does this file or directory exist
        /// </summary>
            public abstract bool Exists { get; }
        /// <summary>
        /// Get the length of this file. Directory always returns 0.
        /// </summary>
            public abstract long Length { get; }
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
        }

    public abstract class OdinSearch_DirectoryInfoGeneric :OdinSearch_FileInfoGeneric
    {
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


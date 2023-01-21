using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace FileInventoryEngine
{
    /// <summary>
    /// SearchAncher is a starting location to search from and contains if the we're be enumating sub folders too.
    /// </summary>
    public class SearchAnchor
    {
        /// <summary>
        /// Make instance defaulting to all attached local mounted drives that are ready.
        /// </summary>
        public SearchAnchor()
        {
            DriveInfo[] Contents = DriveInfo.GetDrives();
            
            
            foreach (DriveInfo D in Contents)
            {
                if (D.IsReady)
                    roots.Add(D.RootDirectory);
            }
        }


        /// <summary>
        /// Make instance with this as the location start.  
        /// </summary>
        /// <param name="AnchorLocation"></param>
        /// <exception cref="IOException">This can be thrown if the passed location is offline/not ready.</exception>
        public SearchAnchor(string AnchorLocation)
        {
            DirectoryInfo ReadyTest = new DirectoryInfo(AnchorLocation);
            if (ReadyTest.Attributes.HasFlag( FileAttributes.Offline) )
            {
                throw new IOException(nameof(AnchorLocation) + " is offline.");
            }
            if (ReadyTest.Attributes.HasFlag(FileAttributes.Directory))
            {
                roots.Add(ReadyTest);
            }
            else
            {
                roots.Add(new DirectoryInfo(ReadyTest.Parent.FullName));
            }
            
        }

        /// <summary>
        /// Add this drive to our list if not already there.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>

        public bool AddAnchor(DriveInfo d)
        {
            bool already_there = false;
            if (d.IsReady)
            {
                roots.ForEach( p => { if (p.Name == (d.VolumeLabel)) { already_there = true; } } );
            }
            if (!already_there)
            {
                roots.Add(d.RootDirectory);
                return true;
            }
            return false;
        }

        /// <summary>
        /// if you set this, the anchor root subfolders will be enumerated.
        /// </summary>
        public bool EnumSubFolders
        {
            get
            {
                return EnumSubFoldersValue;
            }
            set
            {
                EnumSubFoldersValue = value;
            }
        }

        /// <summary>
        /// Container value for <see cref="EnumSubFolders"/>
        /// </summary>
        private bool EnumSubFoldersValue = EnumSubFoldersDefault;
        


        /// <summary>
        /// Specify the default value for <see cref="EnumSubFolders"/> when you create an instance. Note this effects every instance you create after setting it
        /// </summary>
        public static bool EnumSubFoldersDefault { get; set; }
        /// <summary>
        /// Each root location will end up getting a seperate worker thread searching with the parameters.
        /// </summary>
        public readonly List<DirectoryInfo> roots = new List<DirectoryInfo>();
    }
}

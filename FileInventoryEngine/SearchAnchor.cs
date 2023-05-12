using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace OdinSearchEngine
{
    /// <summary>
    /// SearchAnchor is a possible starting location to search from.  
    /// 
    /// Specs:
    /// SearchAnchor() Constructor needs to get all local drives that are online as default
    /// 
    /// SearchAnchor(string location)  Needs to throw IoException on offline location
    /// 
    /// 
    /// AddAnchor(string location)  Don't add enchor already there.  TEst for online 
    /// 
    /// </summary>
    public class SearchAnchor
    {
        /// <summary>
        /// Make an instance based on an existing other isntance
        /// </summary>
        /// <param name="other">Another SearchAnchor</param>
        /// <param name="DiscardRoot">if true, we discard the default root for the new instance before adding the other one.</param>
        public SearchAnchor(SearchAnchor other, bool DiscardRoot)
        {
            if (!DiscardRoot)
            {
               roots.AddRange(other.roots);
            }
            
            this.EnumSubFolders = other.EnumSubFolders;
        }

        
        /// <summary>
        /// Either make an instance of this without any entries in the root or if WantLocalDrives is true, all local online drives
        /// </summary>
       /// <param name="WantLocalDrives">If true, we get a list of all local mounted drives and add if they're noted as ready in in the System</param>
        public SearchAnchor(bool WantLocalDrives)
        {
            if (WantLocalDrives)
            {
                AddLocalDrivesToRoot_OnlyReady();
            }
        }
        /// <summary>
        /// Make instance defaulting to all attached local mounted drives that are ready.
        /// </summary>
        public SearchAnchor()
        {
            AddLocalDrivesToRoot_OnlyReady();
        }


        /// <summary>
        /// Make instance with this as the location start.  
        /// </summary>
        /// <param name="AnchorLocation">start location</param>
        /// <exception cref="IOException">This can be thrown if the passed location is offline/not ready.</exception>
        public SearchAnchor(string AnchorLocation)
        {
            AddAnchor(AnchorLocation);            
        }

        /// <summary>
        /// Common Route for a few of the public addanchor routes
        /// </summary>
        /// <param name="Location">Location to add</param>
        /// <param name="DupCheck">Do not add if the Location is already in the list</param>
        /// <returns>returns true if added and false if Not added.  Note that if DupCheck is false, returns true always</returns>
        /// <exception cref="ArgumentNullException">thrown if Location is null</exception>
        /// <exception cref="IOException">thrown if Location is Offline</exception>
        private bool AddAnchorCommonRoute(DirectoryInfo Location, bool DupCheck)
        {
            if (Location == null)
            {
                throw  new ArgumentNullException(nameof(Location));
            }
            else
            {
                if (Location.Attributes.HasFlag(FileAttributes.Offline))
                {
                    throw new IOException(nameof(Location) + " is offline. Unable to add to our list");
                }


                if (Location.Attributes.HasFlag(FileAttributes.Directory))
                {
                    if (DupCheck)
                    {
                        if (roots.Contains(Location))
                        {
                            return false;
                        }
                    }
                    roots.Add(Location);
                    return true;
                }
                else
                {
                    var AddThis = new DirectoryInfo(Location.Parent.FullName);
                    if (DupCheck)
                    {
                        if (roots.Contains(AddThis))
                        {
                            return false;
                        }
                    }
                    roots.Add(AddThis);
                    return true;
                }
            }

        }
        /// <summary>
        /// Add this location to the root locations to search
        /// </summary>
        /// <param name="location">location to add. </param>
        /// <returns>returns true if it was added ok.</returns>
        /// <exception cref="IOException">can be thrown if the location is not ready or bu</exception>
        /// <exception cref="ArgumentNullException">Thrown if location is null</exception>
        /// <exception cref="ArgumentException">Thrown if for some other reason this routine cant make instance of <see cref="DirectoryInfo"/> pointing to your location</exception>
        /// <exception cref="PathTooLongException">Thrown if this routine can't make a  <see cref="DirectoryInfo"/> due to path being too long</exception>
        /// <exception cref="System.Security.SecurityException"> Thrown if <see cref="DirectoryInfo"/></exception> throw it
        public bool AddAnchor(string location)
        {
            DirectoryInfo ReadyTest = new DirectoryInfo(location);
            return AddAnchorCommonRoute(ReadyTest, true);
        }

        /// <summary>
        /// Add this drive to our list if not already there.
        /// </summary>
        /// <param name="d"></param>
        /// <returns>true if it was added and false if it was not due to duplicate</returns>

        public bool AddAnchor(DriveInfo d)
        {
            bool already_there = false;
            if (d.IsReady)
            {
                roots.ForEach( p => { if (p.Name == (d.Name)) { already_there = true; } } );
            }
            if (!already_there)
            {
                AddAnchorCommonRoute(d.RootDirectory, false); // DUP already checked
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
        /// The constructors <see cref="SearchAnchor()"/> and <see cref="SearchAnchor(true)"/> do there thing by calling this.
        /// </summary>
        protected void AddLocalDrivesToRoot_OnlyReady()
        {
            DriveInfo[] Contents = DriveInfo.GetDrives();


            foreach (DriveInfo D in Contents)
            {
                if (D.IsReady)
                    roots.Add(D.RootDirectory);
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

        /// <summary>
        /// Create an array of duplicate <see cref="SearchAnchor"/>s where each SearchAnchor has once of the roots
        /// </summary>
        /// <returns>an array where search SearchAnchor Entry in it contains just one root and the settings for SearchAnchor they were created from</returns>
        public SearchAnchor[] SplitRoots()
        {
            SearchAnchor[] ret = new SearchAnchor[roots.Count];
            for (int step = 0; step < ret.Length;step++)
            {
                ret[step] = new SearchAnchor(this, true);
                ret[step].roots.Add(roots[step]);
            }

            return ret;
        }
    }
}

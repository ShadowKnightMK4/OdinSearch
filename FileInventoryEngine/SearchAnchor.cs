using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.ExceptionServices;

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
    public class SearchAnchor : IEquatable<SearchAnchor>
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
            
            EnumSubFolders = other.EnumSubFolders;
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
        /// Make instance with this as the location start.  Use ';' to seperate multiple ones. 
        /// </summary>
        /// <param name="AnchorLocation">start location</param>
        /// <exception cref="IOException">This can be thrown if the passed location is offline/not ready/not existing</exception>
        public SearchAnchor(string AnchorLocation)
        {
            AddAnchor(AnchorLocation);            
        }

        /// <summary>
        /// Common Route for a few of the public AddAnchor routines
        /// </summary>
        /// <param name="Location">Location to add. Can't be null.</param>
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
        /// Add this collection of locations as anchors.
        /// </summary>
        /// <param name="Locations">Enumerator of a string collection</param>
        /// <returns>true if everything was added and false if something was not able to be added</returns>
        public bool AddAnchor(IEnumerable<DirectoryInfo> Locations)
        {
            foreach (DirectoryInfo I in Locations)
            {
                if (!AddAnchorCommonRoute(I, true))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Add this collection of locations as anchors.
        /// </summary>
        /// <param name="Locations">Enumerator of a string collection</param>
        /// <returns>true if everything was added and false if something was not able to be added</returns>
        public bool AddAnchor(IEnumerable<string> Locations)
        {
            foreach (string Spot in Locations)
            {
                if (!AddAnchor(Spot))
                {
                    return false;
                }
            }
            return true;
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
            if (!location.Contains(";"))
            {
                DirectoryInfo ReadyTest = new DirectoryInfo(location);
                return AddAnchorCommonRoute(ReadyTest, true);
            }
            else
            {
                string[] parts = location.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < parts.Length;i++)
                {
                    if (!AddAnchorCommonRoute(new DirectoryInfo(parts[i]), true))
                    {
                        return false;
                    }
                }
                return true;
            }
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

        #region xml based constants
        /// <summary>
        /// toplevel xml thing
        /// </summary>
        const string DocumentBaseXMLName = "AnchorRootXML";
        /// <summary>
        ///  The <see cref="roots"/> list items are stores as subitems in this location in the file
        /// </summary>
        const string RootListXmlName = "RootList";
        /// <summary>
        /// The individual folders contained within the <see cref="roots"/> list are stored with these and under the item <see cref="RootListXmlName"/> item
        /// </summary>
        const string RootListXmlEntryName = "Folder";
        /// <summary>
        /// the <see cref="EnumSubFolders"/> bool gets stashed here
        /// </summary>
        const string EnumSubFolderXmlName = "WantSubsToo";
        const string VersionXmlName = "VersionInfo";
        /// <summary>
        /// The <see cref="ToXml"/> and <see cref="CreateFromXmlString(string)"/> use this to mark the version they're working with.  If not there or does not match, it refuses to work
        /// </summary>
        const string VersionXmlData = "Version1";
        
        /// <summary>
        /// Converts the this SearchAnchor to xml and saves it to a stream
        /// </summary>
        /// <param name="output">Saves the xml to rebuild this <see cref="SearchAnchor"/> back via <see cref="CreateFromXmlString(string)"/></param>
        public void SaveXml(Stream output)
        {
            // build the parts
            XmlDocument ret = new XmlDocument();
            XmlElement DocumentBase = ret.CreateElement(DocumentBaseXMLName);

            XmlElement RootData = ret.CreateElement(RootListXmlName);
            XmlElement WantSubs = ret.CreateElement(EnumSubFolderXmlName);
            XmlElement Version = ret.CreateElement(VersionXmlName);

            // add the version info and the subfolder flag
            Version.InnerText = VersionXmlData;
            WantSubs.InnerText = this.EnumSubFoldersValue.ToString();

            ret.AppendChild(DocumentBase);
            DocumentBase.AppendChild(RootData);
            DocumentBase.AppendChild(WantSubs);
            DocumentBase.AppendChild(Version);

            // loop thru the entries and add to the xmldocument
            foreach (DirectoryInfo d in this.roots)
            {
                var ChildNode = ret.CreateElement(RootListXmlEntryName);
                ChildNode.InnerText = d.FullName;
                RootData.AppendChild(ChildNode);
            }
            
            ret.Save(output);
        }
        /// <summary>
        /// Converts the this SearchAnchor to xml and returns as a string
        /// </summary>
        /// <returns>using <see cref="SaveXml(Stream)"/> to save to a <see cref="MemoryStream"/> before reading it back as a string</returns>
        public string ToXml()
        {

            using (MemoryStream output = new MemoryStream())
            {
                SaveXml(output);
                output.Position = 0;
                byte[] buffer = new byte[output.Length];

                output.Read(buffer, 0, buffer.Length);
                string str = Encoding.UTF8.GetString(buffer);
                return str;
            }
            
        }


        

        /// <summary>
        /// Make an instance of the SearchAnchor contained within the xml string.
        /// </summary>
        /// <param name="xml">string containg xml to create from</param>
        /// <returns>returns a new instsance of searchanchor </returns>
        /// <exception cref="ArgumentException">if the string is not valid xml or an unsupported format, this is thrown</exception>
        public static SearchAnchor CreateFromXmlString(string xml)
        {
            SearchAnchor ret = new SearchAnchor(false);
            List<string> Roots = new List<string>();
            var xmlDocument = new XmlDocument();
            
            xmlDocument.LoadXml(xml);

            var Element = xmlDocument.GetElementsByTagName(DocumentBaseXMLName);

            XmlNodeList RootInfo;// = xmlDocument.GetElementsByTagName(RootListXmlName);
            XmlNode EnumFolderInfo;// = xmlDocument.GetElementsByTagName(EnumSubFolderXmlName);

            if (Element.Count == 0)
            {
                throw new ArgumentException();
            }
            else
            {
                // grab version info and fail if non existance or not matching
                var VersionData = Element.Item(0).SelectSingleNode(VersionXmlName);
                if (VersionData != null)
                {
                    if (VersionData.InnerText != VersionXmlData)
                    {
                        throw new ArgumentException();
                    }
                }
                else
                {
                    throw new ArgumentException();
                }

                // grab the anchor root list and fail if non existant
                RootInfo = Element.Item(0).SelectNodes(RootListXmlName);

                if (RootInfo.Count == 0)
                    throw new ArgumentException();
                else
                {
                    var root_tops = RootInfo.Item(0).ChildNodes;
                    foreach(XmlNode root in root_tops)
                    {
                        ret.AddAnchor(root.FirstChild.Value);
                    }
                }

                // don't forget the enum sub folder thing
                EnumFolderInfo = Element.Item(0).SelectSingleNode(EnumSubFolderXmlName);

                if (EnumFolderInfo.InnerText.ToLower() == "false")
                {
                    ret.EnumSubFolders = false;
                }
                else
                {
                    if (EnumFolderInfo.InnerText.ToLower() == "true")
                        ret.EnumSubFolders = true;
                    else
                        throw new ArgumentException();
                }

            }
            return ret;

        }
        
        public override bool Equals(object obj)
        {
            return Equals(obj as SearchAnchor);
        }

        public bool Equals(SearchAnchor other)
        {
            if (other != null)
            {
                

                if (roots.Count == other.roots.Count)
                {
                    for (int step = 0; step < roots.Count; step++)
                    {
                        var a = roots[step].ToString();
                        var b = other.roots[step].ToString();
                        if (a != b)
                        {
                            
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
                if (EnumSubFoldersValue != other.EnumSubFoldersValue) return false;
                return true;
            }

            return false;
          
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EnumSubFolders, roots);
        }

        public static bool operator ==(SearchAnchor left, SearchAnchor right)
        {
            return EqualityComparer<SearchAnchor>.Default.Equals(left, right);
        }

        public static bool operator !=(SearchAnchor left, SearchAnchor right)
        {
            return !(left == right);
        }
        #endregion
    }
}

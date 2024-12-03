using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OdinSearchEngine
{
    public delegate bool CustomizedCheck(FileSystemInfo CheckMe);
    /// <summary>
    /// Indicate what will be searched for.
    /// </summary>
    public class SearchTarget : IEquatable<SearchTarget>
    {
        /// <summary>
        /// from the /anyfile flag in the CLI app, this is a shorthand way to turn off all file system item compares and just feed seen file system items to the consumer class.
        /// </summary>
        public static SearchTarget AllFiles
        {
            get
            {
                return AllFilesBackup;
            }
        }
        private static readonly SearchTarget AllFilesBackup;
        static SearchTarget()
        {
            AllFilesBackup = new SearchTarget();
            AllFilesBackup.FileName.Add(MatchAnyFileName);
            AllFilesBackup.DirectoryMatching = MatchStyleString.Skip;
            AllFilesBackup.FileNameMatching = MatchStyleString.Skip;
            AllFilesBackup.AttributeMatching1 = AllFilesBackup.AttributeMatching2 = FileAttributes.Normal;
            AllFilesBackup.AttribMatching1Style = AllFilesBackup.AttribMatching2Style = MatchStyleFileAttributes.Skip;
            AllFilesBackup.AccessAnchorCheck1 = AllFilesBackup.AccessAnchorCheck2 = SearchTarget.MatchStyleDateTime.Disable;
            AllFilesBackup.WriteAnchorCheck1 = AllFilesBackup.WriteAnchorCheck2 = MatchStyleDateTime.Disable;
            AllFilesBackup.CreationAnchorCheck1 = AllFilesBackup.CreationAnchorCheck2 = MatchStyleDateTime.Disable;
            AllFilesBackup.CheckFileSize = false;
            AllFilesBackup.DirectoryMatching = MatchStyleString.Skip;
        }


        [Obsolete("Please use MatchAnyFileName.  They are the same but with the adding of the AnyFile static SearchTarget, this string may be ambigious. There is the possibility of this being removed.")]
        /// <summary>
        /// When added to <see cref="FileName"/> as in item, causes the compare to match sucessfully against any file name.
        /// </summary>
        public const string MatchAnyFile = "*";
        /// <summary>
        /// When added to <see cref="FileName"/> as in item, causes the compare to match sucessfully against any file name.
        /// </summary>
        public const string MatchAnyFileName = "*";
        
        internal enum ConvertToRegExMode
        {
            WantFileNameRegs = 1,
            WantDirectoryNameRegs = 2,
        }


#if DEBUG
        /// <summary>
        /// This is used in DEBUG unit testing. clearing this to false disables a call to <see cref="Regex.Escape(string)"/>. Note For Release Builds, this CANNOT be turned off.
        /// </summary>
        public bool RegSaftyMode
        {
            get => SafetyMode;
            set => SafetyMode = value;   
        }
#else
        /// <summary>
        /// This is used in DEBUG unit testing. clearing this to false disables a call to <see cref="Regex.Escape(string)"/>.  RELEASE MODE DOES NOT GIVE PUBLIC ability to set to false.
        /// </summary>
        public bool RegSaftyMode 
        {
            get => SafetyMode;
            set => throw new InvalidOperationException("Not supported in RELEASE BUILD.");
        }
#endif
        internal bool SafetyMode = true;
        /// <summary>
        ///  internal class used to convert lists of strings to lists of predone REGEX expresses. 
        /// </summary>
        /// <param name="Target">what to work on</param>
        /// <param name="mode">mode to use</param>
        /// <returns></returns>
        internal static List<Regex> ConvertToRegEx(SearchTarget Target, ConvertToRegExMode mode)
        {
            string pattern_prep(string pattern, MatchStyleString mode)
            {
                string ret;
                if (!mode.HasFlag(MatchStyleString.RawRegExMode))
                {
                    ret = "^" + Regex.Escape(pattern) + "$";
                    ret = ret.Replace("\\*", ".*").Replace("\\?", ".");
                    
                }
                else
                {
                    if (Target.RegSaftyMode)
                        ret = Regex.Escape(pattern);
                    else
                        ret = pattern;
                }
                return ret;
            }
            List<string> loopthru = null;
            MatchStyleString modethru = 0;
            if ((mode & ConvertToRegExMode.WantFileNameRegs | ConvertToRegExMode.WantDirectoryNameRegs) == 0)
            {
                throw new InvalidOperationException("Internal ConverToRegEx() with unspecified mode");
            }
            if (mode.HasFlag(ConvertToRegExMode.WantFileNameRegs))
            {
                loopthru = Target.FileName;
                modethru = Target.FileNameMatching;
            }
            if (mode.HasFlag(ConvertToRegExMode.WantDirectoryNameRegs))
            {
                loopthru = Target.DirectoryPath;
                modethru = Target.DirectoryMatching;
            }

            var ret = new List<Regex>();
            foreach (string s in loopthru)
            {
                string pattern;


                pattern = pattern_prep(s, modethru);
                //pattern = "^" + Regex.Escape(s) + "$";
                //pattern = pattern.Replace("\\*", ".*").Replace("\\?", ".");
                /* this is the match anything regexpression for.
                 * This is hard coded to returning a clear regex list. The code that does the searching treats
                 * it skipping the compare and treating it as a positive match.
                 * */
                if (pattern == "^.*$")
                {
                    ret.Clear();
                    return ret;
                }
                if (Target.FileNameMatching.HasFlag(MatchStyleString.CaseImportant))
                {
                    ret.Add(new Regex(pattern, RegexOptions.Singleline));
                }
                else
                {
                    ret.Add(new Regex(pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase));
                }

            }

            return ret;

        }
        /// <summary>
        /// Call <see cref="ConvertFileNameToRegEx(SearchTarget)"/> and specific this
        /// </summary>
        /// <remarks>same as calling <see cref="SearchTarget.ConvertFileNameToRegEx"/> and passing this</remarks>
        /// <returns>returns a list of RegEx instances to match the possible file names in the target</returns>
        public List<Regex> ConvertFileNameToRegEx()
        {
            return ConvertFileNameToRegEx(this);
        }
        /// <summary>
        /// Convert The passed SearchTarget file names to a matching RegEx list
        /// </summary>
        /// <param name="Target">which one to work on</param>
        /// <returns>returns a list of RegEx instances to match the possible file names in the target</returns>
        public static List<Regex> ConvertFileNameToRegEx(SearchTarget Target)
        {
            return ConvertToRegEx(Target, ConvertToRegExMode.WantFileNameRegs);
        }

        /// <summary>
        /// Convert SearchTarget Directry names to a matching RegEx list
        /// </summary>
        /// <param name="Target">which to work on</param>
        /// <returns>returns a list of RegEx instances to match the possible folders in the target </returns>
        public List<Regex> ConvertDirectoryPathToRegEx()
        {
            return ConvertDirectoryPathToRegEx(this);
        }
        /// <summary>
        /// Convert the passed SearchTarget Directry names to a matching RegEx list
        /// </summary>
        /// <param name="Target">which to work on</param>
        /// <returns>returns a list of RegEx instances to match the possible folders in the target </returns>
        public static List<Regex> ConvertDirectoryPathToRegEx(SearchTarget Target)
        {
            return ConvertToRegEx(Target, ConvertToRegExMode.WantDirectoryNameRegs);
        }

        #region xmlconstants
        /// <summary>
        /// the root element
        /// </summary>
        const string BaseXmlDocName = "OdinSearchTarget";
        /// <summary>
        /// Version Info Tag
        /// </summary>
        const string XmlDocVersion = "VersionInfo";
        /// <summary>
        /// Specific Version of this xml encoding
        /// </summary>
        const string XmlDocVersionData = "1";

        /// <summary>
        /// xml string for the <see cref="AccessAnchor"/> variable
        /// </summary>
        const string XmlAccessAnchor1Name = "AccessAnchor1";
        /// <summary>
        /// xml string for the <see cref="AccessAnchor2"/> varaible
        /// </summary>
        const string XmlAccessAnchor2Name = "AccessAnchor2";

        /// <summary>
        /// xml string for the <see cref="AccessAnchor"/> variable
        /// </summary>
        const string XmlAccessAnchor1Check = "AccessAnchor1Check";
        /// <summary>
        /// xml string for the <see cref="AccessAnchor2"/> varaible
        /// </summary>
        const string XmlAccessAnchor2Check = "AccessAnchor2Check";


        /// <summary>
        /// xml string for the <see cref="AttributeMatching1"/> variable
        /// </summary>
        const string XmlAttribMatch1 = "AttribMatch1";
        /// <summary>
        /// xml string for the <see cref="AttributeMatching2"/> vaarible
        /// </summary>
        const string XmlAttribMatch2 = "AttribMatch2";


        /// <summary>
        /// xml string for the <see cref="AttribMatching1Style"/> variable
        /// </summary>
        const string XmlAttribMatchString1 = "AttribMatchStyle1";
        /// <summary>
        /// xml string for the <see cref="AttribMatching2Style"/> variable
        /// </summary>
        const string XmlAttribMatchString2 = "AttribMatchStyle2";



        /// <summary>
        /// xml string for the <see cref="CheckFileSize"/> variable
        /// </summary>
        const string XmlCheckFileSize = "CheckFileSize";
        const string XmlFileSizeLow = "FileSizeLow";
        const string XmlFileSizeHigh = "FileSizeHigh";

        const string XmlCreationAnchor1 = "CreationAnchor1";
        const string XmlCreationAnchor2 = "CreationAnchor2";

        const string XmlCreationCheck1 = "CreationAnchorCheck1";
        const string XmlCreationCheck2 = "CreationAnchorCheck2";

        const string XmlDirectoryName = "DirectoryPath";
        const string XmlDirectoryNameContent = "DirectoryPathContentEntry";
        const string XmlDirectoryNameCheck = "DirectoryNameCheck";

        const string XmlFileName = "FileName";
        const string XmlFileNameContent = "FileNameEntry";
        const string XmlFileNameCheck = "FileNameCheck";


        const string XmlWriteAnchor1 = "WriteAnchor1";
        const string XmlWriteAnchor2 = "WriteAnchor2";
        const string XmlWriteAnchorCheck1 = "WriteAnchorCheck1";
        const string XmlWriteAnchorCheck2 = "WriteAnchorCheck2";

        #endregion

        /// <summary>
        /// Save this <see cref="SearchTarget"/> as XML to a stream
        /// </summary>
        /// <param name="output"></param>
        /// <exception cref="NotImplementedException">Throws this</exception>
        public void SaveXml(Stream output)
        {
            // 23 different items to xmlize
            XmlDocument ret = new XmlDocument();
            XmlElement BaseTag = ret.CreateElement(BaseXmlDocName);
            XmlElement VersionData = ret.CreateElement(XmlDocVersion);
            VersionData.InnerText = XmlDocVersionData;
            
            ret.AppendChild(BaseTag);
            BaseTag.AppendChild(VersionData);


            XmlElement AccessAnchor1 = ret.CreateElement(XmlAccessAnchor1Name);
            XmlElement AccessAnchor2 = ret.CreateElement(XmlAccessAnchor2Name);
            AccessAnchor1.InnerText = this.AccessAnchor.ToString();
            AccessAnchor2.InnerText = this.AccessAnchor2.ToString();
            BaseTag.AppendChild(AccessAnchor1);
            BaseTag.AppendChild(AccessAnchor2);


            XmlElement AccessAnchor1Check = ret.CreateElement(XmlAccessAnchor1Check);
            XmlElement AccessAnchor2Check = ret.CreateElement(XmlAccessAnchor2Check);
            AccessAnchor1Check.InnerText = this.AccessAnchorCheck1.ToString();
            AccessAnchor2Check.InnerText = this.AccessAnchorCheck2.ToString();
            BaseTag.AppendChild(AccessAnchor1Check);
            BaseTag.AppendChild(AccessAnchor2Check);

            XmlElement attribMatch1style = ret.CreateElement(XmlAttribMatchString1);
            XmlElement attribMatch2style = ret.CreateElement(XmlAttribMatchString2);
            attribMatch1style.InnerText = this.AttribMatching1Style.ToString();
            attribMatch2style.InnerText = this.AttribMatching2Style.ToString();
            BaseTag.AppendChild(attribMatch1style);
            BaseTag.AppendChild(attribMatch2style);

            XmlElement attribMatch1 = ret.CreateElement(XmlAttribMatch1); 
            XmlElement attribMatch2 = ret.CreateElement(XmlAttribMatch2);
            attribMatch1.InnerText = this.AttributeMatching1.ToString();
            attribMatch2.InnerText = this.AttributeMatching2.ToString();
            BaseTag.AppendChild(attribMatch1);
            BaseTag.AppendChild(attribMatch2);

            XmlElement CheckFileSize = ret.CreateElement(XmlCheckFileSize);
            CheckFileSize.InnerText = this.CheckFileSize.ToString();
            BaseTag.AppendChild(CheckFileSize); ;

            XmlElement CreationAnchor1 = ret.CreateElement(XmlCreationAnchor1);
            XmlElement CreationAnchor2 = ret.CreateElement(XmlCreationAnchor2);
            CreationAnchor1.InnerText = this.CreationAnchor.ToString();
            CreationAnchor2.InnerText = this.CreationAnchor2.ToString();
            BaseTag.AppendChild(CreationAnchor1);
            BaseTag.AppendChild(CreationAnchor2 );

            XmlElement CreationAnchorcheck1 = ret.CreateElement(XmlCreationCheck1);
            XmlElement CreationAnchorcheck2 = ret.CreateElement(XmlCreationCheck2);
            CreationAnchorcheck1.InnerText = this.CreationAnchorCheck1.ToString();
            CreationAnchorcheck2.InnerText = this.CreationAnchorCheck2.ToString();
            BaseTag.AppendChild(CreationAnchorcheck1);
            BaseTag.AppendChild(CreationAnchorcheck2);

            var DirectNameTag = ret.CreateElement(XmlDirectoryName);
            foreach (string s in this.DirectoryPath)
            {
                var innername = ret.CreateElement(XmlDirectoryNameContent);
                innername.InnerText = s;
                DirectNameTag.AppendChild(innername);
            }
            BaseTag.AppendChild(DirectNameTag);

            XmlElement DirNameCompare = ret.CreateElement(XmlDirectoryNameCheck);
            DirNameCompare.InnerText = DirectoryMatching.ToString();
            BaseTag.AppendChild(DirNameCompare);

            var FileNameTag = ret.CreateElement(XmlFileName);
            foreach (string s in this.FileName)
            {
                var innername = ret.CreateElement(XmlFileNameContent);
                innername.InnerText = s;
                FileNameTag.AppendChild(innername);
            }
            BaseTag.AppendChild(FileNameTag);

            XmlElement FileNameCompare = ret.CreateElement(XmlFileNameCheck);
            FileNameCompare.InnerText = FileNameMatching.ToString();
            BaseTag.AppendChild(FileNameCompare);

            XmlElement FileSizeHigh = ret.CreateElement(XmlFileSizeHigh);
            FileSizeHigh.InnerText = this.FileSizeMax.ToString();
            BaseTag.AppendChild(FileSizeHigh);

            XmlElement FileSizeLow = ret.CreateElement(XmlFileSizeLow);
            FileSizeLow.InnerText = this.FileSizeMin.ToString();
            BaseTag.AppendChild(FileSizeLow);
            


            XmlElement WriteAnchor1 = ret.CreateElement(XmlWriteAnchor1);
            XmlElement WriteAnchor2 = ret.CreateElement(XmlWriteAnchor2);
            WriteAnchor1.InnerText = this.WriteAnchor.ToString();
            WriteAnchor2.InnerText = this.WriteAnchor2.ToString();
            BaseTag.AppendChild(WriteAnchor1);
            BaseTag.AppendChild(WriteAnchor2);

            XmlElement WriteAnchorCheck1 = ret.CreateElement(XmlWriteAnchorCheck1);
            XmlElement WriteAnchorCheck2 = ret.CreateElement(XmlWriteAnchorCheck2);

            WriteAnchorCheck1.InnerText = this.WriteAnchorCheck1.ToString();
            WriteAnchorCheck2.InnerText = this.WriteAnchorCheck2.ToString();
            BaseTag.AppendChild(WriteAnchorCheck1);
            BaseTag.AppendChild(WriteAnchorCheck2);

            






            


            
            
            







            ret.Save(output);
        }

        /// <summary>
        /// Conert this <see cref="SearchTarget"/> to xmland return it.
        /// </summary>
        /// <returns>string containing the xml.</returns>
        /// <exception cref="NotImplementedException">Due this calling <see cref="SaveXml(Stream)"/>, it will always throw this until that's coded</exception>
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
        public static SearchTarget CreateFromXmlString(string v)
        {
            SearchTarget ret = new SearchTarget();
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(v);

            var Base = xmlDoc.GetElementsByTagName(SearchTarget.BaseXmlDocName);
            
            if (Base.Count == 0)
            {
                throw new ArgumentException();
            }
            var VersionInfo = Base.Item(0).SelectSingleNode(XmlDocVersion);
            if (VersionInfo != null)
            {
                if (VersionInfo.InnerText != XmlDocVersionData)
                {
                    throw new ArgumentException();
                }
            }
            else
            {
                throw new ArgumentException();
            }

            XmlNode AccessAnchor1 = Base.Item(0).SelectSingleNode(XmlAccessAnchor1Name);
            XmlNode AccessAnchor2  = Base.Item(0).SelectSingleNode(XmlAccessAnchor2Name);
            XmlNode AccessAnchorCheck1 = Base.Item(0).SelectSingleNode(XmlAccessAnchor1Check);
            XmlNode AccessAnchorCheck2 = Base.Item(0).SelectSingleNode(XmlAccessAnchor2Check);


            if (AccessAnchor1 != null)
            {
                if (!DateTime.TryParse(AccessAnchor1.InnerText, out ret.AccessAnchor))
                {
                    throw new ArgumentException();
                }

                if (!Enum.TryParse<MatchStyleDateTime>(AccessAnchorCheck1.InnerText, out ret.AccessAnchor1_MatchStyleBacking))
                {
                    throw new ArgumentException();
                }
            }

            if (AccessAnchor2 != null)
            {
                if (!DateTime.TryParse(AccessAnchor2.InnerText, out ret.AccessAnchor2))
                {
                    throw new ArgumentException();
                }

                if (!Enum.TryParse<MatchStyleDateTime>(AccessAnchorCheck2.InnerText, out ret.AccessAnchor2_MatchStyleBacking))
                {
                    throw new ArgumentException();
                }
            }


            XmlNode AttribMatchStyle1 = Base.Item(0).SelectSingleNode(XmlAttribMatchString1);
            XmlNode AttribMatchStyle2 = Base.Item(0).SelectSingleNode(XmlAttribMatchString2);
            XmlNode AttribMatch1 = Base.Item(0).SelectSingleNode(XmlAttribMatch1);
            XmlNode AttribMatch2 = Base.Item(0).SelectSingleNode(XmlAttribMatch2);

            if (AttribMatch1 != null)
            {
                if (!Enum.TryParse<FileAttributes>(AttribMatch1.InnerText, out ret.Attrib1_Backing))
                {
                    throw new ArgumentException();
                }

                if (AttribMatchStyle1 != null)
                {
                    if (!Enum.TryParse<MatchStyleFileAttributes>(AttribMatchStyle1.InnerText, true, out ret.AttribMatching1Style))
                    {
                        throw new ArgumentException();
                    }
                }
            }

            if (AttribMatch2 != null)
            {
                if (!Enum.TryParse<FileAttributes>(AttribMatch2.InnerText, out ret.Attrib2_Backing))
                {
                    throw new ArgumentException();
                }

                if (AttribMatchStyle2 != null)
                {
                    if (!Enum.TryParse<MatchStyleFileAttributes>(AttribMatchStyle2.InnerText, true, out ret.AttribMatching2Style))
                    {
                        throw new ArgumentException();
                    }
                }
            }

            XmlNode CheckSize = Base.Item(0).SelectSingleNode(SearchTarget.XmlCheckFileSize);
            if (CheckSize != null) 
            {
                if (!bool.TryParse(CheckSize.InnerText, out ret.CheckFileSize))
                {
                    throw new ArgumentException();
                }
            }



            XmlNode CreationAnchor1 = Base.Item(0).SelectSingleNode(XmlCreationAnchor1);
            XmlNode CreationAnchor2 = Base.Item(0).SelectSingleNode(XmlCreationAnchor2);
            XmlNode CreationAnchor1Check = Base.Item(0).SelectSingleNode(XmlCreationCheck1);
            XmlNode CreationAnchor2Check = Base.Item(0).SelectSingleNode(XmlCreationCheck2);


            if (CreationAnchor1 != null)
            {
                if (!DateTime.TryParse(CreationAnchor1.InnerText, out ret.CreationAnchor1_Backing))
                {
                    throw new ArgumentException();
                }

                if (!Enum.TryParse<MatchStyleDateTime>(CreationAnchor1Check.InnerText, out ret.CreationAnchor1_MatchStyleBacking))
                {
                    throw new ArgumentException();
                }
            }


            if (CreationAnchor2 != null)
            {
                if (!DateTime.TryParse(CreationAnchor2.InnerText, out ret.CreationAnchor2))
                {
                    throw new ArgumentException();
                }

                if (!Enum.TryParse<MatchStyleDateTime>(CreationAnchor2Check.InnerText, out ret.CreationAnchor2_MatchStyleBacking))
                {
                    throw new ArgumentException();
                }
            }

            XmlNodeList DirectoryNames = Base.Item(0).SelectNodes(XmlDirectoryName);



            if ((DirectoryNames == null) || (DirectoryNames.Count == 0))
            {
                ret.DirectoryPath.Clear();
                ret.DirectoryMatching = MatchStyleString.Skip;
            }
            else
            {
                var DirNameEntries = DirectoryNames.Item(0).ChildNodes;
                foreach (XmlNode DirNameEntry in DirNameEntries)
                {
                    ret.DirectoryPath.Add(DirNameEntry.FirstChild.Value);
                }


                if (ret.DirectoryPath.Count != 0)
                {
                    XmlNode DirNameHandling = Base.Item(0).SelectSingleNode(XmlDirectoryNameCheck);
                    if (DirNameHandling != null)
                    {
                        if (!Enum.TryParse<MatchStyleString>(DirNameHandling.InnerText, out ret.DirectoryMatching))
                        {
                            throw new ArgumentException();
                        }
                    }
                }
            }



            XmlNodeList FileNames = Base.Item(0).SelectNodes(XmlFileName);



            if ( (FileNames == null) || (FileNames.Count == 0))
            {
                ret.FileName.Clear();
            }
            else
            {
                var FileNameEntries = FileNames.Item(0).ChildNodes;
                foreach ( XmlNode FileNameEntry in FileNameEntries)
                {
                    ret.FileName.Add(FileNameEntry.FirstChild.Value);
                }


                if (ret.FileName.Count != 0)
                {
                    XmlNode FileNameHandling = Base.Item(0).SelectSingleNode(XmlFileNameCheck);
                    if (FileNameHandling != null)
                    {
                        if (!Enum.TryParse<MatchStyleString>(FileNameHandling.InnerText, out ret.FileNameMatching))
                        {
                            throw new ArgumentException();
                        }
                    }
                }
            }
            XmlNode FileSizeHigh = Base.Item(0).SelectSingleNode(XmlFileSizeHigh);
            XmlNode FileSizeMin =Base.Item(0).SelectSingleNode(XmlFileSizeLow);

            if (FileSizeHigh != null)
            {
                if (!long.TryParse(FileSizeHigh.InnerText, out ret.FileSizeMax))
                {
                    throw new ArgumentException();
                }
            }
            if (FileSizeMin != null)
            {
                if (!long.TryParse(FileSizeMin.InnerText, out ret.FileSizeMin))
                {
                    throw new ArgumentException();
                }
            }



            XmlNode WriteAnchor1 = Base.Item(0).SelectSingleNode(XmlWriteAnchor1);
            XmlNode WriteAnchor2 = Base.Item(0).SelectSingleNode(XmlWriteAnchor2);
            XmlNode WriteAnchorCheck1 = Base.Item(0).SelectSingleNode(XmlWriteAnchorCheck1);
            XmlNode WriteAnchorCheck2 = Base.Item(0).SelectSingleNode(XmlWriteAnchorCheck2);


            if (WriteAnchor1 != null)
            {
                if (!DateTime.TryParse(WriteAnchor1.InnerText, out ret.WriteAnchor))
                {
                    throw new ArgumentException();
                }

                if (!Enum.TryParse<MatchStyleDateTime>(WriteAnchorCheck1.InnerText, out ret.WriteAnchor1_MatchStyleBacking))
                {
                    throw new ArgumentException();
                }
            }

            if (WriteAnchor2 != null)
            {
                if (!DateTime.TryParse(WriteAnchor2.InnerText, out ret.WriteAnchor2))
                {
                    throw new ArgumentException();
                }

                if (!Enum.TryParse<MatchStyleDateTime>(WriteAnchorCheck2.InnerText, out ret.WriteAnchor2_MatchStyleBacking))
                {
                    throw new ArgumentException();
                }
            }


            return ret;
        }

        public override int GetHashCode()
        {
            int hash = HashCode.Combine(this.AttributeMatching1, AttributeMatching2, AttribMatching2Style, AttribMatching1Style, this.FileNameMatching, FileSizeMax, FileSizeMax, FileSizeMax);
            hash = HashCode.Combine(hash, this.AccessAnchor, AccessAnchor2, AccessAnchorCheck1, AccessAnchorCheck2, this.AttribMatching1Style, AttribMatching2Style, AttributeMatching1);
            hash = HashCode.Combine(hash, AttributeMatching2, this.CheckFileSize, this.CreationAnchor, CreationAnchor2, CreationAnchorCheck1, CreationAnchorCheck2, this.DirectoryMatching);
            
            if (this.FileName != null)
            {
                foreach(string s in this.FileName)
                {
                    hash = HashCode.Combine(hash, s.GetHashCode());
                }
            }
            else
            {
                hash = HashCode.Combine(hash, FileName);
            }

            if (this.DirectoryPath != null)
            {
                foreach (string s in this.DirectoryPath)
                {
                    hash = HashCode.Combine(hash, s.GetHashCode());
                }
            }
            else
            {
                hash = HashCode.Combine(hash, DirectoryPath);
            }

            return hash;


        }

        public static bool operator ==(SearchTarget left, SearchTarget right)
        {
            return EqualityComparer<SearchTarget>.Default.Equals(left, right);
        }

        public static bool operator !=(SearchTarget left, SearchTarget right)
        {
            return !(left == right);
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as SearchTarget);
        }
        public bool Equals(SearchTarget other)
        {
            // does a and b count match and do each list contain the same items.
            bool stringListCompare(List<string> a, List<string> b)
            {
                if ((a != null) && (b != null))
                {
                    if (a.Count != b.Count)
                        return false;
                    else
                    {
                        for (int step = 0; step < a.Count; step++)
                        {
                            if (b.Find(p => { return (p == a[step]); }) == null)
                            {
                                return false;
                            }
                        }


                        for (int step = 0; step < b.Count; step++)
                        {
                            if (a.Find(p => { return (p == b[step]); }) == null)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
                return false;
            }
            if (other == null) return false;

            if (this.CreationAnchor != other.CreationAnchor) return false;
            if (this.CreationAnchor2 != other.CreationAnchor2) return false;
            if (this.AccessAnchor != other.AccessAnchor) return false;
            if (this.AccessAnchor2 != other.AccessAnchor2) return false;
            if (this.WriteAnchor != other.WriteAnchor) return false;
            if (this.WriteAnchor2 != other.WriteAnchor2) return false;
            if (this.CreationAnchorCheck1 != other.CreationAnchorCheck1) return false;
            if (this.CreationAnchorCheck2 != other.CreationAnchorCheck2) return false;
            if (this.AccessAnchorCheck1 != other.AccessAnchorCheck1) return false;
            if (this.AccessAnchorCheck2 != other.AccessAnchorCheck2) return false;
            if (this.WriteAnchorCheck1 != other.WriteAnchorCheck1) return false;
            if (this.WriteAnchorCheck2 != other.WriteAnchorCheck2) return false;


            if (stringListCompare(this.FileName, other.FileName) == false) return false;
            if (this.FileNameMatching != other.FileNameMatching) return false;


            if (stringListCompare(this.DirectoryPath, other.DirectoryPath) == false) return false;
            if (this.DirectoryMatching != other.DirectoryMatching) return false;



            if (this.AttributeMatching1 != other.AttributeMatching1) return false;
            if (this.AttributeMatching2 != other.AttributeMatching2) return false;



            if (this.FileSizeMax != other.FileSizeMax) return false;
            if (this.FileSizeMin != other.FileSizeMin) return false;
            if (this.CheckFileSize != other.CheckFileSize) return false;


            if (this.AttribMatching1Style != other.AttribMatching1Style) return false;
            if (this.AttribMatching2Style != other.AttribMatching2Style) return false;




            return true;

        }
        


        /// <summary>
        /// Make a blank instance of this class.
        /// </summary>
        public SearchTarget()
        {

        }

        #region BACKING_VALUES
        protected FileAttributes Attrib1_Backing = 0;
        protected FileAttributes Attrib2_Backing = FileAttributes.Normal;
        protected DateTime CreationAnchor1_Backing;
        protected MatchStyleDateTime CreationAnchor1_MatchStyleBacking = MatchStyleDateTime.Disable;
        protected MatchStyleDateTime CreationAnchor2_MatchStyleBacking = MatchStyleDateTime.Disable;
        protected MatchStyleDateTime AccessAnchor1_MatchStyleBacking = MatchStyleDateTime.Disable;
        protected MatchStyleDateTime AccessAnchor2_MatchStyleBacking = MatchStyleDateTime.Disable;
        protected MatchStyleDateTime WriteAnchor1_MatchStyleBacking = MatchStyleDateTime.Disable;
        protected MatchStyleDateTime WriteAnchor2_MatchStyleBacking = MatchStyleDateTime.Disable;
        #endregion
        #region INTERNAL_CONSTS
        /// <summary>
        /// used as the upper valid limit for file attributes
        /// </summary>
        private const int OneOffFileAttrib = 262144;
        #endregion
        /// <summary>
        /// Check against <see cref="FileSystemInfo.CreationTime"/>
        /// </summary>
        public DateTime CreationAnchor;
        /// <summary>
        /// 2nd Check against <see cref="FileSystemInfo.CreationTime"/>
        /// </summary>
        public DateTime CreationAnchor2;

        /// <summary>
        /// Check against <see cref="FileSystemInfo.LastAccessTime"/>
        /// </summary>
        public DateTime AccessAnchor;

        /// <summary>
        /// 2nd Check against <see cref="FileSystemInfo.LastAccessTime"/>
        /// </summary>
        public DateTime AccessAnchor2;

        /// <summary>
        /// Check against <see cref="FileSystemInfo.LastWriteTime"/>
        /// </summary>
        public DateTime WriteAnchor;

        /// <summary>
        /// 2nd Check against <see cref="FileSystemInfo.LastWriteTime"/>
        /// </summary>

        public DateTime WriteAnchor2;

        /// <summary>
        /// Indicate what do do with <see cref="CreationAnchor"/>
        /// </summary>
        public MatchStyleDateTime CreationAnchorCheck1
        {
            get => CreationAnchor1_MatchStyleBacking;
            set
            {
                if ( (value < MatchStyleDateTime.Disable)  || (value > MatchStyleDateTime.NoLaterThanThis))
                {
                    throw new InvalidEnumArgumentException();
                }
                CreationAnchor1_MatchStyleBacking = value;
            } 
        }
        /// <summary>
        /// Indicate what do do with <see cref="CreationAnchor2"/>
        /// </summary>
        public MatchStyleDateTime CreationAnchorCheck2
        {
            get => CreationAnchor2_MatchStyleBacking;
            set
            {
                if ((value < MatchStyleDateTime.Disable) || (value > MatchStyleDateTime.NoLaterThanThis))
                {
                    throw new InvalidEnumArgumentException();
                }
                CreationAnchor2_MatchStyleBacking = value;
            }
        }

        /// <summary>
        /// Indicate what do do with <see cref="AccessAnchor"/>
        /// </summary>
        public MatchStyleDateTime AccessAnchorCheck1
        {
            get => AccessAnchor1_MatchStyleBacking;
            set
            {
                if ((value < MatchStyleDateTime.Disable) || (value > MatchStyleDateTime.NoLaterThanThis))
                {
                    throw new InvalidEnumArgumentException();
                }
                AccessAnchor1_MatchStyleBacking = value;
            }
        }
        /// <summary>
        /// Indicate what do do with <see cref="AccessAnchor2"/>
        /// </summary>
        public MatchStyleDateTime AccessAnchorCheck2
        {
            get => AccessAnchor2_MatchStyleBacking;
            set
            {
                if ((value < MatchStyleDateTime.Disable) || (value > MatchStyleDateTime.NoLaterThanThis))
                {
                    throw new InvalidEnumArgumentException();
                }
                AccessAnchor2_MatchStyleBacking = value;
            }
        }

        /// <summary>
        /// Indicate what do do with <see cref="WriteAnchor"/>
        /// </summary>
        public MatchStyleDateTime WriteAnchorCheck1
        {
            get => WriteAnchor1_MatchStyleBacking;
            set
            {
                if ((value < MatchStyleDateTime.Disable) || (value > MatchStyleDateTime.NoLaterThanThis))
                {
                    throw new InvalidEnumArgumentException();
                }
                WriteAnchor1_MatchStyleBacking = value;
            }
        }


        /// <summary>
        /// Indicate what do do with <see cref="WriteAnchor"/>
        /// </summary>
        public MatchStyleDateTime WriteAnchorCheck2
        {
            get => WriteAnchor2_MatchStyleBacking;
            set
            {
                if ((value < MatchStyleDateTime.Disable) || (value > MatchStyleDateTime.NoLaterThanThis))
                {
                    throw new InvalidEnumArgumentException();
                }
                WriteAnchor2_MatchStyleBacking = value;
            }
        }

        /// <summary>
        /// A REGEX express that will be compared againt the <see cref="FileInfoExtract.Name"/>
        /// </summary>
        public readonly List<string> FileName = new List<string>();
        /// <summary>
        /// Determines how will will be comparing input file names against the filters at <see cref="FileName"/>
        /// </summary>
        /// <example>Consider C:\\Windows\\notepad.exe.  The string compared would be notepad.exe.</example>
        public MatchStyleString FileNameMatching = MatchStyleString.MatchAny;

        /// <summary>
        /// REGEX expressthat that's compared against <see cref="FileInfoExtract.FullName"/>
        /// </summary>
        public readonly List<string> DirectoryPath = new List<string>() ;

        /// <summary>
        /// Determines how will will be comparing input complate file location and names against the filters at <see cref="FileName"/>
        /// </summary>
        /// <example>Consider C:\\Windows\\notepad.exe.  The string compared would be C:\\Windows\\notepad.exe.</example>
        public MatchStyleString DirectoryMatching = MatchStyleString.MatchAny;

        /// <summary>
        /// expression that's compared against <see cref="FileInfoExtract.FileAttributes"/>.  If equal to zero or <see cref="FileAttributes.Normal"/>, the compare is skipped
        /// </summary>
        public FileAttributes AttributeMatching1
        {
            get
            {
                return Attrib1_Backing;
            }
            set
            {
                if ( ( (value >= (FileAttributes)SearchTarget.OneOffFileAttrib) || (value < (FileAttributes)1)) && (value != 0))
                {
                    throw new InvalidEnumArgumentException();
                }
                Attrib1_Backing = value;
            }
        }
        /// <summary>
        /// How to compare <see cref="AttributeMatching1"/> with possible entries
        /// </summary>
        public MatchStyleFileAttributes AttribMatching1Style = MatchStyleFileAttributes.Skip;

        /// <summary>
        /// Expression that's (by default) compared to be LACKING in <see cref="FileInfoExtract.FileAttributes"/>
        /// </summary>
        public FileAttributes AttributeMatching2
        {
            get
            {
                return Attrib2_Backing;
            }
            set
            {
                if (value >= (FileAttributes)OneOffFileAttrib)
                {
                    throw new InvalidEnumArgumentException();
                }
                Attrib2_Backing = value;
            }
        }
        /// <summary>
        /// How to compare <see cref="AttributeMatching2"/> with possible entries
        /// </summary>
        public MatchStyleFileAttributes AttribMatching2Style = MatchStyleFileAttributes.Invert | MatchStyleFileAttributes.Skip;


        /// <summary>
        /// The file must be at least this big in bytes. This check is skippped if <see cref="AttribMatching1Style"/> indicates the file system item is device or folder
        /// </summary>
        public long FileSizeMin = 0;
        /// <summary>
        /// The file can't be more than this big in bytes. This check is skippped if <see cref="AttribMatching1Style"/> indicates the file system item is device or folder
        /// </summary>
        public long FileSizeMax=  long.MaxValue;

        /// <summary>
        /// If true we compare the <see cref="FileSizeMax"/> and <see cref="FileSizeMax"/>. This does involve casting the generic <see cref="FileSystemInfo"/> to a <see cref="FileInfo"/>
        /// </summary>
        public bool CheckFileSize = false;

        /// <summary>
        /// Tell the search what do do with the same times specified
        /// </summary>
        public enum MatchStyleDateTime
        {
            // DO NOT CHECK
            Disable = 0,
            /// <summary>
            /// Match Fails if datetime is earlier than specificed time
            /// </summary>
            NoEarlierThanThis = 1,
            // Match Files if datetime is later than this
            NoLaterThanThis = 2,
        }

        /// <summary>
        /// These Attributes indicate how to fail with <see cref="FileAttributes"/> matching
        /// </summary>
        [Flags]
        public enum MatchStyleFileAttributes
        {
            /// <summary>
            /// Match at least attribute item in the file / folder's attributes.  Invert means at least one attribute specified must not be there
            /// </summary>
            MatchAny = 1,
            /// <summary>
            /// The file/folder must contain the <see cref="SearchTarget"/> attributes specified. It can contain others also unless <see cref="Exacting"/> is also set
            /// </summary>
            MatchAll = 2,
            /// <summary>
            /// When comparing <see cref="SearchTarget"/> file attributes, the match is a bool value, invert the result before dealing with it.
            /// </summary>
            Invert = 4,
            /// <summary>
            /// For MatchAll, this means that the file/folder cannot have additional flags beyond what the <see cref="SearchTarget"/> wants or it fails
            /// </summary>
            Exacting = 8,
            /// <summary>
            /// Disable the check
            /// </summary>
            Skip = 16,
            /// <summary>
            /// ReservedForFuture
            /// </summary>
            Reserved = 32
        }

        /// <summary>
        /// In case you need a sanity check against input
        /// </summary>
        /// <param name="e"></param>
        /// <returns>true if it's a valid combo and false if not</returns>
        public static bool VerifyMatchStyleStringValue(MatchStyleString e)
        {
            return (e is >= (MatchStyleString)1 and <= (MatchStyleString)128) && (e.HasFlag( MatchStyleString.ReservedUnused) == false);
        }

        /// <summary>
        /// in case you need a sanity check against input
        /// </summary>
        /// <param name="e"></param>
        /// <returns>true if it's a valid combo and false if not</returns>
        public static bool VerifyMatchStyleFileAttrib(MatchStyleFileAttributes e)
        {
            return (e is >= (MatchStyleFileAttributes)1 and <= (MatchStyleFileAttributes)64) && (e.HasFlag(MatchStyleFileAttributes.Reserved) == false);
        }

        [Flags]
        /// <summary>
        /// Tell the search what to do with the string based search input
        /// </summary>
        public enum MatchStyleString
        {
            /// <summary>
            /// Match at least string item in the list.  Invert means at least one string specified must not be there
            /// </summary>
            MatchAny = 1,
            /// <summary>
            /// DEFAULT if no match is specified. Match all string items in the list. Invert means NO entries must match or the search values.
            /// </summary>
            /// <remarks>If neither <see cref="MatchAll"/> or <see cref="MatchAny"/> are specified a <see cref="SearchTarget.FileName"/> or <see cref="SearchTarget.DirectoryPath"/> default is <see cref="MatchAll"/></remarks>
            MatchAll = 2,
            /// <summary>
            /// A sucessful match to the target fails the compaire i.e. now this part of the <see cref="SearchTarget"/> specifies what it must NOT match
            /// </summary>
            Invert = 4,
            /// This causes your string to be passed to the regex compare without assuming it's a file. Note. you need to ensure proper RegEx encoding or Worker threads may crash.
            /// </summary>
            RawRegExMode= 8,
            /// <summary>
            /// Disable this matching.. 
            /// </summary>
            Skip =16,
            /// <summary>
            /// Add to make the search string case sensitive.
            /// </summary>
            CaseImportant = 32,
            /// <summary>
            /// Reserved for future. Currently not used for this enum beyond a cap in the sanit ychecvk
            /// </summary>
            ReservedUnused = 64
        }
    }
}

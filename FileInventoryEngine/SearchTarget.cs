using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace OdinSearchEngine
{
    public delegate bool CustomizedCheck(FileSystemInfo CheckMe);
    /// <summary>
    /// Indicate what will be searched for.
    /// </summary>
    public class SearchTarget: IEquatable<SearchTarget>
    {
        /// <summary>
        /// When added to <see cref="FileName"/> as in item, causes the compare to match sucessfully against any file.
        /// </summary>
        public const string MatchAnyFile = "*";

        internal enum ConvertToRegExMode
        {
            WantFileNameRegs = 1,
            WantDirectoryNameRegs = 2,
        }


        /// <summary>
        ///  internal class used to convert lists of strings to lists of predone REGEX expresses. 
        /// </summary>
        /// <param name="Target">what to work on</param>
        /// <param name="mode">mode to use</param>
        /// <returns></returns>
        internal static List<Regex> ConvertToRegEx(SearchTarget Target, ConvertToRegExMode mode)
        {
            List<string> loopthru = null;
            
            if ((mode & ConvertToRegExMode.WantFileNameRegs | ConvertToRegExMode.WantDirectoryNameRegs) == 0)
            {
                throw new InvalidOperationException("Internal ConverToRegEx() with unspecified mode");
            }
            if (mode.HasFlag(ConvertToRegExMode.WantFileNameRegs))
                loopthru = Target.FileName;
            if (mode.HasFlag(ConvertToRegExMode.WantDirectoryNameRegs))
                loopthru = Target.DirectoryPath;

            var ret = new List<Regex>();
            foreach (string s in loopthru)
            {
                string pattern;

                pattern = "^" + Regex.Escape(s) + "$";
                pattern = pattern.Replace("\\*", ".*").Replace("\\?", ".");
                /* this is the match anything regexpression for.
                 * This is hard coded to returning a clear regex list. The code that does the searching treats
                 * it skipping the compare and treating it as a positivbematch.
                 * */
                if (pattern == "^.*$")
                {
                    ret.Clear();
                    return ret;
                }
                if (!Target.FileNameMatching.HasFlag(MatchStyleString.CaseImportant))
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

        /// <summary>
        /// Save this <see cref="SearchTarget"/> as XML to a stream
        /// </summary>
        /// <param name="output"></param>
        /// <exception cref="NotImplementedException">Throws this</exception>
        public void SaveXml(Stream output)
        {
            XmlDocument ret = new XmlDocument();
            XmlElement BaseTag = ret.CreateElement("OdinSearchTarget");

            XmlElement AccessAnchor1Tag = ret.CreateElement("AccessAnchor1");
            XmlElement AccessAnchor2Tag = ret.CreateElement("AccessAnchor2");
            XmlElement AccessAnchorHandle1 = ret.CreateElement("AccessAnchor1");


            throw new NotImplementedException();


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
        public static SearchTarget CreateFromXml(string v)
        {
            throw new NotImplementedException();
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


            if (stringListCompare(this.DirectoryPath, other.DirectoryPath) == false)
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
        public MatchStyleDateTime CreationAnchorCheck1 = MatchStyleDateTime.Disable;
        /// <summary>
        /// Indicate what do do with <see cref="CreationAnchor2"/>
        /// </summary>
        public MatchStyleDateTime CreationAnchorCheck2 = MatchStyleDateTime.Disable;

        /// <summary>
        /// Indicate what do do with <see cref="AccessAnchor"/>
        /// </summary>
        public MatchStyleDateTime AccessAnchorCheck1 = MatchStyleDateTime.Disable;
        /// <summary>
        /// Indicate what do do with <see cref="AccessAnchor2"/>
        /// </summary>
        public MatchStyleDateTime AccessAnchorCheck2 = MatchStyleDateTime.Disable;

        /// <summary>
        /// Indicate what do do with <see cref="WriteAnchor"/>
        /// </summary>
        public MatchStyleDateTime WriteAnchorCheck1 = MatchStyleDateTime.Disable;


        /// <summary>
        /// Indicate what do do with <see cref="WriteAnchor"/>
        /// </summary>
        public MatchStyleDateTime WriteAnchorCheck2 = MatchStyleDateTime.Disable;

        /// <summary>
        /// A REGEX express that will be compared againt the <see cref="FileInfoExtract.Name"/>
        /// </summary>
        public readonly List<string> FileName = new List<string>();
        public MatchStyleString FileNameMatching = MatchStyleString.MatchAny;

        /// <summary>
        /// REGEX expressthat that's compared against <see cref="FileInfoExtract.FullName"/>
        /// </summary>
        public readonly List<string> DirectoryPath = new List<string>() ;
        public MatchStyleString DirectoryMatching = MatchStyleString.MatchAny;

        /// <summary>
        /// expression that's compared against <see cref="FileInfoExtract.FileAttributes"/>.  If equal to zero or <see cref="FileAttributes.Normal"/>, the compare is skipped
        /// </summary>
        public FileAttributes AttributeMatching1 = 0;
        /// <summary>
        /// How to comare <see cref="AttributeMatching1"/> with possible entries
        /// </summary>
        public MatchStyleFileAttributes AttribMatching1Style = MatchStyleFileAttributes.Skip;

        /// <summary>
        /// Expression that's (by default) compared to be LACKING in <see cref="FileInfoExtract.FileAttributes"/>
        /// </summary>
        public FileAttributes AttributeMatching2 =  FileAttributes.Normal;
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
        /// Additional checks are points one can add for additional checks / x
        /// </summary>

        /*
        public readonly List<CustomizedCheck> AdditionalChecks1 = new List<CustomizedCheck>();
        public MatchStyleString AdditionalChecks1Matching = MatchStyleString.MatchAny;



        public readonly List<CustomizedCheck> AdditionalChecks2 = new List<CustomizedCheck>();
        public MatchStyleString AdditionalChecks2Matching = MatchStyleString.MatchAny | MatchStyleString.Invert;*/

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
            [Obsolete("Not Implemented")]
            /// <summary>
            /// Reserved for future. Currently not used for this enum.
            /// </summary>
            ReservedUnused = 8,
            /// <summary>
            /// Disable this matching.. 
            /// </summary>
            Skip =16,
            /// <summary>
            /// Add to make the search string case sensitive.
            /// </summary>
            CaseImportant = 32
        }
    }
}

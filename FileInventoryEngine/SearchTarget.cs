using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace OdinSearchEngine
{
    /// <summary>
    /// Indicate what will be searched for.
    /// </summary>
    public class SearchTarget
    {



        public List<Regex> ConvertFileNameToRegEx()
        {
            return ConvertFileNameToRegEx(this);
        }
        /// <summary>cc
        /// Convert The passed SearchTarget file names to a matching RegEx list
        /// </summary>
        /// <param name="Target">which one to work on</param>
        /// <returns>returns a list of RegEx instances to match the possible file names in the target</returns>
        public static List<Regex> ConvertFileNameToRegEx(SearchTarget Target)
        {
            var ret = new List<Regex>();
            foreach (string s in Target.FileName)
            {
                string pattern;

                pattern = "^" + Regex.Escape(s) + "$";
                pattern = pattern.Replace("\\*", ".*").Replace("\\?", ".");
                if (pattern == "^.*$")  // this is the match anything regexpression.  returning a clear regex list disables the compare that will always be true
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
        /// Protoype for the additonal check
        /// </summary>
        /// <param name="CheckMe">file system object to check</param>
        /// <returns>Delegate returns true if the match is ok and false if it is not.</returns>
        public delegate bool CustomizedCheck(FileSystemInfo CheckMe);
        /// <summary>
        /// Make an instance of this class where it matches existing files
        /// </summary>
        public SearchTarget()
        {
            
        }
        /// <summary>
        /// A REGEX express that will be compared againt the <see cref="FileInfoExtract.Name"/>
        /// </summary>
        public readonly List<string> FileName = new List<string>();
        public MatchStyleString FileNameMatching = MatchStyleString.MatchAny;

        /// <summary>
        /// REGEX expressthat that's compared against <see cref="FileInfoExtract.ParentLocationPath"/>
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
        public MatchStyleString AttribMatching1Style = MatchStyleString.Skip;

        /// <summary>
        /// Expression that's (by default) compared to be LACKING in <see cref="FileInfoExtract.FileAttributes"/>
        /// </summary>
        public FileAttributes AttributeMatching2 =  FileAttributes.Normal;
        public MatchStyleString AttribMatching2Style = MatchStyleString.Invert | MatchStyleString.Skip;


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

        public readonly List<CustomizedCheck> AdditionalChecks1 = new List<CustomizedCheck>();
        public MatchStyleString AdditionalChecks1Matching = MatchStyleString.MatchAny;



        public readonly List<CustomizedCheck> AdditionalChecks2 = new List<CustomizedCheck>();
        public MatchStyleString AdditionalChecks2Matching = MatchStyleString.MatchAny | MatchStyleString.Invert;
        public enum MatchStyleString
        {
            /// <summary>
            /// Match at least one item in the list.  Invert means at least one of the items in the list must NOT be there
            /// </summary>
            MatchAny = 1,
            /// <summary>
            /// Match all in the list. Invert means NO entries must match or the search values.
            /// </summary>
            MatchAll = 2,
            /// <summary>
            /// binary '!' on the match i.e. must NOT match.
            /// </summary>
            Invert = 4,
            [Obsolete]
            /// <summary>
            /// This must be sucessfully match (or invert match) or the search fails.
            /// </summary>
            Critical = 8,
            /// <summary>
            /// Disable this matching.. 
            /// </summary>
            Skip =16,
            /// <summary>
            /// Add to make the search string case sensisitve. Has not effect if not comparing strings.
            /// </summary>
            CaseImportant = 32
        }
    }
}

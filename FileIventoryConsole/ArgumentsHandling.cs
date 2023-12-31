using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using OdinSearchEngine;
using OdinSearchEngine.OdinSearch_OutputConsumerTools;
using OdinSearchEngine.OdinSearch_OutputConsumerTools.StreamWriterCommonBased;
using static System.Net.Mime.MediaTypeNames;

namespace FileInventoryConsole
{
    class ArgumentUnknown : Exception
    {
        public ArgumentUnknown(string message) : base(message) { }
    }

    class ArgumentExpectsFileSystemLocation: Exception
    {
        public ArgumentExpectsFileSystemLocation(string message) : base(message) { }
    }



   
     /// <summary>
     /// This class is responsible for parsing into something the app understands
     /// </summary>
    public class ArgHandling
    {
        


        /// <summary>
        /// Trim space, '\'' chars and '\"' chars from a string
        /// </summary>
        /// <param name="QuotedString"></param>
        /// <returns></returns>
        public static string Trim(string QuotedString)
        {
            string trimquotes(string QuotedString, char c)
            {
                if ((QuotedString[0] == c) && (QuotedString[QuotedString.Length - 1] == c))
                {
                    QuotedString = QuotedString.Substring(1, QuotedString.Length - 2);
                }
                return QuotedString;
            }
            QuotedString = QuotedString.Trim();
            QuotedString = trimquotes(QuotedString, '"');
            QuotedString = trimquotes(QuotedString, 'c');
            return QuotedString;
        }
        #region string consts

        #region FileAttrib String Consts for arg handling
        const char CharReadOnly = 'R';
        const char CharHidden = 'H';
        const char CharSystem = 'S';
        const char CharDirectory = 'D';
        const char CharArch = 'A';
        // char CharNormal  = 'N'
        const char CharPPTemp = 'T';
        const char CharPPSparse = 'P';
        const char CharResparse = 'L';
        const char CharCompressed = 'C';
        const char CharOffLine = 'O';
        const char CharNotIndex = 'I';
        const char CharPPEncryted = 'E';
        const char CharPPIntegritySTream = 'N';
        const char CharPPNotScrubData = 'U';


        #endregion

        const string FlagSpecialAnyFile = "/anyfile";
        const string FlagSpecialAnyWhere = "/anywhere";
        const string FlagSetAnchor = "/anchor=";
        const string FlagSetSpecifiedUnmanagedPlugin = "/plugin=";

        const string FlagSetSpecifiedNETPlugin = "/managed=";

        const string FlagToNetPluginClassName = "/class=";
        const string FlagSetNoSignedMode = "-f";

        /// <summary>
        /// Flag to set FileAttributes1 from numbers or full attrib name
        /// </summary>
        const string FlagToSetFileAttributes = "/fileattrib=";

        const string FlagToSetFileAttributeViaDir = "/A";
        /// <summary>
        /// Flag to indicate how to compare file attributes
        /// </summary>
        const string FlagToSetHowToCompareAttrib1 = "/fileattrib_check=";
        /// <summary>
        /// Flag to set the anchor1 for last modified before this date/time
        /// </summary>
        const string FlagToSetLastModifiedNoLaterThan = "/lastmodifiedbefore=";
        /// <summary>
        /// Flag to set anchor2 to match files not last modified before this date /time
        /// </summary>
        const string FlagToSetNOLastModifiedEarlierTHan = "/nolastmodifiedbefore=";
        /// <summary>
        /// Flag to set creation1 anchor to rejust files created before
        /// </summary>
        const string FlagToSetCreationDateNoEarlier = "/notcreatedbefore=";
        
        /// <summary>
        /// FLag to set creation2 anchor to reject files created after
        /// </summary>
        const string FlagToSetCreationDateNoLater = "/notcreatedafter=";

        /// <summary>
        /// This is the flag to look for when user is choosing what they want the matches to be output as.
        /// </summary>
        const string FlagToSetOutputFormat = "/outformat=";
        /// <summary>
        /// This is the flag to look for when the user is indicating where to place the output
        /// </summary>
       const string FlagToSetOutStreamLocation = "/outstream=";
        /// <summary>
        /// This is the flag to look for that the user will user to set the string to compare against a filename.
        /// </summary>
        const string FlagToSetFileCompareString = "/filename=";
        /// <summary>
        /// This is the flag to look for that the user sets to instruct how to compare against the filename the said filename string
        /// </summary>
        const string FlagToSetHowToCompareFileString = "/filecompare=";
        /// <summary>
        /// This is the flag the user sets that can set the string to compare againt the full file location and folder path
        /// </summary>
        const string FlagToSetFullPathCompareString = "/fullname=";
        /// <summary>
        /// This is the flag to look for that the user sets to instruct how to compare against the full file name / folder path
        /// </summary>
        const string FlagToSetHowToCompareFullPathString = "/fullcompare=";

        const string FlagToSetFileWasLastAccessedBefore = "/lastaccessedbefore=";

        const string FlagToSetFileWasLastAccessedAfter = "/nolastaccessedbefore=";


        #endregion
        public enum TargetFormat
        {
            Error=0,
            CVSFile = 1,
            Unicode = 2
        }

        public enum ConsoleLines
        {
            // use the stream
            NoRedirect =0,
            Stdout = 1,
            Stderr =2
        }

        
        public  FileStream TargetStream;
        public  ConsoleLines TargetStreamHandling;

        public  TargetFormat UserFormat = TargetFormat.Error;
        Dictionary<string, object> FoundCustomArgs = new Dictionary<string, object>();
        static List<string> process_possible_multi_folder(string t)
        {
            bool InString = false;
           var result = new List<string>();
            StringBuilder stepper = new();
            for (int i = 0; i < t.Length; i++)
            {
                if (InString == false)
                {
                    if (t[i] == '\"')
                    {
                        InString = true;
                        continue;
                    }
                    else
                    {
                        if (t[i] == ';')
                        {
                            result.Add(stepper.ToString());
                            stepper.Clear();
                        }
                        else
                        {
                            stepper.Append(t[i]);
                        }
                    }
                }
                else
                {
                    if (t[i] == '\"')
                    {
                        InString = false;
                        continue;
                    }
                }
            }
            if (stepper.Length > 0)
            {
                result.Add(stepper.ToString());
            }
            return result;
        }

        /// <summary>
        /// Before parsing arguments, this resolves to a pluginfolder that we load relative plugins from.
        /// 
        /// First check is testing if the value of "ODINSEARCH_PLUGIN_FOLDER" in the enviroment both
        /// exists and is a a folder.
        /// 
        /// If that doesn't we then get the folder the current running exe is in and try to plant a
        /// "\\plugins\\" path there.   
        /// 
        /// If that fails we assign this to null and do not allow relative plugin paths.
        /// </summary>
        public DirectoryInfo PluginFolder;

        /// <summary>
        /// If set, we asscept relative plugin paths
        /// </summary>
        public bool AcceptRelativePlugin = false;

        /// <summary>
        /// ArgHandling is responsible for setting this to the matching coms class that will do the thing the user wants
        /// </summary>
        public  OdinSearch_OutputConsumerBase DesiredPlugin = null;

        /// <summary>
        /// If we are loading an external plugin, this is the complete file location
        /// </summary>
        public string ExternalPluginDll;

        /// <summary>
        /// If we are treating <see cref="ExternalPluginDll"/> has unmanaged, this is null.
        /// If we are treating it as managed, this is the class we will attempt to load.
        /// </summary>
        public string ExternalPluginName;
        public  SearchTarget SearchTarget = new SearchTarget();
        public  SearchAnchor SearchAnchor = new SearchAnchor(false);

        /// <summary>
        /// triggers on /fullname or /filename
        /// </summary>
        public bool was_filename_specified { get; private set; }
        /// <summary>
        /// triggers on /notcreatedafter or /notcreatedbefore.
        /// </summary>
        public bool was_creation_specified { get; private set; }

        /// <summary>
        /// triggers on /nolastmodifiedbefore or /nolastmodifiedbefore
        /// </summary>
        public bool was_lastchanged_specified { get; private set; }
        
        /// <summary>
        /// triggers on /lastaccessedbefore and /nolastaccessedbefore.
        /// </summary>
        public bool was_lastaccessed_specified { get; private set; }
        public bool was_filename_check_set { get; private set; }
        /// <summary>
        /// triggers on /plugin and /managed
        /// </summary>
        public bool is_plugin_set { get; private set; }
    /// <summary>
    /// triggers on /classname
    /// </summary>
    public bool PluginHasManagedClass { get; private set; }

        public bool was_outformat_set { get; private set; }
        public bool was_outstream_set { get; private set; }



    /// <summary>
    /// triggers on /file_attrib_check 
    /// </summary>
    public bool was_fileattrib_check_specified { get; private set; }

        /// <summary>
        /// triggers on /A and /fileattrib
        /// </summary>
        public bool was_fileattribs_set { get; private set; }
        public bool was_anyfile_flag_set { get; private set; }
        public bool was_start_point_set { get; private set; }
        public bool was_wholemachine_flag_set { get; private set; }

        public bool AllowUntrustedPlugin { get; private set; }

        /// <summary>
        /// display the embedded banner file and include the build version info.
        /// </summary>
        public static void DisplayBannerText()
        {
            Version self = Assembly.GetExecutingAssembly().GetName().Version;

            DisplayPackedInResourceText("Resources.Banner.txt", new string[] {self.Major.ToString(), self.Minor.ToString(), self.Build.ToString(), self.Revision.ToString()}, false, 1);
        }

        /// <summary>
        /// Display the usage file and include the assembly name
        /// </summary>
        public static void DisplayUsageText()
        {
            DisplayPackedInResourceText("Resources.UsageText.txt", new string[] { Assembly.GetCallingAssembly().GetName().Name }, true, 0);
        }
        /// <summary>
        /// Display the embedded text file to stdout (console)
        /// </summary>
        /// <param name="ResourceSuffix">trailing of the resource</param>
        /// <param name="Args">argument  if any</param>
        /// <param name="offset">byte offset to begin (banner.txt fix). Note whole file is read. We trim this many characters from the left before sending to stdout</param>
        static void DisplayPackedInResourceText(string ResourceSuffix, string[] Args, bool DisableFormatting, int offset = 0)
        {
            var Self = Assembly.GetCallingAssembly().GetManifestResourceNames();
            foreach (string s in Self)
            {
                //if (s.EndsWith("Resources.UsageText.txt"))
                if (s.EndsWith(ResourceSuffix))
                {
                    using (var SelfStream = Assembly.GetCallingAssembly().GetManifestResourceStream(s))
                    {
                        if (SelfStream != null)
                        {
                            byte[] B = new byte[SelfStream.Length];
                            SelfStream.Read(B, 0, B.Length);
                            string output = Encoding.UTF8.GetString(B);
                            if (offset != 0)
                                output = output.Substring(offset);
                            if (!DisableFormatting)
                                output = string.Format(output, Args);
                            Console.WriteLine(output);
                        }
                    }
                }
            }
            
        }
        enum ProcesFlagWildcardMode
        {
            UseFileOut = 1,
            UseDirectOut = 2
        }


        /// <summary>
        /// Process settings that start with '-'
        /// </summary>
        /// <param name="arg">from the <see cref="DoTheThing(string[])"/></param>
        /// <param name="step">where to look</param>
        /// <returns>return true if known and handled. returns false if unknown or not handled</returns>
        /// <remarks>Note false terminates additional argument processing and causes the console app to display error message</remarks>
        private bool ProcessASetting(string[] arg, int step)
        {
            string low = arg[step].ToLower();

            if (low.StartsWith(FlagSetNoSignedMode))
            {
                this.AllowUntrustedPlugin = true;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Process a argument that starts with '/'
        /// </summary>
        /// <param name="arg">from <see cref="DoTheThing(string[])"/></param>
        /// <param name="step">where to look</param>
        /// <returns>returns true if known and set ok. False if unknown.</returns>
        /// <remarks>Note false terminates additional argument processing and causes the console app to display error message</remarks>
        private bool ProcessAFlag(string[] arg, int step)
        { 
            /* this attacks the ussue by first attempting to try parsing.
             * Should this fail, we resort to just case insensitive matching hardcoded enum values.
             */
            bool DealWithComparingFileAttribEnum(string EnumString, out SearchTarget.MatchStyleFileAttributes Result)
            {
                object tmp = 0;
                Result = SearchTarget.MatchStyleFileAttributes.Skip;
                if (Enum.TryParse(typeof(SearchTarget.MatchStyleString), EnumString, out tmp))
                {



                    Result = (SearchTarget.MatchStyleFileAttributes)tmp;

                    if (SearchTarget.VerifyMatchStyleFileAttrib(Result) == false)
                    {
                        return false;
                    }



                    return true;
                }
                else
                {
                    Result = 0;
                    EnumString = EnumString.ToLower();

                    Result = 0;
                    if (EnumString.Contains("matchany"))
                    {
                        Result |= SearchTarget.MatchStyleFileAttributes.MatchAny;
                    }

                    if (EnumString.Contains("matchall"))
                    {
                        Result |= SearchTarget.MatchStyleFileAttributes.MatchAll;
                    }

                    if (EnumString.Contains("invert"))
                    {
                        Result |= SearchTarget.MatchStyleFileAttributes.Invert;
                    }

                    if (EnumString.Contains("exacting"))
                    {
                        Result |= SearchTarget.MatchStyleFileAttributes.Exacting;
                    }

                    if (EnumString.Contains("skip"))
                    {
                        Result |= SearchTarget.MatchStyleFileAttributes.Skip;
                    }

                    if ( (Result == 0) || (SearchTarget.VerifyMatchStyleFileAttrib(Result) == false))
                    {
                        return false;
                    }
                    return true;

                }
                
            }
           /* this attacks the ussue by first attempting to try parsing.
             * Should this fail, we resort to just case insensitive matching hardcoded enum values.
                */

            
            bool DealWithFileAttribEnum(string EnumString, out FileAttributes Result, bool MimicDirFallback)
            {
                object tmp = 0;
                Result = 0;
                if (Enum.TryParse(typeof(FileAttributes), EnumString, out tmp))
                {
                    Result = (FileAttributes)tmp;

                    return true;
                }
                else
                {

                    if (MimicDirFallback)
                    {
                        if (EnumString.Contains(CharReadOnly))
                        {
                            Result |= FileAttributes.ReadOnly;
                        }

                        if (EnumString.Contains(CharHidden))
                        {
                            Result |= FileAttributes.Hidden;
                        }

                        if (EnumString.Contains(CharSystem))
                        {
                            Result |= FileAttributes.System;
                        }


                        if (EnumString.Contains(CharDirectory))
                        {
                            Result |= FileAttributes.Directory;
                        }


                        if (EnumString.Contains(CharArch))
                        {
                            Result |= FileAttributes.Archive;
                        }

                        /* docs say device is reserved for future.
                        if (EnumString.Contains(CharDevice))
                        {
                            Result |= FileAttributes.Device;
                        }*/

                        /* for completless sake
                        if (EnumString.Contains(CharNormal))
                        {
                            Result |= FileAttributes.Normal;
                        }*/

                        if (EnumString.Contains(CharPPTemp))
                        {
                            Result |= FileAttributes.Temporary;
                        }

                        if (EnumString.Contains(CharPPSparse))
                        {
                            Result |= FileAttributes.SparseFile;
                        }


                        if (EnumString.Contains(CharResparse))
                        {
                            Result |= FileAttributes.ReparsePoint;
                        }


                        if (EnumString.Contains(CharCompressed))
                        {
                            Result |= FileAttributes.Compressed;
                        }

                        if (EnumString.Contains(CharOffLine))
                        {
                            Result |= FileAttributes.Offline;
                        }



                        if (EnumString.Contains(CharNotIndex))
                        {
                            Result |= FileAttributes.NotContentIndexed;
                        }

                        
                        if (EnumString.Contains(CharPPEncryted))
                        {
                            Result |= FileAttributes.Encrypted;
                        }

                        if (EnumString.Contains(CharPPIntegritySTream))
                        {
                            Result |= FileAttributes.IntegrityStream;
                        }

                        if (EnumString.Contains(CharPPNotScrubData))
                        {
                            Result |= FileAttributes.NoScrubData;
                        }
                    }
                    else
                    {
                        EnumString = EnumString.ToLower();

                        if (EnumString.Contains("directory") || EnumString.Contains("dir") )
                        {
                            Result |= FileAttributes.Directory;
                        }

                        if (EnumString.Contains("readonly") || EnumString.Contains("read"))
                        {
                            Result |= FileAttributes.ReadOnly;
                        }

                        if (EnumString.Contains("hidden") || EnumString.Contains("hid"))
                        {
                            Result |= FileAttributes.Hidden;
                        }

                        if (EnumString.Contains("system") || EnumString.Contains("sys"))
                        {
                            Result |= FileAttributes.System;
                        }

                        if (EnumString.Contains("directory") || EnumString.Contains("dir"))
                        {
                            Result |= FileAttributes.Directory;
                        }

                        if (EnumString.Contains("archive") || EnumString.Contains("arc"))
                        {
                            Result |= FileAttributes.Archive;
                        }

                        if (EnumString.Contains("device") || EnumString.Contains("dev"))
                        {
                            Result |= FileAttributes.Device;
                        }

                        if (EnumString.Contains("normal") || EnumString.Contains("norm"))
                        {
                            Result |= FileAttributes.Normal;
                        }

                        if (EnumString.Contains("temporary") || EnumString.Contains("temp"))
                        {
                            Result |= FileAttributes.Temporary;
                        }

                        if (EnumString.Contains("sparsefile") || EnumString.Contains("sparse"))
                        {
                            Result |= FileAttributes.SparseFile;
                        }

                        if (EnumString.Contains("reparsepoint") || EnumString.Contains("reparse"))
                        {
                            Result |= FileAttributes.ReparsePoint;
                        }

                        if (EnumString.Contains("compressed") || EnumString.Contains("comp"))
                        {
                            Result |= FileAttributes.Compressed;
                        }

                        if (EnumString.Contains("offline") || EnumString.Contains("off"))
                        {
                            Result |= FileAttributes.Offline;
                        }

                        if (EnumString.Contains("notcontentindexed") || EnumString.Contains("notcont"))
                        {
                            Result |= FileAttributes.NotContentIndexed;
                        }

                        if (EnumString.Contains("encrypted") || EnumString.Contains("enc"))
                        {
                            Result |= FileAttributes.Encrypted;
                        }

                        if (EnumString.Contains("integritystream") || EnumString.Contains("integ"))
                        {
                            Result |= FileAttributes.IntegrityStream;
                        }

                        if (EnumString.Contains("noscrubdata") || EnumString.Contains("noscrub"))
                        {
                            Result |= FileAttributes.NoScrubData;
                        }


                    }
                    if (Result != 0)
                    {
                        return true;
                    }

                    return false;
                }
                

            }
            
            // this first tries to strait use TryParse and then looks for the words in english
            bool DealWithStringEnum(string EnumString, out SearchTarget.MatchStyleString Result)
            {
                object tmp = 0;
                Result = SearchTarget.MatchStyleString.Skip;
                if (Enum.TryParse(typeof(SearchTarget.MatchStyleString), EnumString, out tmp))
                {

                    
                    
                    Result = (SearchTarget.MatchStyleString)tmp;
                        
                    if (SearchTarget.VerifyMatchStyleStringValue(Result) == false)
                    {
                        return false;
                    }
                    
                    

                    return true;
                }
                else
                {
                    Result = 0;
                    EnumString = EnumString.ToLower();

                    if (EnumString.Contains("matchany"))
                    {
                        Result |= SearchTarget.MatchStyleString.MatchAny;
                    }
                    if (EnumString.Contains("matchall"))
                    {
                        Result |= SearchTarget.MatchStyleString.MatchAll;
                    }

                    if (EnumString.Contains("invert"))
                    {
                        Result |= SearchTarget.MatchStyleString.Invert;
                    }

                    if (EnumString.Contains("caseimportant") || EnumString.Contains("casesensitive"))
                    {
                        Result |= SearchTarget.MatchStyleString.CaseImportant;
                    }

                    if (Result == 0)
                        return false;
                    else
                    {
                        if ((Result.HasFlag(SearchTarget.MatchStyleString.MatchAny) == false))
                        {
                            if ((Result.HasFlag(SearchTarget.MatchStyleString.MatchAll) == false))
                            {
                                Result |= SearchTarget.MatchStyleString.MatchAll;
                            }
                        }
                        return true;
                    }
                }
            }
            /// attempt parse the string. Sets result = -1 obn fail
            void DealWithDateTimeEnum(string Enumstring, out SearchTarget.MatchStyleDateTime Result)
            {
                Result = (SearchTarget.MatchStyleDateTime)(-1);
                switch (Enumstring)
                {
                    case "0": Result = SearchTarget.MatchStyleDateTime.Disable; break;
                    case "1": Result = SearchTarget.MatchStyleDateTime.NoEarlierThanThis; break;
                    case "2": Result = SearchTarget.MatchStyleDateTime.NoLaterThanThis; break;
                    default:
                        {
                            string lo = Enumstring.ToLower();
                            if (lo.StartsWith("noe") || lo.Equals("noearlierthanthis") )
                            {
                                Result = SearchTarget.MatchStyleDateTime.NoEarlierThanThis;
                            }
                            if (lo.StartsWith("nol") || lo.Equals("nolaterthanthis") )
                            {
                                Result = SearchTarget.MatchStyleDateTime.NoLaterThanThis;
                            }
                            break;
                        }
                }
            }
            
            // convert this datetime string to that
            bool HandleConvertedToDate(string source, out DateTime Output)
            {
                DateTime ret = Output = DateTime.MinValue;
                if (DateTime.TryParse(source, out ret))
                {
                    Output = ret;
                    return true;
                }
                return false;
            }

            bool HandleSingleFilePath(string Path, out string loc)
            {
                if (Path != null)
                {
                    Path = ArgHandling.Trim(Path);
                    loc = Path;
                    return true;
                }
                loc = null;
                return false;
            }
            // used to split '*.dll;*.exe' into multiple search targets for SeachTarget
            bool SplitWildcards(string target, ProcesFlagWildcardMode usefile)
            {
              target = Trim(target); 
                if (!string.IsNullOrEmpty(target)) 
                {
                    StringBuilder Part = new();
                    for (int step=0;step < target.Length; step++)
                    {
                        if (target[step] == ';')
                        {
                            if (usefile == ProcesFlagWildcardMode.UseFileOut)
                                SearchTarget.FileName.Add(Part.ToString().Trim());
                            else
                                SearchTarget.DirectoryPath.Add(Part.ToString().Trim());
                            Part.Clear();
                        }
                        else
                        {
                            Part.Append(target[step]);
                        }
                    }
                    if (Part.Length != 0)
                    {
                        if (usefile == ProcesFlagWildcardMode.UseFileOut)
                            SearchTarget.FileName.Add(Part.ToString().Trim());
                        else
                            SearchTarget.DirectoryPath.Add(Part.ToString().Trim());
                    }
                }
                return true;
            }
            string low = arg[step].ToLowerInvariant();

            // set how the user wants the output format do be done.
            if (low.StartsWith(FlagToSetOutputFormat))
            {
                
                string low_part = low[FlagToSetOutputFormat.Length..];
                switch (low_part)
                {
                    case "unicode":
                        UserFormat = TargetFormat.Unicode;
                        break;
                    case "cvsfile":
                        UserFormat = TargetFormat.CVSFile;
                        break;
                    default:
                        UserFormat = TargetFormat.Error;
                        return false;
                }
                was_outformat_set = true;
                return true;
            }

            if (low.StartsWith(FlagSpecialAnyFile))
            {
                was_anyfile_flag_set = true; return true;

            }

            // set how the user wants the output format to be sent
            if (low.StartsWith(FlagToSetOutStreamLocation))
            {
                
                string low_part = arg[step].Substring(FlagToSetOutStreamLocation.Length);
                if (low_part.Equals("stdout"))
                {
                    TargetStream = null;
                    TargetStreamHandling = ConsoleLines.Stdout;
                    was_outstream_set = true;
                    return true;
                }
                if (low_part.Equals("stderr"))
                {
                    TargetStream = null;
                    TargetStreamHandling = ConsoleLines.Stderr;
                    was_outstream_set = true;
                    return true;
                }

                if (low_part.Length > 2)
                {
                    if ( (low_part[0] == '\"') && (low_part[low_part.Length - 1] == '\"'))
                    {
                        TargetStream = File.OpenWrite(low_part.Substring(1, low_part.Length-2));
                        TargetStreamHandling = ConsoleLines.NoRedirect;
                        was_outstream_set = true;
                        return true;
                    }
                    else
                    {
                        TargetStream = File.OpenWrite(low_part);
                        TargetStreamHandling = ConsoleLines.NoRedirect;
                        was_outstream_set = true;
                        return true;
                    }

                    
                }
                return false;
            }

            // set the wildcard the user wants checked agains the name
            if (low.StartsWith(FlagToSetFileCompareString))
            {
                string low_part = arg[step].Substring(FlagToSetFileCompareString.Length);
                if ( SplitWildcards(low_part, ProcesFlagWildcardMode.UseFileOut))
                {
                    was_filename_specified = true;
                    return true;
                }
            }

            // set how the user wants the wildcard checked
            if (low.StartsWith(FlagToSetHowToCompareFileString))
            {
                string low_part = arg[step].Substring(FlagToSetHowToCompareFileString.Length);
                if (DealWithStringEnum(low_part, out SearchTarget.FileNameMatching) )
                {
                    was_filename_check_set = true;
                    return true;
                }
            }

            // set wildcard that's comparaed the user wants the full path to the file checkedd
            if (low.StartsWith(FlagToSetFullPathCompareString))
            {
                string low_part = arg[step].Substring(FlagToSetFullPathCompareString.Length);
                if (SplitWildcards(low_part, ProcesFlagWildcardMode.UseDirectOut))
                {
                    was_filename_specified = true;
                    return true;
                }
            }


            // set how the wildcard will check
            
            if (low.StartsWith(FlagToSetHowToCompareFullPathString))
            {
                string low_part = arg[step].Substring(FlagToSetHowToCompareFullPathString.Length);
                if (DealWithStringEnum(low_part, out SearchTarget.DirectoryMatching))
                {
                    was_filename_check_set = true;
                    return true;
                }
            }

            // set last modified anchor1 to be no earlier than this
            if (low.StartsWith(FlagToSetNOLastModifiedEarlierTHan))
            {
                string low_part = arg[step].Substring(FlagToSetNOLastModifiedEarlierTHan.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.WriteAnchor))
                {
                    was_lastchanged_specified = true;
                    SearchTarget.WriteAnchorCheck1 = SearchTarget.MatchStyleDateTime.NoEarlierThanThis;
                    return true;
                }
            }
            
            
            if (low.StartsWith(FlagToSetLastModifiedNoLaterThan))
            {
                string low_part =arg[step].Substring(FlagToSetLastModifiedNoLaterThan.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.WriteAnchor2))
                {
                    was_lastchanged_specified = true;
                    SearchTarget.WriteAnchorCheck2 = SearchTarget.MatchStyleDateTime.NoLaterThanThis;
                    return true;
                }
            }



            if (low.StartsWith(FlagToSetFileWasLastAccessedBefore))
            {
                string low_part = arg[step].Substring(FlagToSetFileWasLastAccessedBefore.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.AccessAnchor))
                {
                    was_lastaccessed_specified = true;
                    SearchTarget.AccessAnchorCheck1 = SearchTarget.MatchStyleDateTime.NoEarlierThanThis;
                    return true;
                }
            }

            if (low.StartsWith(FlagToSetFileWasLastAccessedAfter))
            {
                string low_part = arg[step].Substring(FlagToSetFileWasLastAccessedAfter.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.AccessAnchor2))
                {
                    was_lastaccessed_specified = true;
                    SearchTarget.AccessAnchorCheck2 = SearchTarget.MatchStyleDateTime.NoLaterThanThis;
                    return true;
                }

            }



            if (low.StartsWith(FlagToSetCreationDateNoEarlier))
            {
                string low_part = arg[step].Substring(FlagToSetCreationDateNoEarlier.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.CreationAnchor))
                {
                    was_creation_specified = true;
                    SearchTarget.CreationAnchorCheck1 = SearchTarget.MatchStyleDateTime.NoEarlierThanThis;
                    return true;
                }
            }

            if (low.StartsWith(FlagToSetCreationDateNoLater))
            {
                string low_part = arg[step].Substring(FlagToSetCreationDateNoLater.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.CreationAnchor2))
                {
                    was_creation_specified = true;
                    SearchTarget.CreationAnchorCheck2 = SearchTarget.MatchStyleDateTime.NoLaterThanThis;
                    return true;
                }

            }
            if (low.StartsWith(FlagToSetFileAttributes))
            {
                string low_part = arg[step].Substring(FlagToSetFileAttributes.Length);
                if (DealWithFileAttribEnum(low_part, out SearchTarget.AttributeMatching1, false))
                {
                    was_fileattribs_set = true;
                    return true;
                }
            }

            if (low.StartsWith(FlagToSetFileAttributeViaDir))
            {
                string low_part = arg[step].Substring(FlagToSetFileAttributeViaDir.Length);
                if (DealWithFileAttribEnum(low_part, out SearchTarget.AttributeMatching1, true))
                {
                    was_fileattribs_set = true;
                    return true;
                }
            }
            if (low.StartsWith(FlagToSetHowToCompareAttrib1))
            {
                string low_part = arg[step].Substring(FlagToSetHowToCompareAttrib1.Length);
                if (DealWithComparingFileAttribEnum(low_part, out SearchTarget.AttribMatching1Style))
                {
                    was_fileattrib_check_specified = true;
                    return true;
                }

            }

            if (low.StartsWith(FlagSetSpecifiedUnmanagedPlugin))
            {
                string low_part = arg[step].Substring(FlagSetSpecifiedUnmanagedPlugin.Length);
                ExternalPluginDll = ArgHandling.Trim(low_part);
                ExternalPluginName = null;
                is_plugin_set = true;
                this.PluginHasManagedClass = false;
                return true;
            }

            if (low.StartsWith(FlagSetSpecifiedNETPlugin))
            {
                string low_part = arg[step].Substring(FlagSetSpecifiedNETPlugin.Length);
                ExternalPluginDll = ArgHandling.Trim(low_part);
                ExternalPluginName = string.Empty;
                is_plugin_set = true;
                
                return true;
            }

            if (low.StartsWith(FlagToNetPluginClassName))
            {
                string low_part = arg[step].Substring(FlagSetSpecifiedNETPlugin.Length);
                ExternalPluginName = low_part;
                PluginHasManagedClass = true;
                return true;
            }

            if (low.StartsWith(FlagSpecialAnyWhere))
            {
                was_wholemachine_flag_set = true;
                return true;
            }

            if (low.StartsWith(FlagSetAnchor))
            {
                string low_part = arg[step].Substring(FlagSetAnchor.Length);
                low_part = Trim(low_part);
                if (Directory.Exists(low_part))
                {
                    SearchAnchor.AddAnchor(low_part);
                    was_start_point_set = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        internal static void LoadAssemblyAndGetClass(string pluginpos, string pluginname)
        {
            Assembly assembly = null;

            try
            {
                assembly = Assembly.LoadFrom(pluginpos);


            }
            catch (BadImageFormatException)
            {
                // try unmanaged
            }

        }




        private  OdinSearch_OutputConsumerBase ResolveOutFormatAndPlugins()
        {
            OdinSearch_OutputConsumerBase ret = null;
            if (!is_plugin_set)
            {
                
                
                switch (TargetStreamHandling)
                {
                    case ConsoleLines.NoRedirect:
                        switch (UserFormat)
                        {
                            case TargetFormat.Unicode:
                                ret = new OdinSearch_OutputSimpleConsole();
                                ret[OdinSearch_OutputSimpleConsole.MatchStream] = this.TargetStream;
                                ret[OdinSearch_OutputSimpleConsole.FlushAlways] = true;
                                break;
                            case TargetFormat.CVSFile:
                                ret = new OdinSearchOutputCVSWriter();
                                ret[OdinSearchOutputCVSWriter.MatchStream] = this.TargetStream;
                                ret[OdinSearchOutputCVSWriter.FlushAlways] = true;
                                break;
                        }
                        
                        

                        break;
                    case ConsoleLines.Stdout:
                        ret = new OdinSearch_OutputSimpleConsole();
                        ret[OdinSearch_OutputSimpleConsole.MatchStream] = Console.Out;
                        break;
                    case ConsoleLines.Stderr:
                        ret = new OdinSearch_OutputSimpleConsole();
                        ret[OdinSearch_OutputSimpleConsole.MatchStream] = Console.Error;
                        break;
                    default:
                        return null;
                 }
            }
            else
            {
                if (PluginHasManagedClass == false)
                {
                    ret = new OdinSearch_OutputConsumer_ExternUnmangedPlugin();
                    ret[OdinSearch_OutputConsumer_ExternUnmangedPlugin.SetDllTarget] = this.DesiredPlugin;
                }
                else
                {
                    throw new NotImplementedException("managed plugin not done yet");
                }
            }
            
            return ret;
        }

        /// <summary>
        /// After parsing with <see cref="DoTheThing(string[])"/>, this finalizez stuff
        /// </summary>
        public  void FinalizeCommands()
        {
            if (SearchAnchor == null)
            {
                SearchAnchor = new SearchAnchor();
                SearchAnchor.EnumSubFolders = true;
            }

            if (SearchTarget == null)
            {
                SearchTarget = new SearchTarget();
                
            }

            if (!was_anyfile_flag_set)
            {
                if (!was_filename_check_set)
                {
                    SearchTarget.FileName.Add(SearchTarget.MatchAnyFile);
                }

                if (!was_fileattribs_set)
                {
                    SearchTarget.AttributeMatching1 = FileAttributes.Normal;
                }

                if (!was_fileattrib_check_specified)
                {
                    if (was_fileattribs_set == true)
                    {
                        SearchTarget.AttribMatching1Style = SearchTarget.MatchStyleFileAttributes.MatchAll;
                    }
                    else
                    {
                        SearchTarget.AttribMatching1Style = SearchTarget.MatchStyleFileAttributes.Skip;
                    }

                }

                if (!was_creation_specified)
                {
                    SearchTarget.CreationAnchorCheck1 = SearchTarget.MatchStyleDateTime.Disable;
                }

                if (!was_lastaccessed_specified)
                {
                    SearchTarget.AccessAnchorCheck1 = SearchTarget.MatchStyleDateTime.Disable;
                }

                if (!was_lastchanged_specified)
                {
                    SearchTarget.WriteAnchorCheck1 = SearchTarget.MatchStyleDateTime.Disable;
                }
            }
            else
            {
                SearchTarget = new SearchTarget();
                SearchTarget.FileName.Add(SearchTarget.MatchAnyFile);
                SearchTarget.DirectoryMatching = SearchTarget.MatchStyleString.Skip;
                SearchTarget.FileNameMatching = SearchTarget.MatchStyleString.Skip;
                SearchTarget.AttributeMatching1 = SearchTarget.AttributeMatching2 = FileAttributes.Normal;
                SearchTarget.AttribMatching1Style = SearchTarget.AttribMatching2Style = SearchTarget.MatchStyleFileAttributes.Skip;
                SearchTarget.AccessAnchorCheck1 = SearchTarget.AccessAnchorCheck2 = SearchTarget.MatchStyleDateTime.Disable;
                SearchTarget.WriteAnchorCheck1 = SearchTarget.WriteAnchorCheck2 = SearchTarget.MatchStyleDateTime.Disable;
                SearchTarget.CreationAnchorCheck1 = SearchTarget.CreationAnchorCheck2 = SearchTarget.MatchStyleDateTime.Disable;
                SearchTarget.CheckFileSize = false;
                SearchTarget.DirectoryMatching = SearchTarget.MatchStyleString.Skip;
            }
          
            if (!was_outstream_set)
            {
                this.TargetStreamHandling = ConsoleLines.Stdout;
                TargetStream = null;
            }

            if (!was_outformat_set)
            {
                this.UserFormat = TargetFormat.Unicode;
            }
            else
            {

            }

            if (!was_start_point_set)
            {
                SearchAnchor = new SearchAnchor();
                SearchAnchor.EnumSubFolders = true;
            }
            DesiredPlugin = ResolveOutFormatAndPlugins();

        }
        public bool DoTheThing(string[] Args)
        {
            bool Valid = true;
            for (int step=0;step < Args.Length;step++)
            {
                if (Args[step][0] == '/')   // its a flag
                {
                    
                    if (!ProcessAFlag(Args, step))
                    {
                        Console.WriteLine("Unexpected fail handling this token \"" + Args[step] + "\"");
                        Valid = false;
                        break;
                    }
                    continue;
                }
                if (Args[step][0] == '-') // its a specicial setting
                {
                    if (!ProcessASetting(Args, step))
                    {
                        Console.WriteLine("Unexpected fail handling this token \"" + Args[step] + "\"");
                        Valid = false;
                        break;

                    }
                }
            }

            if (Valid == true)
            {
                if (PluginFolder == null)
                {
                    try
                    {
                        PluginFolder = new DirectoryInfo(Environment.GetEnvironmentVariable("ODINSEARCH_PLUGIN_FOLDER"));
                    }
                    catch (ArgumentNullException)
                    {
                        var self = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
                        PluginFolder = new DirectoryInfo(Path.Combine(self, "Plugins"));
                    }
                }
            }
            return Valid;
        }

        internal static void Usage()
        {
            DisplayUsageText();
        }
    }
}


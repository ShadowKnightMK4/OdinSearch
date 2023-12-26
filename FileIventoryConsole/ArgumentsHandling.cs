using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using OdinSearchEngine;
using OdinSearchEngine.OdinSearch_OutputConsumerTools;

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

        /// <summary>
        /// Flag to set FileAttributes1
        /// </summary>
        const string FlagToSetFileAttributes = "/fileattrib=";

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

        public  DirectoryInfo PluginFolder;
        public  OdinSearch_OutputConsumerBase DesiredPlugin = null;
        public  SearchTarget SearchTarget = new SearchTarget();
        public  SearchAnchor SearchAnchor = new SearchAnchor(false);
        
        /// <summary>1
        /// if start pos is not specified we default to all local drives
        /// </summary>
         bool was_start_specified = false;

        /// <summary>
        /// display the embedded banner file and include the build version info.
        /// </summary>
        public static void DisplayBannerText()
        {
            Version self = Assembly.GetExecutingAssembly().GetName().Version;

            DisplayPackedInResourceText("Resources.Banner.txt", new string[] {self.Major.ToString(), self.Minor.ToString(), self.Build.ToString(), self.Revision.ToString()}, 1);
        }

        /// <summary>
        /// Display the usage file and include the assembly name
        /// </summary>
        public static void DisplayUsageText()
        {
            DisplayPackedInResourceText("Resources.UsageText.txt", new string[] { Assembly.GetCallingAssembly().GetName().Name }, 0);
        }
        /// <summary>
        /// Display the embedded text file to stdout (console)
        /// </summary>
        /// <param name="ResourceSuffix">trailing of the resource</param>
        /// <param name="Args">argument  if any</param>
        /// <param name="offset">byte offset to begin (banner.txt fix). Note whole file is read. We trim this many characters from the left before sending to stdout</param>
        static void DisplayPackedInResourceText(string ResourceSuffix, string[] Args, int offset=0)
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
        private bool ProcessAFlag(string[] arg, int step)
        { 
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
                ;
            }
                bool DealWithFileAttribEnum(string EnumString, out FileAttributes Result)
            {
                object tmp = 0;
                Result = FileAttributes.Normal;
                if (Enum.TryParse(typeof(FileAttributes), EnumString, out tmp))
                {
                    Result = (FileAttributes) tmp;
                    
                    return true;
                }
                return false;
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
                return true;
            }

            // set how the user wants the output format to be sent
            if (low.StartsWith(FlagToSetOutStreamLocation))
            {
                string low_part = arg[step].Substring(FlagToSetOutStreamLocation.Length);
                if (low_part.Equals("stdout"))
                {
                    TargetStream = null;
                    TargetStreamHandling = ConsoleLines.Stdout;
                    return true;
                }
                if (low_part.Equals("stderr"))
                {
                    TargetStream = null;
                    TargetStreamHandling = ConsoleLines.Stderr;
                    return true;
                }

                if (low_part.Length > 2)
                {
                    if ( (low_part[0] == '\"') && (low_part[low_part.Length - 1] == '\"'))
                    {
                        TargetStream = File.OpenWrite(low_part.Substring(1, low_part.Length-2));
                        TargetStreamHandling = ConsoleLines.NoRedirect;
                        return true;
                    }
                    else
                    {
                        TargetStream = File.OpenWrite(low_part);
                        TargetStreamHandling = ConsoleLines.NoRedirect;
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
                    return true;
                }
            }

            // set how the user wants the wildcard checked
            if (low.StartsWith(FlagToSetHowToCompareFileString))
            {
                string low_part = arg[step].Substring(FlagToSetHowToCompareFileString.Length);
                if (DealWithStringEnum(low_part, out SearchTarget.FileNameMatching) )
                {
                    return true;
                }
            }

            // set wildcard that's comparaed the user wants the full path to the file checkedd
            if (low.StartsWith(FlagToSetFullPathCompareString))
            {
                string low_part = arg[step].Substring(FlagToSetFullPathCompareString.Length);
                if (SplitWildcards(low_part, ProcesFlagWildcardMode.UseDirectOut))
                {
                    return true;
                }
            }


            // set how the wildcard will check
            if (low.StartsWith(FlagToSetHowToCompareFullPathString))
            {
                string low_part = arg[step].Substring(FlagToSetHowToCompareFullPathString.Length);
                if (DealWithStringEnum(low_part, out SearchTarget.DirectoryMatching))
                {
                    return true;
                }
            }

            // set last modified anchor1 to be no earlier than this
            if (low.StartsWith(FlagToSetNOLastModifiedEarlierTHan))
            {
                string low_part = arg[step].Substring(FlagToSetNOLastModifiedEarlierTHan.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.WriteAnchor))
                {
                    SearchTarget.WriteAnchorCheck1 = SearchTarget.MatchStyleDateTime.NoEarlierThanThis;
                    return true;
                }
            }
            
            
            if (low.StartsWith(FlagToSetLastModifiedNoLaterThan))
            {
                string low_part =arg[step].Substring(FlagToSetLastModifiedNoLaterThan.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.WriteAnchor2))
                {
                    SearchTarget.WriteAnchorCheck2 = SearchTarget.MatchStyleDateTime.NoLaterThanThis;
                    return true;
                }
            }



            if (low.StartsWith(FlagToSetFileWasLastAccessedBefore))
            {
                string low_part = arg[step]..Substring(FlagToSetFileWasLastAccessedBefore.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.AccessAnchor))
                {
                    SearchTarget.AccessAnchorCheck1 = SearchTarget.MatchStyleDateTime.NoEarlierThanThis;
                    return true;
                }
            }

            if (low.StartsWith(FlagToSetFileWasLastAccessedAfter))
            {
                string low_part = arg[step]..Substring(FlagToSetFileWasLastAccessedAfter.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.AccessAnchor2))
                {
                    SearchTarget.AccessAnchorCheck2 = SearchTarget.MatchStyleDateTime.NoLaterThanThis;
                    return true;
                }

            }



            if (low.StartsWith(FlagToSetCreationDateNoEarlier))
            {
                string low_part = arg[step]..Substring(FlagToSetCreationDateNoEarlier.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.CreationAnchor))
                {
                    SearchTarget.CreationAnchorCheck1 = SearchTarget.MatchStyleDateTime.NoEarlierThanThis;
                    return true;
                }
            }

            if (low.StartsWith(FlagToSetCreationDateNoLater))
            {
                string low_part = arg[step].Substring(FlagToSetCreationDateNoLater.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.CreationAnchor2))
                {
                    SearchTarget.CreationAnchorCheck2 = SearchTarget.MatchStyleDateTime.NoLaterThanThis;
                    return true;
                }

            }
            if (low.StartsWith(FlagToSetFileAttributes))
            {
                string low_part = arg[step].Substring(FlagToSetFileAttributes.Length);
                if (DealWithFileAttribEnum(low_part, out SearchTarget.AttributeMatching1))
                {
                    return true;
                }
            }
            if (low.StartsWith(FlagToSetHowToCompareAttrib1))
            {
                string low_part = arg[step].Substring(FlagToSetHowToCompareAttrib1.Length);
                if (DealWithComparingFileAttribEnum(low_part, out SearchTarget.AttribMatching1Style))
                {
                    return true;
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


         OdinSearch_OutputConsumerBase DefaultSpecialPluginHandler(string name)
        {
            switch (name.ToLower())
            {
                case "consoleview":
                case "console":  return new OdinSearch_OutputSimpleConsole();

            }
            return null;
        }

        private  OdinSearch_OutputConsumerBase process_plugin(string plugin)
        {
            OdinSearch_OutputConsumerBase ret = null;
            string target_class = null;
            int loc = plugin.IndexOf(',');
            string pluginlocation;
            ret = DefaultSpecialPluginHandler(plugin);
            if (ret != null)
            {
                return ret;
            }
            if (loc != -1)
            {
                pluginlocation = plugin.Substring(0, loc);
                target_class = plugin.Substring(loc + 1);
            }
            else
            {
                pluginlocation = plugin.Substring(0, loc);
                target_class = null;
            }
            return ret;
        }

        public  void ApplyDefaults()
        {
            if (SearchAnchor == null)
            {
                SearchAnchor = new SearchAnchor();
                SearchAnchor.EnumSubFolders = true;
            }
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


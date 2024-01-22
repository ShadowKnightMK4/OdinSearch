using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using OdinSearchEngine;
using OdinSearchEngine.OdinSearch_OutputConsumerTools;
using OdinSearchEngine.OdinSearch_OutputConsumerTools.CmdProcessorTools;
using OdinSearchEngine.OdinSearch_OutputConsumerTools.ExternalBased;
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


    internal static class ArgHandling_tools
    {

    }


    /// <summary>
    /// With the uniquieness of the /explain flag, this class is dedicated to handle the implemntaiton of the code
    /// that does the /explain. Note that <see cref="ArgHandling.DisplayExplain()"/> calls <see cref="ArgHandlingExplainHandling.DisplayExplain(SearchTarget, SearchAnchor)"/> here
    /// </summary>
    internal static class ArgHandlingExplainHandling
    {

        /// <summary>
        /// Indicates if we're checking <see cref="SearchTarget.FileName"/> or <see cref="SearchTarget.DirectoryPath"/>
        /// </summary>
        enum ExplainStringMatchFlag
        {
            FileMode = 1,
            DirMode = 2
        };

        /// <summary>
        /// Controls the string that <see cref="ExplainDateTimeMatch(SearchTarget, ExplainDateTimeFlag)"/> will send out
        /// </summary>
        enum ExplainDateTimeFlag
        {
            Creation1 = 1,
            Access1 = 3,
            Modified1 = 5,

        }


        /// <summary>
        /// Explain to stdout what this seach will do.
        /// </summary>
        /// <param name="SearchTarget"></param>
        /// <param name="SearchAnchor"></param>
        public static void DisplayExplain(SearchTarget SearchTarget, SearchAnchor SearchAnchor, ArgHandling that)
        {

            Console.Write("We are searching for ");
            ExplainFileAttibMatch(SearchTarget);
            ExplainStringMatch(ExplainStringMatchFlag.FileMode, SearchTarget);
            ExplainStringMatch(ExplainStringMatchFlag.DirMode, SearchTarget);
            ExplainDateTimeMatch(SearchTarget, ExplainDateTimeFlag.Creation1);
            ExplainDateTimeMatch(SearchTarget, ExplainDateTimeFlag.Access1);
            ExplainDateTimeMatch(SearchTarget, ExplainDateTimeFlag.Modified1);
            ExplainAnchors(SearchAnchor);
            ExplainSize(SearchTarget);
            ExplainOutputConsumer(that);
        }

        static string[] size_strings = { "B", "KB", "MB", "GB", "PB", "EB", "PB" };
        static string ExplainSizeVal(long val)
        {
            long vis_display = val;
            int offset = 0;
            while ( (vis_display > 1024) && (offset < size_strings.Length) ) 
            {
                vis_display /= 1024;
                offset++;
            }
            return vis_display.ToString() + size_strings[offset];
        }

        static void ExplainOutputConsumer(ArgHandling that)
        {
            if (!that.WasOutStreamSet)
            {
                if ( (that.WasNetPluginSet == that.WasUnmanagedPluginSet) && (that.WasNetPluginSet == true))
                {
                    Console.WriteLine("The match can't proceed due to setting multiple plugins with /managed and /plugin. Pick one please.");
                    return;
                }
                if (that.WasUnmanagedPluginSet)
                {
                    Console.WriteLine(string.Format("The unmanaged plugin was set ok with /plugin argument to load {0}.", that.ExternalPluginDll));
                }
                if (that.WasNetPluginSet)
                {
                    Console.WriteLine(string.Format("The .NET based plugin was set ok with /managed argument to load {0}.", that.ExternalPluginDll));
                    if (that.PluginHasClassNameSet)
                    {
                        Console.WriteLine(string.Format("The .NET plugin controller object is set to {0}.", that.ExternalPluginName));
                    }
                    else
                    {
                        Console.WriteLine("There's a problem. The .NET plugin controller was not specified with /class. That's required");
                    }
                }

            }
            else
            {
                if ((that.WasNetPluginSet == that.WasUnmanagedPluginSet) && (that.WasNetPluginSet == true))
                {
                    Console.WriteLine("The match can't proceed due to setting multiple plugins with /managed and /plugin. Pick one please.");
                    return;
                }

                string outtarget;
                string outformat;
                switch (that.TargetStreamHandling)
                {
                    case ArgHandling.ConsoleLines.NoRedirect: outtarget = that.TargetStream.Name; break;
                    case ArgHandling.ConsoleLines.Stderr: outtarget = "stderr on the console"; break;
                    case ArgHandling.ConsoleLines.Stdout: outtarget = "stdout on the console"; break;
                    default: outtarget = "ERROR ERROR"; break;
                }

                switch (that.UserFormat)
                {
                    case ArgHandling.TargetFormat.CSVFile: outformat = "Excel Comma delimited file"; break;
                    case ArgHandling.TargetFormat.Unicode: outformat = "Unicode Text File"; break;
                    default: outformat = "ERROR ERROR"; break;
                }

                Console.WriteLine(string.Format("The match is to output to {0} with the format off {1}.", outtarget, outformat));
            }
        }
        static void ExplainSize(SearchTarget Target)
        {
            
            if (!Target.CheckFileSize)
            {
                Console.WriteLine("The match does not care about the file size (if any).");
            }
            else
            {
                Console.WriteLine(string.Format("The match is looking for file sizes between ({0}, {1})", ExplainSizeVal(Target.FileSizeMin), ExplainSizeVal(Target.FileSizeMax)));
            }
        }

        static void ExplainAnchors(SearchAnchor Anchor)
        {
            if (Anchor.roots.Count == 0)
            {
                Console.WriteLine("Warning: The search does not have a defined starting point. Don't forget to specify one with /anywhere or /anchor");
            }
            if (Anchor.roots.Count != 0)
            {
                Console.Write("The match will start looking in ");
                if (Anchor.roots.Count > 1)
                {
                    Console.Write("these locations (");
                }
                else
                {
                    Console.Write("this location (");
                }
                for (int i = 0; i < Anchor.roots.Count; i++)
                {
                    Console.Write(Anchor.roots[i].FullName.ToString());
                    if (!(i == Anchor.roots.Count - 1))
                    {
                        Console.Write(", ");
                    }
                }
                Console.WriteLine(")");


            }
        }
        /// <summary>
        /// This makes assumes that the arg processing will set the date time currently as its does, should that change, this will need to be updated 
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="mode"></param>
        static void ExplainDateTimeMatch(SearchTarget Target, ExplainDateTimeFlag mode)
        {
            string DateTimeType;
            SearchTarget.MatchStyleDateTime ActionMode1;
            SearchTarget.MatchStyleDateTime ActionMode2;
            DateTime Val1;
            DateTime Val2;
            void ProcessActionMode(bool OneIfTrue)
            {
                string modestring;
                string finaldate;

                if (OneIfTrue)
                {
                    if (ActionMode1 == SearchTarget.MatchStyleDateTime.NoEarlierThanThis)
                    {
                        modestring = "no earlier";
                    }
                    else
                    {
                        modestring = "no later";
                    }
                    finaldate = Val1.ToString();
                }
                else
                {
                    if (ActionMode2 == SearchTarget.MatchStyleDateTime.NoEarlierThanThis)
                    {
                        modestring = "no earlier";
                    }
                    else
                    {
                        modestring = "no later";
                    }
                    finaldate = Val2.ToString();
                }


                Console.Write(string.Format("wants file items that were {0} {1} than {2}.", DateTimeType, modestring, finaldate));
            }
            switch (mode)
            {
                case ExplainDateTimeFlag.Access1:
                    DateTimeType = "accessed";
                    Val1 = Target.AccessAnchor;
                    Val2 = Target.AccessAnchor2;
                    ActionMode1 = Target.AccessAnchorCheck1;
                    ActionMode2 = Target.AccessAnchorCheck2;
                    break;
                case ExplainDateTimeFlag.Creation1:
                    DateTimeType = "created";
                    Val1 = Target.CreationAnchor;
                    Val2 = Target.CreationAnchor2;
                    ActionMode1 = Target.CreationAnchorCheck1;
                    ActionMode2 = Target.CreationAnchorCheck2;
                    break;
                case ExplainDateTimeFlag.Modified1:
                    DateTimeType = "last changed";
                    Val1 = Target.WriteAnchor;
                    Val2 = Target.WriteAnchor2;
                    ActionMode1 = Target.WriteAnchorCheck1;
                    ActionMode2 = Target.WriteAnchorCheck2;
                    break;
                default:
                    throw new NotImplementedException("Error: Assumed SearchTarget.MatchStyleDateTime was only 3 possible states. If that's nolonger true, the explain code for datetime needs to be updated.");
            }

            Console.Write("The match ");


            if ((ActionMode1 == ActionMode2) && (ActionMode1 == SearchTarget.MatchStyleDateTime.Disable))
            {
                Console.WriteLine(string.Format("does not care about when the file system item was {0}.", DateTimeType));
            }
            else
            {
                ProcessActionMode(false);
                ProcessActionMode(true);
            }

        }
        static void ExplainFileAttibMatch(SearchTarget Target)
        {
            if (Target.AttribMatching1Style.HasFlag(SearchTarget.MatchStyleFileAttributes.Skip) ||
                ((Target.AttributeMatching1 == FileAttributes.Normal) || (Target.AttributeMatching1 == 0)))
            {
                Console.Write("file or directories with no special attributes set.");
            }
            else
            {
                if (Target.AttributeMatching1.HasFlag(FileAttributes.Directory))
                {
                    Console.Write("directories ");
                }
                else
                {
                    Console.Write("files ");
                }

                string ex = string.Empty;
                if (Target.AttribMatching1Style.HasFlag(SearchTarget.MatchStyleFileAttributes.Exacting))
                    ex = "ONLY";
                Console.WriteLine(string.Format("with {0} these attributes as defined in the system: ({1}).", ex, Target.AttributeMatching1.ToString()));

            }
        }
        static void ExplainStringMatch(ExplainStringMatchFlag String, SearchTarget Target)
        {
            List<string> GenMatch;
            SearchTarget.MatchStyleString GenStyle;
            if (String == ExplainStringMatchFlag.FileMode)
            {
                GenMatch = Target.FileName;
                GenStyle = Target.FileNameMatching;
            }
            else
            {
                GenMatch = Target.DirectoryPath;
                GenStyle = Target.DirectoryMatching;
            }
            if ((GenStyle == SearchTarget.MatchStyleString.Skip) ||
                (GenMatch.Count == 0) ||
                (GenMatch.Contains(SearchTarget.MatchAnyFile)))
            {
                Console.Write("The match does not care about comparing something ");
                if (String == ExplainStringMatchFlag.FileMode)
                {
                    Console.WriteLine("against the file system item's name at all.");
                }
                else
                {
                    Console.WriteLine("against the file sytem item's exact location and name at all.");
                }
            }
            else
            {
                Console.Write("The ");
                if (GenStyle.HasFlag(SearchTarget.MatchStyleString.Invert))
                {
                    Console.Write("negative ");
                }
                if (String == ExplainStringMatchFlag.FileMode)
                {
                    Console.Write("file ");
                }
                else
                {
                    Console.Write("full ");
                }


                Console.Write("location match will compare the item's name against ");
                if (GenMatch.Count == 1)
                    Console.Write("this string (");
                else
                    Console.Write("these strings (");
                for (int i = 0; i < GenMatch.Count; i++)
                {
                    Console.Write(GenMatch[i].ToString());
                    if (!(i == GenMatch.Count - 1))
                    {
                        Console.Write(", ");
                    }
                }
                Console.WriteLine("). ");

                Console.Write("The ");
                if (String == ExplainStringMatchFlag.FileMode)
                {
                    Console.Write("file ");
                }
                else
                {
                    Console.Write("full ");
                }

                Console.Write("match ");
                if (GenStyle.HasFlag(SearchTarget.MatchStyleString.CaseImportant))
                {
                    Console.Write("is case SENSITIVE, and ");
                }
                else
                {
                    Console.Write("does not case about case of letters, and ");
                }

                Console.Write("must match ");
                if (GenStyle.HasFlag(SearchTarget.MatchStyleString.MatchAll))
                {
                    Console.Write("all ");
                }
                else
                {
                    Console.Write("any ");
                }

                Console.WriteLine("of the search strings");
            }

        }
    }
   
     /// <summary>
     /// This class is responsible for parsing into something the app understands
     /// </summary>
    public class ArgHandling
    {
        
        #region STATIC TOOLS

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
        #endregion
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

        #region special flags
        /// <summary>
        /// Special flag to act as a shortcut to match any filename
        /// </summary>
        const string FlagSpecialAnyFile = "/anyfile";
        /// <summary>
        /// Special flag to act as a shortcut to match any locaiton
        /// </summary>
        const string FlagSpecialAnyWhere = "/anywhere";

        /// <summary>
        /// Default is just top level. Include this flag to op into looking at sub folders too.
        /// </summary>
        const string FlagEnumSubfolder = "/subfolders";
        #endregion

        const string FlagSetAction = "/action=";
        /// <summary>
        /// This string is used set if <see cref="OdinSearch_OutputConsole"/> will output only the name. Ignored if not using that class. Currently only for outputing as unicode text
        /// </summary>

        const string FlagUnicodeSet_StrictFileName = "-justname";

        /// <summary>
        /// String used to check for setting anchors.
        /// </summary>

        const string FlagSetAnchor = "/anchor=";

        #region PLUGINS
        /// <summary>
        /// an unmanaged plugin is one that exports a C level linkage.
        /// </summary>
        const string FlagSetSpecifiedUnmanagedPlugin = "/plugin=";

        const string FlagSetSpecifiedNETPlugin = "/managed=";

        const string FlagToNetPluginClassName = "/class=";
        const string FlagSetNoSignedMode = "-f";
        #endregion
        /// <summary>
        /// Flag to set FileAttributes1 from numbers or full attrib name
        /// </summary>
        const string FlagToSetFileAttributes = "/fileattrib=";

        /// <summary>
        /// flag to set file attributes 1 via a 'dir' command style
        /// </summary>
        const string FlagToSetFileAttributeViaDir = "/a";
        /// <summary>
        /// Flag to indicate how to compare file attributes
        /// </summary>
        const string FlagToSetHowToCompareAttrib1 = "/fileattrib_check=";
        /// <summary>
        /// Flag to set the anchor2 for last modified before this date/time
        /// <remarks>The code handling this flag should set <see cref="SearchTarget.WriteAnchor2"/></remarks> and <see cref="SearchTarget.WriteAnchorCheck2"/>
        /// </summary>
        const string FlagToSetLastModifiedNoLaterThan = "/lastmodifiedbefore=";
        /// <summary>
        /// Flag to set anchor1 to match files not last modified before this date /time
        /// <remarks>The code handling this flag should set <see cref="SearchTarget.WriteAnchor"/></remarks> and <see cref="SearchTarget.WriteAnchorCheck1"/>
        /// </summary>
        const string FlagToSetNOLastModifiedEarlierTHan = "/nolastmodifiedbefore=";
        /// <summary>
        /// Flag to set creation1 anchor to rejust files created before
        /// <remarks>The code handling this flag should set <see cref="SearchTarget.CreationAnchor"/> and <see cref="SearchTarget.CreationAnchorCheck1"/></remarks>
        /// </summary>
        const string FlagToSetCreationDateNoEarlier = "/notcreatedbefore=";

        /// <summary>
        /// FLag to set creation2 anchor to reject files created after
        /// /// <remarks>The code handling this flag should set <see cref="SearchTarget.CreationAnchor"/> and <see cref="SearchTarget.CreationAnchorCheck2"/></remarks>
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

        /// <summary>
        /// This is the flag that's used to set to look for files that weren't accessed before this time.
        /// <remarks>code handling this should set <see cref="SearchTarget.AccessAnchor"/> to the date time and <see cref="SearchTarget.AccessAnchorCheck1"/> to <see cref="SearchTarget.MatchStyleDateTime.NoEarlierThanThis"/></remarks> 
        /// </summary>
        const string FlagToSetFileWasLastAccessedBefore = "/lastaccessedbefore=";

        /// <summary>
        /// This is the flag that's used to set to look for files that weren't accessed before this time.
        /// <remarks>code handling this should set <see cref="SearchTarget.AccessAnchor2"/> to the date time and <see cref="SearchTarget.AccessAnchorCheck2"/> to <see cref="SearchTarget.MatchStyleDateTime.NoLaterThanThis"/></remarks> 
        /// </summary>
        const string FlagToSetFileWasLastAccessedAfter = "/nolastaccessedbefore=";


        const string FlagToSetMinFileSize = "/minfilesize=";

        const string FlagToSetMaxFileSize = "/maxfilesize=";
        /// <summary>
        /// This special flag is used to generate in english what the search will do.
        /// </summary>
        const string FlagToSetExplainSettingsToUser = "/explain";
        #endregion

        /// <summary>
        /// this enum is the encoding we emit to the output
        /// </summary>
        public enum TargetFormat
        {
            // error, should fail
            Error=0,
            // the excal CSV fle
            CSVFile = 1,
            // unicode text
            Unicode = 2
        }


        /// <summary>
        /// Due to an issue of figuring how to seemlessly use Console.out vs a stream.
        /// This is an enum to control which to use
        /// </summary>
        public enum ConsoleLines
        {
            /// <summary>
            /// use the stream set by <see cref="TargetStream"/>
            /// </summary>
            NoRedirect =0,
            /// <summary>
            /// Use stdout
            /// </summary>
            Stdout = 1,

            /// <summary>
            /// use stderr
            /// </summary>
            Stderr =2
        }

        
        /// <summary>
        /// Can be set to specify where to send matching output
        /// </summary>
        public  FileStream TargetStream;

        /// <summary>
        /// Controls how to interact with <see cref="TargetStream"/>
        /// </summary>
        public  ConsoleLines TargetStreamHandling;

        /// <summary>
        /// Indicates which format to send to <see cref="TargetStream"/>
        /// </summary>
        public  TargetFormat UserFormat = TargetFormat.Error;
        Dictionary<string, object> FoundCustomArgs = new Dictionary<string, object>();
        

        /// <summary>
        /// TODO:
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
        /// If set, we accept relative plugin paths
        /// </summary>
        public bool AcceptRelativePlugin = false;

        /// <summary>
        /// Once done, this is the Coms class to use in the search
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

        public enum ActionCommand
        {
            /// <summary>
            /// Nothing set
            /// </summary>
            None = 0,
            /// <summary>
            /// On Windows Cmd.exe
            /// </summary>
            CmdShell = 1,
            /// <summary>
            /// On Linux Bash
            /// </summary>
            BashShell = CmdShell,
            /// <summary>
            /// On Windows. PowerShell
            /// </summary>
            PowerShell = 2
        }

        public ActionCommand UserAction { get; private set; }
        public bool WasActionSet { get; private set; }

        /// <summary>
        /// Is set to be what to search for on finish
        /// </summary>
        public  SearchTarget SearchTarget = new SearchTarget();
        /// <summary>
        /// is set to be what to serach fon on finish
        /// </summary>
        public  SearchAnchor SearchAnchor = new SearchAnchor(false);

        /// <summary>
        /// triggers on /fullname or /filename
        /// </summary>
        public bool WasFileNameSet { get; private set; }
        /// <summary>
        /// triggers on /notcreatedafter or /notcreatedbefore.
        /// </summary>
        public bool WasCreationDateSet { get; private set; }

        /// <summary>
        /// triggers on /nolastmodifiedbefore or /nolastmodifiedbefore
        /// </summary>
        public bool WasLastChangedDateSet { get; private set; }
        
        /// <summary>
        /// triggers on /lastaccessedbefore and /nolastaccessedbefore.
        /// </summary>
        public bool WasLastAccessDateSet { get; private set; }
        
        /// <summary>
        /// triggers on /managed flag being found
        /// </summary>
        public bool WasNetPluginSet { get; private set; }

        /// <summary>
        /// /managed
        /// </summary>
        public bool WasUnmanagedPluginSet { get; private set; }
        /// <summary>
        /// triggers on /class
        /// </summary>
        public bool PluginHasClassNameSet { get; private set; }

        
        public bool WasOutFormatSet { get; private set; }
        public bool WasOutStreamSet { get; private set; }



    /// <summary>
    /// triggers on /file_attrib_check 
    /// </summary>
    public bool was_fileattrib_check_specified { get; private set; }

        /// <summary>
        /// triggers on /A and /fileattrib
        /// </summary>
        public bool was_fileattribs_set { get; private set; }

        /// <summary>
        /// This flag triggers on /anyfile set
        /// </summary>
        public bool WasAnyFileSet { get; private set; }

        /// <summary>
        /// This flag triggers on was /anchor= or was /anywhere set
        /// </summary>
        public bool WasStartPointSet { get; private set; }

        /// <summary>
        /// This flag trigers on /anywhere flag.
        /// </summary>
        public bool WasWholeMachineSet { get; private set; }


        /// <summary>
        /// This flag controls if we are go for attempting unsigned plugin loading and has different defaults if we're working with DEBUG vs RELEASE
        /// For DEBUG builds default is yes.
        /// For RELEASE builds default is no.
        /// </summary>
        public bool AllowUntrustedPlugin
        {
            get => BackingAllowUnTrusted;
            private set => BackingAllowUnTrusted = value;
        }
#if DEBUG
        private bool BackingAllowUnTrusted = true;
#else
        private bool BackingAllowUnTrusted = false;
#endif

        /// <summary>
        /// did the user note they want only just the matching location?
        /// </summary>
        public bool FlagJustFileName { get; private set; }

        /// <summary>
        /// does the user want to go into each subfolder found too.  /subfolders
        /// </summary>
        public bool WantSubFoldersAlso { get; private set; }

        /// <summary>
        /// Has the user specified how to compare filename or directory paths aka /filecompare =
        /// </summary>
        public bool WasFileCompareSet { get; private set; }


        /// <summary>
        ///  Does the user want an explaination of settings?
        /// </summary>
        public bool WantUserExplaination { get; private set; }
        



        /// <summary>
        /// This is used to emit a string to the console that explains what we're seaching for. Note /explain 
        /// <remarks>Call *AFTER* calling <see cref="FinalizeCommands"/>
        /// </summary>
        public void DisplayExplain()
        {
            ArgHandlingExplainHandling.DisplayExplain(SearchTarget, SearchAnchor, this);
        }
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
            DisplayPackedInResourceText("Resources.UsageText.txt", new string[] { Assembly.GetCallingAssembly().GetName().Name }, true, 1);
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

        /// <summary>
        /// There is a bit of common code in /filename and /fullname comparing. This controls the mode and where to stash results
        /// </summary>
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

            if (low.StartsWith(FlagUnicodeSet_StrictFileName))
            {
                this.FlagJustFileName = true;
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

            // deal with str that's an int or something like 5MB or 2KB or 20GB
            bool DealWithFileSizeParsing(string str, out long finalSize)
            {
                long ret = 0;
                if (long.TryParse(str, out ret))
                {
                    finalSize = ret;
                    return true;
                }
                else
                {
                    str = str.Trim().ToLower();
                    if (str.Length > 2)
                    {
                        if (str.EndsWith("kb"))
                        {
                            str = str.Substring(0, str.Length - 2);
                            if (long.TryParse(str, out ret))
                            {
                                finalSize = ret*1024;
                                return true;
                            }
                        }

                        if (str.EndsWith("mb"))
                        {
                            str = str.Substring(0, str.Length - 2);
                            if (long.TryParse(str, out ret))
                            {
                                finalSize = ret * 1024 * 1024;
                                return true;
                            }
                        }

                        if (str.EndsWith("gb"))
                        {
                            str = str.Substring(0, str.Length - 2);
                            if (long.TryParse(str, out ret))
                            {
                                finalSize = ret * 1024 * 1024 * 1024;
                                return true;
                            }
                        }

                        if (str.EndsWith("tb"))
                        {
                            str = str.Substring(0, str.Length - 2);
                            if (long.TryParse(str, out ret))
                            {
                                finalSize = ret * 1024 * 1024 * 1024 *1024;
                                return true;
                            }
                        }

                        if (str.EndsWith("pb"))
                        {
                            str = str.Substring(0, str.Length - 2);
                            if (long.TryParse(str, out ret))
                            {
                                finalSize = ret * 1024 * 1024 * 1024 * 1024 * 1024;
                                return true;
                            }
                        }
                        finalSize = 0;
                        return false;
                    }
                    else
                    {
                        finalSize = 0;
                        return false;
                    }
                }
            }
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

            bool DealWithAction(string Action)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    switch (Action)
                    {
                        case "bash":
                            UserAction = ActionCommand.BashShell;
                            WasActionSet = true;
                            return true;
                        default: return false;
                    }

                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    switch (Action)
                    {
                        case "cmd":
                        case "cmdshell":
                            UserAction = ActionCommand.CmdShell;
                            WasActionSet = true;
                            return true;
                        case "powershell":
                        case "ps":
                            UserAction = ActionCommand.PowerShell;
                            WasActionSet = true;
                            return true;
                        default: return false;
                    }
                }

                return false;
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

            bool HandlePossibleMultipleFilePath(string source, SearchAnchor r)
            {
                if (string.IsNullOrEmpty(source))
                {
                    return false;
                }
                else
                {
                    StringBuilder sb = new();
                    for (int i = 0; i < source.Length;i++)
                    {
                        if (source[i] == ';')
                        {
                            sb.Replace(sb.ToString(),Trim(sb.ToString()));
                            r.AddAnchor(sb.ToString());
                            sb.Clear();
                        }
                        else
                        {
                            sb.Append(source[i]);



                        }
                        
                    }
                    return true;
                }
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
                    case "csvfile":
                        UserFormat = TargetFormat.CSVFile;
                        break;
                    default:
                        UserFormat = TargetFormat.Error;
                        return false;
                }
                WasOutFormatSet = true;
                return true;
            }

            if (low.StartsWith(FlagSpecialAnyFile))
            {
                WasAnyFileSet = true; return true;

            }

            // set how the user wants the output format to be sent
            if (low.StartsWith(FlagToSetOutStreamLocation))
            {
                
                string low_part = arg[step].Substring(FlagToSetOutStreamLocation.Length);
                if (low_part.Equals("stdout"))
                {
                    TargetStream = null;
                    TargetStreamHandling = ConsoleLines.Stdout;
                    WasOutStreamSet = true;
                    return true;
                }
                if (low_part.Equals("stderr"))
                {
                    TargetStream = null;
                    TargetStreamHandling = ConsoleLines.Stderr;
                    WasOutStreamSet = true;
                    return true;
                }

                if (low_part.Length > 2)
                {
                    if ( (low_part[0] == '\"') && (low_part[low_part.Length - 1] == '\"'))
                    {
                        TargetStream = File.OpenWrite(low_part.Substring(1, low_part.Length-2));
                        TargetStreamHandling = ConsoleLines.NoRedirect;
                        WasOutStreamSet = true;
                        return true;
                    }
                    else
                    {
                        TargetStream = File.OpenWrite(low_part);
                        TargetStreamHandling = ConsoleLines.NoRedirect;
                        WasOutStreamSet = true;
                        return true;
                    }

                    
                }
                return false;
            }

            if (low.StartsWith(FlagToSetExplainSettingsToUser))
            {
                WantUserExplaination = true;
                return true;
            }

            // set the wildcard the user wants checked agains the name
            if (low.StartsWith(FlagToSetFileCompareString))
            {
                string low_part = arg[step].Substring(FlagToSetFileCompareString.Length);
                if ( SplitWildcards(low_part, ProcesFlagWildcardMode.UseFileOut))
                {
                    WasFileNameSet = true;
                    return true;
                }
            }

            if (low.StartsWith(FlagToSetExplainSettingsToUser))
            {
                this.WantUserExplaination = true;
            }

            if (low.StartsWith(FlagEnumSubfolder))
            {
                SearchAnchor.EnumSubFoldersDefault = true;
                this.WantSubFoldersAlso = true;
                return true;
            }

            // set how the user wants the wildcard checked
            if (low.StartsWith(FlagToSetHowToCompareFileString))
            {
                string low_part = arg[step].Substring(FlagToSetHowToCompareFileString.Length);
                if (DealWithStringEnum(low_part, out SearchTarget.FileNameMatching) )
                {
                    WasFileCompareSet = true;
                    return true;
                }
            }

            // set wildcard that's comparaed the user wants the full path to the file checkedd
            if (low.StartsWith(FlagToSetFullPathCompareString))
            {
                string low_part = arg[step].Substring(FlagToSetFullPathCompareString.Length);
                if (SplitWildcards(low_part, ProcesFlagWildcardMode.UseDirectOut))
                {
                    WasFileNameSet = true;
                    return true;
                }
            }


            // set how the wildcard will check
            
            if (low.StartsWith(FlagToSetHowToCompareFullPathString))
            {
                string low_part = arg[step].Substring(FlagToSetHowToCompareFullPathString.Length);
                if (DealWithStringEnum(low_part, out SearchTarget.DirectoryMatching))
                {
                    WasFileNameSet = true;
                    return true;
                }
            }

            // set last modified anchor1 to be no earlier than this
            if (low.StartsWith(FlagToSetNOLastModifiedEarlierTHan))
            {
                string low_part = arg[step].Substring(FlagToSetNOLastModifiedEarlierTHan.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.WriteAnchor))
                {
                    WasLastChangedDateSet = true;
                    SearchTarget.WriteAnchorCheck1 = SearchTarget.MatchStyleDateTime.NoEarlierThanThis;
                    return true;
                }
            }
            
            // we should set WRiteAnchor2 and WriteAnchor2Check for this one.
            
            if (low.StartsWith(FlagToSetLastModifiedNoLaterThan))
            {
                string low_part =arg[step].Substring(FlagToSetLastModifiedNoLaterThan.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.WriteAnchor2))
                {
                    WasLastChangedDateSet = true;
                    SearchTarget.WriteAnchorCheck2 = SearchTarget.MatchStyleDateTime.NoLaterThanThis;
                    return true;
                }
            }



            // this should set Access Check1 and Access Anchor
            if (low.StartsWith(FlagToSetFileWasLastAccessedBefore))
            {
                string low_part = arg[step].Substring(FlagToSetFileWasLastAccessedBefore.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.AccessAnchor))
                {
                    WasLastAccessDateSet = true;
                    SearchTarget.AccessAnchorCheck1 = SearchTarget.MatchStyleDateTime.NoEarlierThanThis;
                    return true;
                }
            }

            // this should set access anchor2 and access check 2
            if (low.StartsWith(FlagToSetFileWasLastAccessedAfter))
            {
                string low_part = arg[step].Substring(FlagToSetFileWasLastAccessedAfter.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.AccessAnchor2))
                {
                    WasLastAccessDateSet = true;
                    SearchTarget.AccessAnchorCheck2 = SearchTarget.MatchStyleDateTime.NoLaterThanThis;
                    return true;
                }

            }



            if (low.StartsWith(FlagToSetCreationDateNoEarlier))
            {
                string low_part = arg[step].Substring(FlagToSetCreationDateNoEarlier.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.CreationAnchor))
                {
                    WasCreationDateSet = true;
                    SearchTarget.CreationAnchorCheck1 = SearchTarget.MatchStyleDateTime.NoEarlierThanThis;
                    return true;
                }
            }

            if (low.StartsWith(FlagToSetCreationDateNoLater))
            {
                string low_part = arg[step].Substring(FlagToSetCreationDateNoLater.Length);
                if (HandleConvertedToDate(low_part, out SearchTarget.CreationAnchor2))
                {
                    WasCreationDateSet = true;
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
                WasUnmanagedPluginSet = true;
                this.PluginHasClassNameSet = false;
                return true;
            }

            if (low.StartsWith(FlagSetSpecifiedNETPlugin))
            {
                string low_part = arg[step].Substring(FlagSetSpecifiedNETPlugin.Length);
                ExternalPluginDll = ArgHandling.Trim(low_part);
                WasNetPluginSet = true;
                
                return true;
            }

            if (low.StartsWith(FlagToNetPluginClassName))
            {
                string low_part = arg[step].Substring(FlagToNetPluginClassName.Length);
                ExternalPluginName = low_part;
                PluginHasClassNameSet = true;
                return true;
            }

            if (low.StartsWith(FlagSpecialAnyWhere))
            {
                WasWholeMachineSet = true;
                return true;
            }

            if (low.StartsWith(FlagToSetMinFileSize))
            {
                long filesize = 0;
                string low_part = arg[step].Substring(FlagToSetMinFileSize.Length);
                

                if (long.TryParse(low_part, out filesize))
                {
                    SearchTarget.FileSizeMin = filesize;
                    SearchTarget.CheckFileSize = true;
                    return true;
                }
                else
                {
                    if (DealWithFileSizeParsing(low_part, out filesize))
                    {
                        SearchTarget.FileSizeMin = filesize;
                        SearchTarget.CheckFileSize = true;
                        return true;
                    }
                }

                return false;
            }

            if (low.StartsWith(FlagToSetMaxFileSize))
            {
                long filesize = 0;
                string low_part = arg[step].Substring(FlagToSetMaxFileSize.Length);


                if (long.TryParse(low_part, out filesize))
                {
                    SearchTarget.FileSizeMax = filesize;
                    SearchTarget.CheckFileSize = true;
                    return true;
                }
                else
                {
                    if (DealWithFileSizeParsing(low_part, out filesize))
                    {
                        SearchTarget.FileSizeMax = filesize;
                        SearchTarget.CheckFileSize = true;
                        return true;
                    }
                }

                return false;
            }
            if (low.StartsWith(FlagSetAnchor))
            {
                string low_part = arg[step].Substring(FlagSetAnchor.Length);
                low_part = Trim(low_part);
                if (Directory.Exists(low_part))
                {
                    SearchAnchor.AddAnchor(low_part);
                    WasStartPointSet = true;
                    return true;
                }
                else
                {
                    if (!low_part.Contains(';'))
                    {
                        return false;
                    }
                    else
                    {
                        if (HandlePossibleMultipleFilePath(low_part, SearchAnchor))
                        {
                            WasStartPointSet = true;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            
            /*
             * this check is due to both commands starting with the same letter
             * Should you move this to where anchor is checked afterwards. Ensure the && test is kept
             * */
            if (low.StartsWith(FlagToSetFileAttributeViaDir) && (low.Contains(FlagSetAnchor) == false))
            {
                string low_part = arg[step].Substring(FlagToSetFileAttributeViaDir.Length);
                if (DealWithFileAttribEnum(low_part, out SearchTarget.AttributeMatching1, true))
                {
                    was_fileattribs_set = true;
                    return true;
                }
            }


            if (low.StartsWith(FlagSetAction) )
            {
                string low_part = arg[step].Substring(FlagSetAction.Length);
                if (DealWithAction(low_part))
                {
                    WasActionSet = true;
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




        private  OdinSearch_OutputConsumerBase ResolveOutFormatAndPlugins()
        {
            OdinSearch_OutputConsumerBase ret = null;
            if (!WasNetPluginSet)
            {
                if (WasActionSet)
                {
                    switch (UserAction)
                    {
                        case ActionCommand.CmdShell:
                            ret = new OdinSearch_OutputConsumer_CmdProcessor();
                            break;
                        case ActionCommand.PowerShell:
                            ret = new OdinSearch_OutputConsumer_PowerShell();
                            break;
                        default:
                            return null;
                            break;
                    }
                }
                
                switch (TargetStreamHandling)
                {
                    case ConsoleLines.NoRedirect:
                        switch (UserFormat)
                        {
                            case TargetFormat.Unicode:
                                ret = new OdinSearch_OutputConsole();
                                ret[OdinSearch_OutputConsole.MatchStream] = this.TargetStream;
                                ret[OdinSearch_OutputConsole.FlushAlways] = true;
                                ret[OdinSearch_OutputConsole.OutputOnlyFileName] = FlagJustFileName;
                                break;
                            case TargetFormat.CSVFile:
                                ret = new OdinSearchOutputCVSWriter();
                                ret[OdinSearchOutputCVSWriter.MatchStream] = this.TargetStream;
                                ret[OdinSearchOutputCVSWriter.FlushAlways] = true;
                                break;
                        }
                        
                        

                        break;
                    case ConsoleLines.Stdout:
                        ret = new OdinSearch_OutputConsole();
                        ret[OdinSearch_OutputConsole.MatchStream] = Console.Out;
                        ret[OdinSearch_OutputConsole.FlushAlways] = true;
                        ret[OdinSearch_OutputConsole.OutputOnlyFileName] = FlagJustFileName;

                        break;
                    case ConsoleLines.Stderr:
                        ret = new OdinSearch_OutputConsole();
                        ret[OdinSearch_OutputConsole.MatchStream] = Console.Error;
                        ret[OdinSearch_OutputConsole.FlushAlways] = true;
                        ret[OdinSearch_OutputConsole.OutputOnlyFileName] = FlagJustFileName;


                        break;
                    default:
                        return null;
                 }
            }
            else
            {
                if (PluginHasClassNameSet == false)
                {
                    ret = new OdinSearch_OutputConsumer_UnmanagedPlugin(ExternalPluginDll);
                }
                else
                {
                    ret = new OdinSearch_OutputConsumer_ExternManaged(ExternalPluginDll, null, ExternalPluginName);
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

            

            if ( (WasFileNameSet == true) || (WasAnyFileSet == false))
            {

                if (!WasFileNameSet)
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

                if (!WasCreationDateSet)
                {
                    SearchTarget.CreationAnchorCheck1 = SearchTarget.MatchStyleDateTime.Disable;
                }

                if (!WasLastAccessDateSet)
                {
                    SearchTarget.AccessAnchorCheck1 = SearchTarget.MatchStyleDateTime.Disable;
                }

                if (!WasLastChangedDateSet)
                {
                    SearchTarget.WriteAnchorCheck1 = SearchTarget.MatchStyleDateTime.Disable;
                }

                if (!WasFileCompareSet)
                {
                    if (SearchTarget.FileName.Contains(SearchTarget.MatchAnyFile) == false)
                    {
                        if (SearchTarget.FileName.Count > 1)
                        {
                            SearchTarget.AttribMatching1Style = SearchTarget.MatchStyleFileAttributes.MatchAny;
                        }
                        else
                        {
                            SearchTarget.AttribMatching1Style = SearchTarget.MatchStyleFileAttributes.MatchAll;
                        }
                    }
                }
            }
            /*if  ( (was_anyfile_flag_set == false) && )
            {
            }*/
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
          
            if (!WasOutStreamSet)
            {
                this.TargetStreamHandling = ConsoleLines.Stdout;
                TargetStream = null;
            }

            if (!WasOutFormatSet)
            {
                this.UserFormat = TargetFormat.Unicode;
            }
            else
            {

            }

            if ((WasWholeMachineSet) || (!WasStartPointSet))
            {
                //if (was_wholemachine_flag_set)
                {
                    SearchAnchor = new SearchAnchor();
                    SearchAnchor.EnumSubFolders = true;
                }
                
            }
            if (WantSubFoldersAlso)
            {
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


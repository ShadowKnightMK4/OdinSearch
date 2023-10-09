using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OdinSearchEngine;
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
     /// This class is responsible for parsing
     /// </summary>
    static class ArgHandling
    {
        public static SearchTarget SearchTarget = new SearchTarget();
        public static SearchAnchor SearchAnchor = new SearchAnchor();
        /// <summary>
        /// This flag lets user set what to feed into <see cref="SearchTarget"/>
        /// </summary>
        const string filenamearg = "/filename=";

        const string filenameanyfile = "/anyfile";


        /// <summary>
        /// THis is shorthand to just searrch the whole machine
        /// </summary>
        const string wholemachine = "/searchall";

        const string enumsubfolders = "/enumsubs";

        const string dropenumfolders = "/-enumsubs";


        const string matchfilesettings = "/matchfilenamestyle=";

        const string directoryspec = "/exactfilecheck=";
        public static void DisplayUsageText()
        {
            var Self = Assembly.GetCallingAssembly().GetManifestResourceNames();
            foreach (string s in Self)
            {
                if (s.EndsWith("Resources.UsageText.txt"))
                {
                    using (var SelfStream = Assembly.GetCallingAssembly().GetManifestResourceStream(s))
                    {
                        if (SelfStream != null)
                        {
                            byte[] B = new byte[SelfStream.Length];
                            SelfStream.ReadExactly(B,0, B.Length);
                            Console.WriteLine(string.Format(Encoding.UTF8.GetString(B), Assembly.GetCallingAssembly().GetName().Name));
                        }
                    }
                }
            }
            Console.WriteLine(Self);
        }
        enum ProcesFlagWildcardMode
        {
            UseFileOut = 1,
            UseDirectOut = 2
        }
        private static bool ProcessAFlag(string[] arg, int step)
        {
             
            // used to split '*.dll;*.exe' into multiple search targets for SeachTarget
            void SplitWildcards(string target, ProcesFlagWildcardMode usefile)
            {
                target  = target.Trim();
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
            }
            string low = arg[step].ToLowerInvariant();

            if (low.StartsWith(directoryspec))
            {
                string filespec = low.Substring(directoryspec.Length);
                if (filespec[0] == '\"')
                {
                    SplitWildcards(filespec, ProcesFlagWildcardMode.UseDirectOut);
                }   
                else
                {
                    SplitWildcards(filespec, ProcesFlagWildcardMode.UseDirectOut);
                }
                return true;
            }

            // this lets the user spec which wildcards will match.
            if (low.StartsWith(filenamearg))
            {
                string filespec = low.Substring(filenamearg.Length);
                // /filename=*.dll
                if (filenamearg[0] == '\"')
                {
                    // filename=\\*.dll;*.exe
                    SplitWildcards(filespec, ProcesFlagWildcardMode.UseFileOut);
                }
                else
                {
                    SplitWildcards(filespec, ProcesFlagWildcardMode.UseFileOut);
                }
                return true;
            }

            if (low.StartsWith(wholemachine))
            {
                SearchAnchor = new SearchAnchor();
                SearchAnchor.EnumSubFolders = true;
                return true;
            }

            if (low.StartsWith(enumsubfolders))
            {
                SearchAnchor.EnumSubFolders = true;
                return true;
            }

            if (low.StartsWith(dropenumfolders))
            {
                SearchAnchor.EnumSubFolders = false;
                return true;
            }

            if (low.StartsWith(filenameanyfile))
            {
                SearchTarget.FileName.Clear();
                SearchTarget.FileName.Add(SearchTarget.MatchAnyFile);
                return true;
            }

            

            if (low.StartsWith(matchfilesettings))
            {
                string e = low.Substring(matchfilesettings.Length);
                SearchTarget.MatchStyleString TestMatch;

                try
                {
                    TestMatch = (SearchTarget.MatchStyleString)Enum.Parse(typeof(SearchTarget.MatchStyleString), e);
                    return true;
                }
                catch (Exception)
                {
                    // attempt as a number
                    try
                    {
                        TestMatch = (SearchTarget.MatchStyleString)Enum.Parse(typeof(int), e);
                    }
                    catch (Exception)
                    {
                        return false;
                    }

                }
            }


            return false;
        }

        public static void ApplyDefaults()
        {
            if (SearchAnchor == null)
            {
                SearchAnchor = new SearchAnchor();
                SearchAnchor.EnumSubFolders = true;
            }
        }
        public static bool DoTheThing(string[] Args)
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
            return Valid;
        }

        internal static void Usage()
        {
            DisplayUsageText();
        }
    }

}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools
{

    /// <summary>
    /// Small abstract to make it easier to create symbolic/hardlinks between Linux/Windows. Currently Tested only on Windows.
    /// </summary>
    public static class OdinSearch_FileLinkHelper
    {
        [DllImport("Kernel32.dll", EntryPoint = "CreateHardLinkW", CallingConvention = CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool CreateHardLink(string PlacePointerHere, string PointerPointsToThis, IntPtr SecurityAttrib);

#if _LINUX
        const string LN_command = "-l \"{0}\" \"{1}\"";
#else
        const string LN_command = "";
#endif 


        public static void CreateSymbolicLink(string PointerPlace, string Target)
        {
             File.CreateSymbolicLink(PointerPlace, Target);
        }
        /// <summary>
        /// For Windows we invoke the createhardlink api. For Linux, ln for everything else, Nothing.
        /// </summary>
        /// <param name="PointerPlace">Place pointer here</param>
        /// <param name="Target">Pointer should point to this file item</param>
        /// <returns></returns>
        public static bool CreateHardLink(string PointerPlace, string Target)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return CreateHardLink(PointerPlace, Target, nint.Zero);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("ln", string.Format(LN_command, PointerPlace, Target));
            }
            return false;
        }
        static OdinSearch_FileLinkHelper()
        {
        }
    }
    /// <summary>
    /// This creates symbolic links from the search to the output folder. User of this class can subclass <see cref="OdinSearch_SymbolicLinkSort_FileSystemHelp"/> to customize how <see cref="OdinSearch_SymbolicLinkSort"/> creates links and folders.
    /// </summary>
    public class OdinSearch_SymbolicLinkSort: OdinSearch_OutputConsumerBase
    {
        public abstract class OdinSearch_SymbolicLinkSort_FileSystemHelp
        {

            /// <summary>
            /// place a call to this in your subclassed code before attempting to make link. It will test for existance of the link location and if it does exist append a number and keep trying until either it has the cap for uint128 or fails.
            /// </summary>
            /// <param name="linklocation">Pass where you plan to create the link</param>
            /// <returns>returns the possibly altered location if there's an existing symbol file with that name there already.</returns>
            protected string LoopUntilUnique(string linklocation)
            {
                if (File.Exists(linklocation))
                {
                again:
                    uint counter = 1;
                    string Base = Path.GetDirectoryName(linklocation);
                    string ext = Path.GetExtension(linklocation);
                    string name = Path.GetFileNameWithoutExtension(linklocation);

                    string testspot = Path.Combine(Base, name);
                    testspot += "(" + counter.ToString() + ")" + ext;

                    if (File.Exists(testspot))
                    {
                        counter++;
                        goto again;
                    }
                    return testspot;
                }
                return linklocation;
            }

            /// <summary>
            /// Create a folder in root location based on Info and return its location
            /// </summary>
            /// <param name="Info">Matching file  system item recevied from <see cref="OdinSearch"/></param>
            /// <param name="RootLocation">This is from the string set by <see cref="OdinSearch_SymbolicLinkSort.OutputFolderArgument"/></param>
            /// <returns>Should return the full path to the folder created. If you need to </returns>
            public abstract string MakeFolder(FileSystemInfo Info, string RootLocation);
            /// <summary>
            /// Create A link between Info's file system item and the Link Location
            /// </summary>
            /// <param name="Info">Matching file  system item recevied from <see cref="OdinSearch"/></param>
            /// <param name="RootLocation">This is from the string set by <see cref="OdinSearch_SymbolicLinkSort.OutputFolderArgument"/></param>
            /// <param name="LinkLocation">This is where to place the link.</param>
            /// <returns>return true if it worked and false if not.</returns>
            public abstract bool MakeLink(FileSystemInfo Info, string RootLocation, string LinkLocation);
        }

        /// <summary>
        /// Should the user not specify one, this is instanced instead. Tries to first create hardlinks and if that reports failure, soft links
        /// </summary>
        internal class DefaultSymbolicLinkSearch: OdinSearch_SymbolicLinkSort_FileSystemHelp
        {
            /// <summary>
            /// Default class action. try to create the folder specified by RootLocation and return it.
            /// </summary>
            /// <param name="Info"></param>
            /// <param name="RootLocation"></param>
            /// <returns></returns>
            public override string MakeFolder(FileSystemInfo Info, string RootLocation)
            {
                string ext = Path.GetExtension(Info.FullName);
                if (ext == string.Empty)
                {
                    ext = "Noone";
                }
                else
                {
                    ext = ext.Substring(1);
                }
                RootLocation = Path.Combine(RootLocation, ext);
                if (!Directory.Exists(RootLocation))
                {
                    Directory.CreateDirectory(RootLocation);
                }
                return RootLocation;
            }

            /// <summary>
            /// Default class action. Try to make a hardlink first and if that fails, soft link. Should duplicate names be used already we try appeanding (1,2)
            /// </summary>
            /// <param name="Info">Received from <see cref="OdinSearch_OutputConsumerBase.Match(FileSystemInfo)"/></param>
            /// <param name="LinkLocation">Try to create the link here. Check for duplicaties</param>
            /// <param name="RootLocation">Receveid from <see cref="OutputFolderArgument"/> in <see cref="OdinSearch_SymbolicLinkSort"/> custom args</param>
            /// <returns>return true if it worked and false if not.  Not that it makes a difference really any way.</returns>
            public override bool MakeLink(FileSystemInfo Info, string RootLocation, string LinkLocation)
            {
                if (File.Exists(LinkLocation))
                {
                    LinkLocation = LoopUntilUnique(LinkLocation);
                }
                //File.CreateSymbolicLink(fileout, info.FullName);
                if (!OdinSearch_FileLinkHelper.CreateHardLink(LinkLocation, Info.FullName))
                {
                    //                if (lasterr == 17)
                    {
                        OdinSearch_FileLinkHelper.CreateSymbolicLink(LinkLocation, Info.FullName);
                        return true;
                    }
                }
                return true;
            }
        }
        
        /// <summary>
        /// REQUIRED <see cref="OdinSearch_OutputConsumerBase.CustomParameters"/> string value.  We store hardlinks to folders of this base. Folder is created if not existing.
        /// </summary>
        public const string OutputFolderArgument = "OutputFolder";

        /// <summary>
        /// This custom Argument is unused for this class.
        /// </summary>
        public const string CreateSubfoldersOption = "CreateSubFolders";

        /// <summary>
        /// OPTIONAL <see cref="OdinSearch_OutputConsumerBase.CustomParameters"/> <see cref="OdinSearch_SymbolicLinkSort_FileSystemHelp"/> class. If not set, a default one is used where it creates symbolic links by extention in the target.
        /// </summary>
        public const string LinkSearchHelperClass = "LinkSearchHelperClass";

        protected OdinSearch_SymbolicLinkSort_FileSystemHelp Touch = null;

        public override void Match(FileSystemInfo info)
        {

            string root = this[OutputFolderArgument].ToString();
            string LinkLocation =  Touch.MakeFolder(info,root );

            Touch.MakeLink(info, root, Path.Combine(LinkLocation, info.Name) );

            base.Match(info);
        }
        public override bool SearchBegin(DateTime Start)
        {
            if ( (ArgCheck(CreateSubfoldersOption) == false) || (ArgCheck(OutputFolderArgument)==false) )  
            {
                throw new InvalidOperationException();
            }

            try
            {
                Touch = this[LinkSearchHelperClass] as OdinSearch_SymbolicLinkSort_FileSystemHelp;
            }
            catch (ArgumentNotFoundException)
            {
                // its fine. Just make the default one
                Touch = new DefaultSymbolicLinkSearch();
            }
            if (!Directory.Exists(this[OutputFolderArgument].ToString()))
            {
                Directory.CreateDirectory(this[OutputFolderArgument].ToString());
            }
            

            return false;
        }

    }
}

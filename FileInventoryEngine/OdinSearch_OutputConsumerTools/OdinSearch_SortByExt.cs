using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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
        /// For Windows we invoke the createhardlink api. For Linux, ln for everything else, NOthing
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
    /// This creates symbolic links from the search to the output folder.
    /// </summary>
    public class OdinSearch_SortByExt: OdinSearch_OutputConsumerBase
    {
        
        public enum CreateSubFolderOptionsEnum
        {
            /// <summary>
            /// We extra extention and 
            /// </summary>
            ByExt = 0
        }
        /// <summary>
        /// We store hardlinks to folders of this base.
        /// </summary>
        public const string OutputFolderArgument = "OutputFolder";

        /// <summary>
        /// This is 
        /// </summary>
        public const string CreateSubfoldersOption = "CreateSubFolders";

        public override void Match(FileSystemInfo info)
        {
            string fileout;
            string targetout;
            string ext = Path.GetExtension(info.FullName);
            if (ext == string.Empty)
            {
                targetout = "Noone";
            }
            else
            {
                targetout = ext.Substring(1);
            }

            fileout = Path.Combine(this[OutputFolderArgument].ToString(), targetout);
            if (!Directory.Exists(fileout))
            {
                Directory.CreateDirectory(fileout);
            }
            fileout = Path.Combine(fileout, info.Name);
            
            //File.CreateSymbolicLink(fileout, info.FullName);
            if (!OdinSearch_FileLinkHelper.CreateHardLink(fileout, info.FullName))
            {
//                if (lasterr == 17)
                {
                    OdinSearch_FileLinkHelper.CreateSymbolicLink(fileout, info.FullName);
                }
            }
            base.Match(info);
        }
        public override bool SearchBegin(DateTime Start)
        {
            if ( (ArgCheck(CreateSubfoldersOption) == false) || (ArgCheck(OutputFolderArgument)==false) )  
            {
                throw new InvalidOperationException();
            }
            if (!Directory.Exists(this[OutputFolderArgument].ToString()))
            {
                Directory.CreateDirectory(this[OutputFolderArgument].ToString());
            }
            return false;
        }

    }
}

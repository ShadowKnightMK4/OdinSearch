using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools.StreamWriterCommonBased
{
    /// <summary>
    /// A <see cref="OdinSearch_OutputConsumerStreamWriter"/> based version of <see cref="OdinSearch_OutputSimpleConsole"/>. 
    /// Has these extra features beyond the original:
    ///     can set custom arg <see cref="CharEncoding"/> to specify an encoding to use.
    /// </summary>
    /// 
    public class OdinSearch_OutputConsole : OdinSearch_OutputConsumerStreamWriter
    {
        /// <summary>
        /// Optional and default is false:   If set to bool, we output *only* filename for matches rather than File Match *a @ this 
        /// </summary>
        /// <example>
        /// A Match is found at C:\\Windows\\notepad.exe.
        /// 
        /// JUSTTHENAME Set would then cause this to output "C:\\Windows\\notepad.exe".
        /// JUSTTHENAME unspecified or clear would calse this to output ' filematch "notepad.exe" @ "C:\\Windows\\notepad.exe"'
        /// </example>
        public const string OutputOnlyFileName = "JUSTTHENAME";
        /// <summary>
        /// if this is set, the Match routine should only send just the exact file path + name to the outstream
        /// </summary>
        protected bool OutputOnlyName = false;

        public override void Match(FileSystemInfo info)
        
        {
            string fin;
            if (!OutputOnlyName)
            {
                fin = string.Format("File Match: \"{0}\" @ \"{1}\"\r\n", info.Name, info.FullName);
            }
            else
            {
                fin = string.Format("{0}\r\n", info.FullName);
            }
            WriteToOutStream(fin);
            base.Match(info);
        }
        public override bool SearchBegin(DateTime Start)
        {
            if (GetCustomParameterNames().Contains(OutputOnlyFileName))
            {
                bool result;
                try
                {
                    result = (bool)this[OutputOnlyFileName];
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("Invalid argument for output control flag. Expected true or false value", e);
                }
                OutputOnlyName = result;
            }
            return base.SearchBegin(Start);
        }

    }
}

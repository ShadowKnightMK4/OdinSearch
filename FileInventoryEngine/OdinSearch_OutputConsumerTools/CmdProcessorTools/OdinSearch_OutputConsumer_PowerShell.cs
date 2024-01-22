using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools.CmdProcessorTools
{
    [SupportedOSPlatform("windows")]
    public class OdinSearch_OutputConsumer_PowerShell: OdinSearch_OutputConsumer_CmdProcessor
    {
 
        protected override void AssignCommandProcessor()
        {
             bool HuntDownPowerShell()
            {
                string SystemRoot = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string Test1 = Path.Combine(SystemRoot, "\\WindowsPowerShell\\v1.0");

                if (Path.Exists(Test1))
                {
                    TargetCommandProcessor.FileName = Path.Combine(Test1, "powershell.exe");
                    if (File.Exists(TargetCommandProcessor.FileName))
                    {
                        return true;
                    }
                }
                return false;
            }
            if (!HuntDownPowerShell())
            {

            }
        }
        public OdinSearch_OutputConsumer_PowerShell()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == false)
            {
                throw new NotSupportedException("OdinSerach_OutputConsumer_PowerShell class is for windows only.");
            }
            
        }
    }
}

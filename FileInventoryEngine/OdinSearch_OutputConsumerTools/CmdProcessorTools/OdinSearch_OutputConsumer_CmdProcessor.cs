using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools.CmdProcessorTools
{
    /// <summary>
    /// Execute commands to the command shell (cmd.exe / bash). 
    /// </summary>
    /// <exception cref="FileNotFoundException">Can be thrown if unable to locate the cmd.exe/bash </exception>
    public class OdinSearch_OutputConsumer_CmdProcessor: OdinSearch_OutputConsumer_ExternCmdProcessorBase
    {
        protected override void AssignCommandProcessor()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // windows. we pick the command line processor in C:\Windows\system32\cmd.exe
                this.TargetCommandProcessor.FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "cmd.exe");
                if (!File.Exists(TargetCommandProcessor.FileName))
                {
                    throw new FileNotFoundException(TargetCommandProcessor.FileName + " does not appear to exist!?");
                }

                return;
            }
            else
            {

                if (File.Exists("/usr/bin/bash"))
                {
                    this.TargetCommandProcessor.FileName = "/usr/bin/bash";
                }
                {
                    if (File.Exists("/bin/bash"))
                    {
                        this.TargetCommandProcessor.FileName = "/bin/bash";
                    }
                    else
                    {
                        this.TargetCommandProcessor.FileName = Environment.GetEnvironmentVariable("BASH_SOURCE");
                        if (!File.Exists(TargetCommandProcessor.FileName))
                        {
                            // thank you bing ai
                            {
                                // linux. we use the Process class to run the which command and get the output
                                Process p = new Process();
                                p.StartInfo.FileName = "which";
                                p.StartInfo.Arguments = "bash";
                                p.StartInfo.UseShellExecute = false;
                                p.StartInfo.RedirectStandardOutput = true;
                                p.Start();
                                string output = p.StandardOutput.ReadToEnd();
                                p.WaitForExit();
                                this.TargetCommandProcessor.FileName = output.Trim();

                                if (File.Exists(TargetCommandProcessor.FileName) == false)
                                {
                                    throw new FileNotFoundException("Can't locate bash for some reason. Perhaps assign BASH_SOURCE Envirorment variable to where it's at?");
                                }


                            }

                        }
                    }
                }
            }
        }
        public OdinSearch_OutputConsumer_CmdProcessor()
        {
            
        }

        
        public override bool SearchBegin(DateTime Start)
        {
            if (ExecutionMode == Mode.RunAndFeedCommandsToStdout)
            {
                if (RuntimeInformation.IsOSPlatform( OSPlatform.Windows))
                {
                    // we assume cmd.exe 
                    CommandPrefix = "/K ";

                }
            }
            else
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // we assume cmd.exe 
                    CommandPrefix = "/C ";
                }
            }
            return base.SearchBegin(Start);
        }
    }
}

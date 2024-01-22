using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools.CmdProcessorTools
{
    /// <summary>
    /// This is the base comsumer to run commands with cmd.exe / ps / Linux equal on matches.
    /// </summary>
    public abstract class OdinSearch_OutputConsumer_ExternCmdProcessorBase : OdinSearch_OutputConsumerBase
    {
        /// <summary>
        /// This is called in the constructor. This should locate which command processor to use i.e. cmd.exe / powershell / bash depending on the platform and set <see cref="TargetCommandProcessor.FileName"/> to what to use.
        /// </summary>
        protected abstract void AssignCommandProcessor();
        public OdinSearch_OutputConsumer_ExternCmdProcessorBase()
        {
            TargetCommandProcessor = new ProcessStartInfo();
            AssignCommandProcessor();
        }
        /// <summary>
        /// This is set to true automatiiccaly if <see cref="CommandToExecute"/> property is assigned something
        /// </summary>
        public bool WasCommandSet { get; private set; }

        /// <summary>
        /// This is the command to execute. Take note that {0} will resolve to the full path of the matched file and will be quoted automatically
        /// </summary>
        public string CommandToExecute
        {
            get
            {
                return __CommandToRun;
            }
            set
            {
                __CommandToRun = value;
                WasCommandSet = true;
            }
        }

        /// <summary>
        /// private backing variable of <see cref="CommandToExecute"/>
        /// </summary>
        string __CommandToRun;

        /// <summary>
        /// Which mode are we dealing with the processor in
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// we spawn a new process each time and execute the command;
            /// </summary>
            RunOnce = 0,
            /// <summary>
            /// we spawn a single process at the start and execute the command 
            /// </summary>
            RunAndFeedCommandsToStdout =1
        }
        /// <summary>
        /// This is what we start to use to execute the process.
        /// </summary>
        protected ProcessStartInfo TargetCommandProcessor { get; set; }

        /// <summary>
        /// this is pasted after the cmd processor. For example /K and /C on cmd.exe 
        /// </summary>
        protected string CommandPrefix; 
        /// <summary>
        /// Holds the process class we use for the life if <see cref="ExecutionMode"/> is <see cref="Mode.RunAndFeedCommandsToStdout"/>.
        /// </summary>
        protected Process ProcessContainer;

        /// <summary>
        /// What mode we start the shell with. Currently only supports <see cref="Mode.RunOnce"/>
        /// </summary>
        public Mode ExecutionMode
        {
            get
            {
                return __ExecMode;
            }
        }

        Mode __ExecMode = Mode.RunOnce;

        /// <summary>
        /// format the command based on the passed data and the command processor
        /// </summary>
        /// <param name="command"></param>
        /// <param name="Info"></param>
        /// <returns></returns>
        private string MakeCommand(string command, FileSystemInfo Info)
        {
            
            {
                command = CommandPrefix + command;
            }
            return string.Format(command, "\"" + Info.FullName + "\""); 
        }

        /// <summary>
        /// Assign the arguments to to this cmd we will run
        /// </summary>
        /// <param name="command"></param>
        /// <param name="i"></param>
        /// <param name="Info"></param>
        protected virtual void SetCommand(string command, ProcessStartInfo i, FileSystemInfo Info)
        {
            i.Arguments = MakeCommand(command, Info);
        }
        public override void AllDone()
        {
            base.AllDone();
        }
        public override void Blocked(string Blocked)
        {
            base.Blocked(Blocked);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        public override bool HasPendingActions()
        {
            return base.HasPendingActions();
        
        }

        public override void Match(FileSystemInfo info)
        {
            if (ExecutionMode == Mode.RunOnce)
            {
                // we spawn the process and do not care about it.
                SetCommand(__CommandToRun, TargetCommandProcessor, info);
                Process newProcess = new Process();
                newProcess.StartInfo = new ProcessStartInfo(TargetCommandProcessor.FileName, TargetCommandProcessor.Arguments); ;
                


                newProcess.Start();
                    
            }
            base.Match(info);
        }

        public override void Messaging(string Message)
        {
            base.Messaging(Message);
        }
        public override bool ResolvePendingActions()
        {
            return base.ResolvePendingActions();
        }

        public override bool SearchBegin(DateTime Start)
        {
            if (!WasCommandSet)
            {
                throw new InvalidOperationException("This kind of communcation class needs a command to execute on matches. Set " + nameof(CommandToExecute) + "something to run on matches.");
            }
            ProcessContainer = new Process();
            ProcessContainer.StartInfo = TargetCommandProcessor;
            if (ExecutionMode == Mode.RunOnce)
            {
                TargetCommandProcessor.RedirectStandardInput = false;
            }
            else
            {
                TargetCommandProcessor.RedirectStandardInput = true;
            }

            return base.SearchBegin(Start);
        }

        public override void WasNotMatched(FileSystemInfo info)
        {
            base.WasNotMatched(info);
        }

        
    }

}

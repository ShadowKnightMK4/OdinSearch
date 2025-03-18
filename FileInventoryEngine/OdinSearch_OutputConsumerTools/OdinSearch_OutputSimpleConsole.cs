using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools
{

    /// <summary>
    /// Output matches to a TextWriter or a Stream or a File. Default action is send positive match text to stdout and error text to stderr.
    /// One can tell it to just send the matched location itself by setting custom arg OutputOnlyFileName to true.
    /// Telling this to output to something other than the console likely is gonna need FlushAlways also set.
    /// </summary>
    /// <remarks>This class is intended to be an Example. There's not plans to modify this currently unless there's issues / bug fixes. Use <see cref="StreamWriterCommonBased.OdinSearch_OutputConsole"/> for additonal needs</remarks>
    public class OdinSearch_OutputSimpleConsole : OdinSearch_OutputConsumerBase
    {
        /// <summary>
        /// Set the argument to a string to place output to that file in unicode. If a stream or TestWriter, writes directly to that.
        /// </summary>
        public const string MatchStream = "STDOUT";
        /// <summary>
        /// Set the argument to a string to place output to that file in unicode. If a stream or TestWriter, writes directly to that.
        /// </summary>
        public const string BlockStream = "STDERR";

        /// <summary>
        /// Default action is stdout/stderr is assumed to always flush but streams are flushed on SearchDone().  Set this to flush after each call regardless
        /// </summary>
        public const string FlushAlways = "FLUSHALWAYS";
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

#pragma warning disable IDE0052 // Remove unread private members
        // Suppression due to the noise, these hold the streams that stdout and stderr deal with
        Stream outstream, errstream;
        TextWriter stdout, stderr;
        bool OutputOnlyName = false;
        bool FlushAlwaysFlag = false;
        bool DisposeOutStream = false;
        bool DisploseErrStream = false;
#pragma warning restore IDE0052
        public OdinSearch_OutputSimpleConsole()
        {
            this[MatchStream]  = Console.Out;
            this[BlockStream] = Console.Error;
            // yay defaults.
              stdout = Console.Out;
              stderr = Console.Error;
            FlushAlwaysFlag = false;
        }
        public override void AllDone()
        {
            if (FlushAlwaysFlag)
            {
                errstream?.Flush();
                outstream?.Flush();
            
            }
            base.AllDone();
        }

        
        

        /// <summary>
        /// Begin the search, parse our custom arguments and set defaults.
        /// </summary>
        /// <param name="Start">This is the DateTime of now when the search starts</param>
        /// <returns>returns same thing as <see cref="OdinSearch_OutputConsumerBase.SearchBegin(DateTime)"/><returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <remarks> see also <see cref="OdinSearch_OutputConsumerBase.SearchBegin(DateTime)"/></remarks>
        public override bool SearchBegin(DateTime Start)
        {
            string[] Custom = this.GetCustomParameterNames();
            if (Custom.Contains(MatchStream))
            {
                string test_string = this[MatchStream] as string;

                if (test_string != null)
                {
                    outstream = File.OpenWrite(test_string);
                    DisposeOutStream = true;
                }
                else
                {

                    stdout = this[MatchStream] as TextWriter;

                    if (stdout == null)
                    {
                        outstream = this[MatchStream] as Stream; 
                        if (outstream == null)
                        {
                            throw new InvalidOperationException("Invalid Argument for MatchStream. Expected a string for a file, a StreamWRiter or a textwriter for stream");
                        }
                    }
                }
            }

            if (Custom.Contains(BlockStream))
            {
                string test_string = this[BlockStream] as string;

                if (test_string != null)
                {
                    errstream = File.OpenWrite(test_string);
                    DisploseErrStream = true;
                }

                stderr = this[BlockStream] as TextWriter;

                if (stderr == null)
                {
                    errstream = this[BlockStream] as Stream;
                    if (errstream == null)
                    {
                        throw new InvalidOperationException("Invalid Argument for BlockStream. Expected a string for a file, a Stream or a textwriter for stream");
                    }
                }
            }

            if (Custom.Contains(OutputOnlyFileName))
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
            if (Custom.Contains(FlushAlways))
            {
                bool result;
                try
                {
                    result = (bool)this[FlushAlways];
                    this.FlushAlwaysFlag = result;
                }
                catch (Exception e)
                {
                   throw new InvalidOperationException("Invalid argument for Flush Always flag. Expected true or false value", e);
                }
            }
            return base.SearchBegin(Start);

        }

        public override void WasNotMatched(FileSystemInfo info)
        {
            base.WasNotMatched(info);
        }
        public override void Match(FileSystemInfo info)
        {
            if (!OutputOnlyName)
            {
                if (stdout != null)
                {
                    stdout.WriteLine("File Match: \"{0}\" @ \"{1}\"", info.Name, info.FullName);
                }
                else
                {
                    byte[] b = Encoding.UTF8.GetBytes(string.Format("File Match: \"{0}\" @ \"{1}\"" + "\r\n", info.Name, info.FullName));
                    outstream.Write(b, 0, b.Length);
                    if (FlushAlwaysFlag)
                    {
                        outstream.Flush();
                    }
                }
            }
            else
            {
                if (stdout != null)
                {
                    stdout.WriteLine("{0}\r\n", info.FullName);
                }
                else
                {
                    byte[] b = Encoding.UTF8.GetBytes(info.FullName + "\r\n");
                    outstream.Write(b, 0, b.Length);
                    if (FlushAlwaysFlag)
                    {
                        outstream.Flush();
                    }
                }
                
            }
            base.Match(info);
        }

        public override void Blocked(string Blocked)
        {
            stderr.WriteLine(Blocked);
            base.Blocked(Blocked);
        }

        public override void Messaging(string Message)
        {
            stdout.WriteLine(Message);
            base.Messaging(Message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if ( DisposeOutStream)
                {                    
                    outstream?.Dispose();
                }

                if (DisploseErrStream)
                {
                    errstream?.Dispose();
                }
            }

            base.Dispose(disposing);
        }
        ~OdinSearch_OutputSimpleConsole()
        {
            Dispose(true);
            
        }
    }
}

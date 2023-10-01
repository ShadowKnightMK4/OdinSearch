using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools
{
    /// <summary>
    /// Serves as an example.  Matches are sent to stdout,  blocks are send to stderr by default. 
    /// </summary>
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

        Stream outstream, errstream;
        TextWriter stdout, stderr;
        public OdinSearch_OutputSimpleConsole()
        {
            this[MatchStream]  = Console.Out;
            this[BlockStream] = Console.Error;
            // yay defaults.
              stdout = Console.Out;
              stderr = Console.Error;
        }
        public override bool SearchBegin(DateTime Start)
        {
            string[] Custom = this.GetCustomParameterNames();
            if (Custom.Contains(MatchStream))
            {
                string test_string = this[MatchStream] as string;

                if (test_string != null)
                {
                    outstream = File.OpenWrite(test_string);
                }
                else
                {

                    stdout = this[MatchStream] as TextWriter;

                    if (stdout == null)
                    {
                        throw new InvalidOperationException("Invalid Argument for MatchStream. Expected a string for a file or a textwriter for stream");
                    }
                }
            }

            if (Custom.Contains(BlockStream))
            {
                string test_string = this[BlockStream] as string;

                if (test_string != null)
                {
                    errstream = File.OpenWrite(test_string);
                }

                stderr = this[MatchStream] as TextWriter;

                if (stderr == null)
                {
                    throw new InvalidOperationException("Invalid Argument for Error Stream. Expected a string for a file or a textwriter for stream");
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
            stdout.WriteLine("File Match: \"{0}\" @ \"{1}\"", info.Name, info.FullName);
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
    }
}

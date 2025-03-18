using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools.StreamWriterCommonBased
{
    




    /// <summary>
    /// The stream writer class houses some common stuff begin <see cref="OdinSearch_OutputSimpleConsole"/> and <see cref="OdinSearch_OutputSimpleCSVWriter"/>.
    /// IT also can search as a base
    /// </summary>
    /// <remarks>This please a little loose with <see cref="IDisposable"/>.  The underling streams it caches to function are tracked yes. The class itself tracks how many times <see cref="OdinSearch_OutputConsumerBase.SearchBegin(DateTime)"/> is called ticking up a value. When disposal is called, it gets ticked down, and if less than zero THEN we dispoe of the streams. THIS MEANS subclasses should CALL the base method of this when overriding <see cref="SearchBegin(DateTime)"/>. Skipping that = streams cleaned early</remarks>
    public class OdinSearch_OutputConsumerStreamWriter : OdinSearch_OutputConsumerBase
    {
        /// <summary>
        /// Exception thrown when attempting to write to a null stream with <see cref="WriteToErrStream(string)"/> or <see cref="WriteToOutStream(string)"/> after validation and the stream is null. 
        /// </summary>
        class PrematureDisposeException : Exception
        {
            public PrematureDisposeException() :  base($"A subclass of {nameof(OdinSearch_OutputConsumerStreamWriter)} did not implement correctly. Dev message: \"It should skip the call to the base {nameof(SearchBegin)} class or reture TRUE")
             { }
            public PrematureDisposeException(string message) : base(message) { }
        }
        /// <summary>
        /// Set the argument to a string to place output to that file in unicode. If a stream or TestWriter, writes directly to that.
        /// </summary>
        public const string MatchStream = "MATCHSTREAM";
        /// <summary>
        /// Set the argument to a string to place output to that file in unicode. If a stream or TestWriter, writes directly to that.
        /// </summary>
        public const string BlockStream = "BLCKESTREAM";

        /// <summary>
        /// Default action is stdout/stderr (The text writer) is assumed to always flush but streams are flushed on SearchDone().  Set this to flush after each call regardless. 
        /// </summary>
        public const string FlushAlways = "FLUSHALWAYS";

        /// <summary>
        /// Default will be <see cref="Encoding.UTF8"/> for output if not set.
        /// </summary>
        public const string CharEncoding = "CharEncoding";

#pragma warning disable IDE0052 // Remove unread private members
        /// <summary>
        /// If we accept the streams in validation page -set to true.
        /// </summary>
        bool StreamsValidated = false;
        /// <summary>
        /// We count the number of times <see cref="SearchBegin(DateTime)"/> is called, tick by one.  When we dispose, we tick this by 1 and if 0, actually dispoe
        /// </summary>
        protected int SearchReferenceCounter { get;  private set; } = 0;
        // Suppression due to the noise, these hold the streams that stdout and stderr deal with
        /// <summary>
        /// If a valid stream, we output here on matches. Should both this and stdout be set, stdout wins
        /// </summary>
        protected Stream outstream;
        /// <summary>
        /// If a valid stream, we output here on problems.Should both this and stderr be set, stderr wins
        /// </summary>
        protected Stream errstream;

        /// <summary>
        /// Mainly for Console.out.  Settings both this and oustream results in outstream ignored
        /// </summary>
        protected TextWriter stdout;
        /// <summary>
        /// Mainly for Console.errr.  Settings both this and oustream results in errstrema ignored
        /// </summary>
        protected TextWriter stderr;

        /// <summary>
        /// If this is set the code should always call the correct Flush() routine after outputing each bit of data.
        /// </summary>
        protected bool FlushAlwaysFlag = false;

        /// <summary>
        /// If we opened the outstream rather than say use a provide stream, we need to set this to true to close it on dispose
        /// </summary>
        protected bool DisposeOutStream = false;
        /// <summary>
        /// If we opened the errstream rather than say use a provide stream, we need to set this to true to close it on dispose
        /// </summary>

        protected bool DisploseErrStream = false;


        protected Encoding TargetEncoding = null;
#pragma warning restore IDE0052
        public OdinSearch_OutputConsumerStreamWriter()
        {
            this[MatchStream] = Console.Out;
            this[BlockStream] = Console.Error;
            // yay defaults.
            stdout = Console.Out;
            stderr = Console.Error;
            FlushAlwaysFlag = false;
            TargetEncoding = Encoding.UTF8;
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
            string[] Custom = GetCustomParameterNames();
            if (Custom.Contains(MatchStream))
            {
                // if we can cast to string, we open that write writing and set the dispal flag
                string test_string = this[MatchStream] as string;

                if (test_string != null)
                {
                    outstream = File.OpenWrite(test_string);
                    DisposeOutStream = true;
                }
                else
                {

                    // first callback. Can we use it as a TextWriter?
                    stdout = this[MatchStream] as TextWriter;

                    if (stdout == null)
                    {
                        // last fallback. Use it as a stream
                        outstream = this[MatchStream] as Stream;
                        if (outstream == null)
                        {
                            // failure. We don't know what to do
                            throw new InvalidOperationException("Invalid Argument for MatchStream. Expected a string for a file, a StreamWRiter or a textwriter for stream");
                        }
                    }
                }
            }

            if (Custom.Contains(BlockStream))
            {
                // if we can cast to string, we open that write writing and set the dispal flag
                string test_string = this[BlockStream] as string;

                if (test_string != null)
                {
                    errstream = File.OpenWrite(test_string);
                    DisploseErrStream = true;
                }
                else
                {
                    // first callback. Can we use it as a TextWriter?
                    stderr = this[BlockStream] as TextWriter;

                    if (stderr == null)
                    {
                        // last fallback. Use it as a stream
                        errstream = this[BlockStream] as Stream;
                        if (errstream == null)
                        {
                            throw new InvalidOperationException("Invalid Argument for BlockStream. Expected a string for a file, a Stream or a textwriter for stream");
                        }
                    }
                }
            }


            // this we is we just care if it is a bool and set to true or false as needed
            if (Custom.Contains(FlushAlways))
            {
                bool result = false;
                try
                {
                    result = (bool)this[FlushAlways];
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("Invalid argument for Flush Always flag. Expected true or false value", e);
                }
                FlushAlwaysFlag = result;
            }

            // We assign this to the encoding we use. Must be able to be cast to encoding object
            // we default to UTF8 if not set,
            if (Custom.Contains(CharEncoding))
            {
                TargetEncoding = this[CharEncoding] as Encoding;
                if (TargetEncoding == null)
                {
                    throw new InvalidOperationException("Invalid argument for Encoding flag. Expected a Encoding C# object such as Encoding.UTF8. Leave unset for UTF8");
                }
            }
            else
            {
                TargetEncoding = Encoding.UTF8;
            }
            // tick up reference
            this.SearchReferenceCounter++;
            // we made it this far, we're good to assume streams are ok for the lifespace
            this.StreamsValidated = true;
            return true;

        }

        /// <summary>
        /// This one just calls the base one. 
        /// </summary>
        /// <param name="info"></param>
        public override void WasNotMatched(FileSystemInfo info)
        {
            base.WasNotMatched(info);
        }
        /// <summary>
        /// IMPORTANT. You're gonna want to override this with how to deal with matches.
        /// <see cref="Console2.Match(FileSystemInfo)"/> or <see cref="CVSWriter2.Match(FileSystemInfo)"/> for  for ideas
        /// </summary>
        /// <param name="info"></param>
        public override void Match(FileSystemInfo info)
        {
            base.Match(info);
        }

        /// <summary>
        /// IMPORTANT. You're gonna want to override this with what to do if search could not access something. Default is nothing.
        /// </summary>
        /// <param name="Blocked"></param>
        public override void Blocked(string Blocked)
        {
            base.Blocked(Blocked);
        }

        /// <summary>
        /// IMPORTANT. You're gonna want to ovvride this to deal with messaging.  Default is ignoring them
        /// </summary>
        /// <param name="Message"></param>
        public override void Messaging(string Message)
        {
            base.Messaging(Message);
        }

        /// <summary>
        /// This dispoal routine takes care of cleaning up the underlying outstream and errstream if needed
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    // reduce the reference count. If <= 0, dispoe and clear stream
                    SearchReferenceCounter--;
                    if (SearchReferenceCounter <= 0)
                    {
                        if (DisposeOutStream)
                        {
                            outstream?.Dispose();
                        }

                        if (DisploseErrStream)
                        {
                            errstream?.Dispose();
                        }
                        errstream = outstream = null;
                    }
                    base.Dispose(disposing);
                    GC.SuppressFinalize(this);
                }

            }
            
        }
        ~OdinSearch_OutputConsumerStreamWriter()
        {
            Dispose(true);
        }

        /// <summary>
        /// Write to the underlying stream. Tries TW and falls back to targetstream if if is null. data is encodinged under targetformat if writing to targetstream
        /// </summary>
        /// <param name="data">string to write. If TargetStream is used, will be encoding with <see cref="TargetEncoding"/></param>
        /// <param name="TW">If null, we write with TargetStream, otherwise we write to this and add a newline</param>
        /// <param name="TargetStream"></param>
        /// <remarks>Throws <see cref="PrematureDisposeException"/> if both TW and TargetStream are null</remarks>
        void WriteToUnderlyingStreamBase(string data, TextWriter TW, Stream TargetStream)
        {
            if ((TW is null) && (TargetStream is null))
            {
                throw new PrematureDisposeException();
            }   
            if (TW != null)
            {
                TW.WriteLine(data);
            }
            else
            {
                
                byte[] b = TargetEncoding.GetBytes(data);
                TargetStream.Write(b, 0, b.Length);
                if (FlushAlwaysFlag)
                {
                    TargetStream.Flush();
                }
            }
        }

        /// <summary>
        /// Use this to write to the Match Result stream in your class. uses <see cref"TargetEncoding"/> as the format
        /// </summary>
        /// <param name="data">string to write</param>
   
         /// <exception cref="PrematureDisposeException">This can be thrown if the tracked private streams become null.  That means it's likely <see cref="SearchBegin(DateTime)"/>'s base call was not skipped. To Fix: have child class return true;</exception>
        protected void WriteToOutStream(string data)
        {
           
                WriteToUnderlyingStreamBase(data, stdout, outstream);
            
                
        }

        /// <summary>
        /// Use this to write to the blocked result or error result in your class.uses <see cref"TargetEncoding"/> as the format
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="PrematureDisposeException">This can be thrown if the tracked private streams become null.  That means it's likely <see cref="SearchBegin(DateTime)"/>'s base call was not skipped. To Fix: have child class return true;</exception>
        protected void WriteToErrStream(string data)
        {
            WriteToUnderlyingStreamBase(data, stderr, errstream);
        }
    }
}

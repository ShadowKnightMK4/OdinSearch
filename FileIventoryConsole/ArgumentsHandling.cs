using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;
using OdinSearchEngine;
using System.Globalization;
using Microsoft.VisualBasic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace FileInventoryConsole
{
    public class ArgumentMissingException: ArgumentException
    {
        public ArgumentMissingException() { }

        public ArgumentMissingException(string message) : base(message) { } 

    }
    /// <summary>
    /// class deals with parsing and extracting arguments
    /// </summary>
    static class ArgumentsHandling
    {

        public static Dictionary<string, string> ArgumentVariables = new Dictionary<string, string>();
        public enum Modes
        {
            None = 0,
            Usage = None,
            /// <summary>
            /// Output in instance of <see cref="OdinSearchEngine.SearchTarget"/>
            /// </summary>
            EmitXmlInputFiles
        }

        public const string ArgOutputFolder = "OutputFolder";
        public const string ArgXmlOutAnchor = "ExampleAnchor";
        public const string ArgXmlOutTarget = "ExampleTarget";

        /// <summary>
        /// Creates xml files to edit and act as source.
        /// </summary>
        /// <remarks>Requires "TargetFolder" set</remarks>
        public static void EmitXmlInputFiles()
        {
            string Target1, Target2;
            SearchTarget ExampleTarget = new SearchTarget();
            SearchAnchor ExampleAnchor= new SearchAnchor(true);

            XmlSerializer TargetOut = new XmlSerializer(typeof(SearchTarget));
            XmlSerializer Targetanchor = new XmlSerializer(typeof(SearchAnchor));


            
            if (ArgumentVariables.ContainsKey(ArgOutputFolder) == false)
            {
                throw new ArgumentMissingException(string.Format("Required Input: \'{0}\' was not specified",ArgOutputFolder));
            }

            if (ArgumentVariables.ContainsKey(ArgXmlOutTarget) == false)
            {
                throw new ArgumentMissingException(string.Format("Required Input: \'{0}\' was not specified", ArgXmlOutTarget));
            }

            if (ArgumentVariables.ContainsKey(ArgXmlOutAnchor) == false)
            {
                throw new ArgumentMissingException(string.Format("Required Input: \'{0}\' was not specified", ArgXmlOutAnchor));
            }

            Target1 = Path.Combine(ArgumentVariables[ArgOutputFolder], ArgumentVariables[ArgXmlOutAnchor]);
            Target2 = Path.Combine(ArgumentVariables[ArgOutputFolder], ArgumentVariables[ArgXmlOutTarget]);

            using (var Stream1 = File.OpenWrite(Target1))
            {
                TargetOut.Serialize(Stream1, Targetanchor);
            }

            using (var Stream2 = File.OpenWrite(Target2))
            {
                TargetOut.Serialize(Stream2, TargetOut);
            }
        }
        /// <summary>
        /// Read the reasource at "ProjectName.Resource.UsageText.txt" and send to stdout.  Resource is assumed to be unicode text
        /// </summary>
        public static void Usage()
        {
            
            SearchAnchor test=  new SearchAnchor(,)
            using (var text = Assembly.GetCallingAssembly().GetManifestResourceStream(Assembly.GetCallingAssembly().GetName().Name  + ".Resources.UsageText.txt"))
            {
                byte[] data =new byte[text.Length];
                text.Read(data, 0, data.Length);

                Console.WriteLine(Encoding.UTF8.GetString(data));
            }

        }
    }
}

using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools
{
    /// <summary>
    ///  Like <see cref="OdinSearch_OutputSimpleConsole"/>, this serves as an example.
    ///  Set <see cref="TargetSaveLocation"/> custom arg to a file to output too and this will make a CSV style text file there with matches to open with Excel later.
    /// </summary>
    public class OdinSearch_OutputSimpleCSVWriter : OdinSearch_OutputConsumerBase
    {

        /// <summary>
        /// Want Attributes in the list "Directory, Hidden".
        /// </summary>
        public const string WantAttributes = "WantAttributes";
        /// <summary>
        /// Want the Creation UTC Date
        /// </summary>
        public const string WantCreationUTCDate = "WantCreationUTCDate";
        /// <summary>
        /// Want the creation UTC Time
        /// </summary>
        public const string WantCreationUTCTime = "WantCreationUTCTime";

        public const string WantDirectoryLocation = "WantDirLocation";
        public const string WantFullLocation = "WantFullLocation";
        public const string WantExt = "WantExt";
        public const string WantNameNoExt = "WantFullNameNoExt";

        public const string WantLastAccessUTCDate = "WantLastAccessUTCDate";
        public const string WantLastWriteUTCDate = "WantLastWriteUTCDate";

        public const string WantLastAccessUTCTime = "WantLastAccessUTCTime";
        public const string WantLastWriteUTCTime = "WantLastWriteUTCTime";

        public const string WantNameWithExt = "WantNameWithExt";
        public const string WantSize = "WantSize";
        public const string DefaultMIAVAl = "DefaultMIAValue";
        void CheckDefault()
        {
            bool AnyAssigned = false;
            var Names = GetCustomParameterNames();

            
            foreach ( var Name in Names ) 
            { 
                string low = Name.ToLower();
                switch ( low )
                {
                    case WantCreationUTCDate:
                    case WantCreationUTCTime:
                    case WantDirectoryLocation:
                    case WantFullLocation:
                    case WantExt:
                    case WantNameNoExt:
                    case WantLastAccessUTCDate:
                    case WantLastWriteUTCDate:
                    case WantLastAccessUTCTime:
                    case WantLastWriteUTCTime:
                    case WantNameWithExt:
                    case WantSize:
                        AnyAssigned = true;
                        break;
                }
            }

            if (!AnyAssigned)
            {
                DefaultCustomSettings();
            }
            
        }


        /// <summary>
        /// write names based on the FileSystemInfo Properties at runtime
        /// </summary>
        private void WriteItemNames()
        {
            StringBuilder line = new StringBuilder();
            var Names = GetCustomParameterNames();
            foreach ( var Name in Names )
            {
                bool val = false;
                string val_as = GetCustomParameter(Name) as string;

                if ((Name.ToLowerInvariant().StartsWith("want")))
                {
                    if (val_as == null)
                    {
                        val = (bool)GetCustomParameter(Name);
                    }
                    if (Name.ToLowerInvariant().StartsWith("want") == true)
                    {
                        switch (Name)
                        {
                            case WantCreationUTCDate:
                                if (val)
                                    line.Append("CreationUTC_Date");
                                break;
                            case WantCreationUTCTime:
                                if (val)
                                    line.Append("CreationUTC_Time");
                                break;
                            case WantDirectoryLocation:
                                if (val)
                                    line.Append("LocationDirectory");
                                break;
                            case WantFullLocation:
                                if (val)
                                    line.Append("FullLocation");
                                break;
                            case WantExt:
                                if (val)
                                    line.Append("Ext");
                                break;
                            case WantNameNoExt:
                                if (val)
                                    line.Append("NameNoExt");
                                break;
                            case WantLastAccessUTCDate:
                                if (val)
                                    line.Append("LastAccessUTCDate");
                                break;
                            case WantLastAccessUTCTime:
                                if (val)
                                    line.Append("LastAccessUTCTime");
                                break;

                            case WantLastWriteUTCDate:
                                if (val)
                                    line.Append("LastWriteUTCDate");
                                break;
                            case WantLastWriteUTCTime:
                                if (val)
                                    line.Append("LastWriteUTCTime");
                                break;

                            case WantNameWithExt:
                                if (val)
                                    line.Append("NameAndExt");
                                break;
                            case WantAttributes:
                                if (val)
                                {
                                    line.Append("Attributes");
                                }
                                break;
                            case WantSize:
                                if (val)
                                    line.Append("WantSize");
                                break;
                        }
                        line.Append(',');

                    }
                }   
                

            }

            line.Length -= 1;
            TargetFile.WriteLine(line.ToString());
        }
        /// <summary>
        /// if we have no custom settings assigned, this is the default
        /// </summary>
        private void DefaultCustomSettings()
        {
            this[WantAttributes] = true;
            this[WantCreationUTCDate] = true;
            this[WantCreationUTCTime] = true;
            this[WantDirectoryLocation] = true;
            this[WantFullLocation] = true;
            this[WantExt] = true;
            this[WantLastAccessUTCTime] = true;
            this[WantLastWriteUTCTime] = true;

            this[WantLastAccessUTCDate] = true;
            this[WantLastWriteUTCDate] = true;
            this[WantNameWithExt] = true;
            this[WantSize] = true;
            this[DefaultMIAVAl] = "Unknown";
        }

        /// <summary>
        /// Required custom arg write the CSV file to here.
        /// </summary>
        public const string TargetSaveLocation = "TargetSaveLocation";
        FileSystemInfo First = null;
        StreamWriter TargetFile = null;


        /*
         * Our general plan with this is fetch the properties of the info item,
         * 
         * we do not assume it is a FileInfo or DirectoryInfo and play defensively.
         * 
         * Should a propertty not exist - for example DirectoryInfo.Length - we sub out the value for the Default one specified in the custom args.
         * If non is set, we use TDB
         * 
         * we also make the assuming that any custom arg name that starst with 'want' is sometime we need to extract from the object to write to the output file
         */
        private void WriteItemValues(FileSystemInfo info)
        {
            
            void WriteDateComponent(bool val, PropertyInfo[] Properties, string PropertyName, out string stringval, string unknown,  bool ExtractDateIfTrueOtherWiseExtractTime)
            {
                stringval = null;
                if (val)
                {
                    if (Properties.Any(p => p.Name == PropertyName))
                    {
                        var pro = Properties.FirstOrDefault(p => p.Name == PropertyName);
                        DateTime timewalker = (DateTime)pro.GetValue(info);
                        if (ExtractDateIfTrueOtherWiseExtractTime)
                        {
                            
                            stringval = timewalker.Date.ToString();
                            stringval = stringval.Substring(0, stringval.IndexOf(' '));
                        }
                        else
                        {
                            stringval = timewalker.ToUniversalTime().ToString();
                            stringval = stringval.Substring(stringval.IndexOf(' ')+1);
                        }
                    }
                    else
                    {
                        stringval = unknown;
                    }
                    stringval = "" + ""+stringval+"";
                }
            }
            StringBuilder line = new StringBuilder();
            var Names = GetCustomParameterNames();
            var Properties = info.GetType().GetProperties();
            string unknown;

            try
            {
                unknown = GetCustomParameter(DefaultMIAVAl).ToString();
            }
            catch (ArgumentNotFoundException)
            {
                unknown = "TDB";
            }
            foreach (string name in Names)
            {
                
                if (name.ToLowerInvariant().StartsWith("want"))
                {
                    bool val = (bool)GetCustomParameter(name);
                    string stringval=null;
                 
                    switch (name)
                    {
                        case WantAttributes:
                            if (val)
                            {
                                if (Properties.Any(p => p.Name == "Attributes"))
                                {
                                    var pro = Properties.FirstOrDefault(p => p.Name == "Attributes");
                                    stringval = Enum.GetName(pro.PropertyType,pro.GetValue(info));

                                }
                                else
                                {
                                    stringval = unknown;
                                }
                            }
                            break;
                        case WantCreationUTCDate:
                            
                            if (val)
                            {
                                WriteDateComponent(val, Properties, "CreationTimeUtc", out stringval, unknown, true);
                                break;
                            }
                            break;
                        case WantCreationUTCTime:
                            if (val)
                            {
                                WriteDateComponent(val, Properties, "CreationTimeUtc", out stringval, unknown,false);
                                break;
                            }
                            break;
                        case WantDirectoryLocation:
                            if (val)
                            {
                                if (Properties.Any(p => p.Name == "Directory"))
                                {
                                    var pro = Properties.FirstOrDefault(p => p.Name == "Directory");
                                    stringval = (pro.GetValue(info)).ToString();
                                }
                                else
                                {
                                    stringval = unknown;
                                }
                            }
                            break;
                        case WantFullLocation:
                            if (val)
                            {
                                if (Properties.Any(p => p.Name == "FullName"))
                                {
                                    var pro = Properties.FirstOrDefault(p => p.Name == "FullName");
                                    stringval = (pro.GetValue(info)).ToString();
                                }
                                else
                                {
                                    stringval = unknown;
                                }
                            }
                            break;
                        case WantExt:
                            if (val)
                            {
                                if (Properties.Any(p => p.Name == "Extension"))
                                {
                                    var pro = Properties.FirstOrDefault(p => p.Name == "Extension");
                                    stringval = (pro.GetValue(info)).ToString();
                                }
                                else
                                {
                                    stringval = unknown;
                                }
                            }
                            break;
                        case WantNameNoExt:
                            if (val)
                            {
                                if (Properties.Any(p => p.Name == "Name"))
                                {
                                    stringval = Properties.FirstOrDefault(p => p.Name == "").ToString();
                                    stringval = Path.GetFileNameWithoutExtension(stringval);
                                }
                                else
                                {
                                    stringval = unknown;
                                }
                            }
                            break;
                        case WantLastAccessUTCDate:
                            if (val)
                            {
                                WriteDateComponent(val, Properties, "LastAccessTimeUtc", out stringval, unknown, true);
                                break;
                            }
                            break;
                        case WantLastWriteUTCDate:
                            if (val)
                            {
                                WriteDateComponent(val, Properties, "LastWriteTimeUtc", out stringval, unknown, true);
                                break;
                            }
                            break;

                        case WantLastAccessUTCTime:
                            if (val)
                            {
                                WriteDateComponent(val, Properties, "LastAccessTimeUtc", out stringval, unknown, false);
                                break;
                            }
                            break;
                        case WantLastWriteUTCTime:
                            if (val)
                            {
                                WriteDateComponent(val, Properties, "LastWriteTimeUtc", out stringval, unknown, false);
                                break;
                            }
                            break;
                        case WantNameWithExt:
                            if (val)
                            {
                                if (Properties.Any(p => p.Name == "Name"))
                                {
                                    var pro = Properties.FirstOrDefault(p => p.Name == "Name");
                                    stringval = (pro.GetValue(info)).ToString();
                                }
                                else
                                {
                                    stringval = unknown;
                                }
                            }
                            break;
                        case WantSize:
                            if (val)
                            {
                                if (Properties.Any(p => p.Name == "Length"))
                                {
                                    var pro = Properties.FirstOrDefault(p => p.Name == "Length");
                                    stringval = ((long)pro.GetValue(info)).ToString();
                                }
                                else
                                {
                                    stringval = unknown;
                                }
                            }
                            break;
                    }
                    line.AppendFormat("=\"{0}\",",stringval);
                }
            }
            line.Length -= 1;

            TargetFile.WriteLine(line.ToString());
        }

        public override void Match(FileSystemInfo info)
        {
            if (First == null)
            {
                First = info;
                WriteItemNames();
            }
            else
            {
                WriteItemValues(info);
            }
            base.Match(info);
        }

        public override bool SearchBegin(DateTime Start)
        {
            string TargetLocation = GetCustomParameter(TargetSaveLocation) as string ?? throw new InvalidOperationException("Missing required target location to save");

            CheckDefault();
            TargetFile = new StreamWriter(File.OpenWrite(TargetLocation), Encoding.UTF8);
            return false;
        }

        public override void AllDone()
        {
            TargetFile.Flush();
            TargetFile.Close();
            base.AllDone();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TargetFile?.Flush();
                TargetFile?.Dispose();
                TargetFile = null;
            }
            base.Dispose(disposing);
        }

        ~OdinSearch_OutputSimpleCSVWriter()
        {
            Dispose(false);
        }

    }
}

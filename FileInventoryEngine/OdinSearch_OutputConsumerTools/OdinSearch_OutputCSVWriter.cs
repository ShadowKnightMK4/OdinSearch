using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools
{
    public class OdinSearch_OutputCSVWriter : OdinSearch_OutputConsumerBase
    {

        /// <summary>
        /// Want Attributes in the list "Directory, Hidden".
        /// </summary>
        public const string WantAttributes = "WantAttributes";
        /// <summary>
        /// 
        /// </summary>
        public const string WantCreationUTC = "WantCreationUTC";
        public const string WantDirectoryLocation = "WantDirLocation";
        public const string WantFullLocation = "WantFullLocation";
        public const string WantExt = "WantExt";
        public const string WantNameNoExt = "WantFullNameNoExt";
        public const string WantLastAccessUTC = "WantLastAccessUTC";
        public const string WantLastWriteUTC = "WantLastWriteUTC";
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
                    case WantCreationUTC:
                    case WantDirectoryLocation:
                    case WantFullLocation:
                    case WantExt:
                    case WantNameNoExt:
                    case WantLastAccessUTC:
                    case WantLastWriteUTC:
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
                            case WantCreationUTC:
                                if (val)
                                    line.Append("CreationUTC");
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
                            case WantLastAccessUTC:
                                if (val)
                                    line.Append("LastAccessUTC");
                                break;
                            case WantLastWriteUTC:
                                if (val)
                                    line.Append("LastWriteUTC");
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
            this[WantCreationUTC] = true;
            this[WantDirectoryLocation] = true;
            this[WantFullLocation] = true;
            this[WantExt] = true;
            this[WantLastAccessUTC] = true;
            this[WantLastWriteUTC] = true;
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
                        case WantCreationUTC:
                            if (val)
                            {
                                if (Properties.Any(p => p.Name == "CreationTimeUtc"))
                                {
                                    var pro = Properties.FirstOrDefault(p => p.Name == "CreationTimeUtc");
                                    stringval = ((DateTime)pro.GetValue(info)).ToString();
                                }
                                else
                                {
                                    stringval = unknown;
                                }
                            }
                            break;
                        case WantDirectoryLocation:
                            if (val)
                            {
                                if (Properties.Any(p => p.Name == "Directory"))
                                {
                                    var pro = Properties.FirstOrDefault(p => p.Name == "Directory");
                                    stringval = (pro.GetValue(info)).ToString();
                                    //stringval = Properties.FirstOrDefault(p => p.Name == "Directory").ToString();
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
                        case WantLastAccessUTC:
                            if (val)
                            {
                                if (Properties.Any(p => p.Name == "LastAccessTimeUtc"))
                                {
                                    var pro = Properties.FirstOrDefault(p => p.Name == "LastAccessTimeUtc");
                                    stringval = ((DateTime)pro.GetValue(info)).ToString();
                                }
                                else
                                {
                                    stringval = unknown;
                                }
                            }
                            break;
                        case WantLastWriteUTC:
                            if (val)
                            {
                                if (Properties.Any(p => p.Name == "LastWriteTimeUtc"))
                                {
                                    var pro = Properties.FirstOrDefault(p => p.Name == "LastWriteTimeUtc");
                                    stringval = ((DateTime)pro.GetValue(info)).ToString();
                                }
                                else
                                {
                                    stringval = unknown;
                                }
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
                    line.AppendFormat("\"{0}\",",stringval);
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
            this.TargetFile = new StreamWriter(File.OpenWrite(TargetLocation), Encoding.UTF8);
            return false;
            return base.SearchBegin(Start);
        }

        public override void AllDone()
        {
            this.TargetFile.Flush();
            this.TargetFile.Close();
            base.AllDone();
        }
    }
}

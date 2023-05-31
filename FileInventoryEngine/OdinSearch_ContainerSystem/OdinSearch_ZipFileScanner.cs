using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace OdinSearchEngine.OdinSearch_ContainerSystems
{
    /// <summary>
    /// NOT FULLY IMPLEMENTED YET
    /// </summary>
    public class OdinSearch_ZipFileScanner: OdinSearch_ContainerGeneric
    {
        protected string WalkPathForZip(string path, out string remaining)
        {
            StringBuilder ret = new StringBuilder();

            for (int step = 0; step < path.Length;step++)
            {
                ret.Append(path[step]);
                if ( (path[step] == Path.DirectorySeparatorChar) || (path[step] == Path.AltDirectorySeparatorChar))
                {
                    try
                    {
                        if (File.Exists(ret.ToString()) == true)
                        {
                            FileInfo Info = new FileInfo(ret.ToString());   
                            if (Info.LinkTarget != null)
                            {
                                ret.Clear();
                                ret.Append(Info.LinkTarget);
                            }
                            if (File.Exists(ret.ToString()) == true)
                            {
                                
                            }
                        }
                    }
                    finally
                    {

                    }
                }
            }
            remaining= string.Empty;
            return ret.ToString();
        }
        public override string[] GetContainerFiles(string Location)
        {
            string zipart;
            string remain;
            zipart = WalkPathForZip(Location, out remain);
            throw new NotImplementedException();
            using (Stream instream = File.OpenRead(zipart))
            {
                using (ZipArchive zipArchive = new ZipArchive(instream, ZipArchiveMode.Read, true))
                {

                }
            }
        }

        public override string[] GetFiles(string Location)
        {
            throw new NotImplementedException();
        }

        public override OdinSearch_ContainerSystemItem MakeInstance(string Name)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}

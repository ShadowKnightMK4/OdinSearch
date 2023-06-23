using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine.OdinSearch_ContainerSystems;
using System.IO.Compression;

namespace NonSqlUnitTests
{
    [TestClass]
    public class ContainerTests
    {
        [TestInitialize]
        public void Init() {
            Directory.CreateDirectory("C:\\TestScrubLocation");
        }

        /// <summary>
        /// We make 2 files, a normal file of length 0 and a zip file and see if the general class can work on both
        /// </summary>
        [TestMethod]
        public void MiMT_File()
        {
            using (FileItemCache Cache = new FileItemCache())
            {
                Cache.MakeFolder("C:\\TestScrubLocation", "ContainerTests");
                Cache.MakeFolder("C:\\TestScrubLocation\\ContainerTests\\MiMT_File",string.Empty);
                Cache.MakeFile("C:\\TestScrubLocation\\ContainerTests\\MiMT_File\\testfile.dat", string.Empty, FileAttributes.Normal);
                using (var str = File.Open("C:\\TestScrubLocation\\ContainerTests\\MiMT_File\\testfile.zip", FileMode.OpenOrCreate))
                {
                    using (ZipArchive Arch = new ZipArchive(str, ZipArchiveMode.Create))
                    {
                        ZipArchiveEntry Entry = Arch.CreateEntry("Demo.data");
                        using (Stream ComEntry = Entry.Open())
                        {
                            byte[] Buff = new byte[512];
                            ComEntry.Write(Buff, 0, 512);
                        }
                    }
                }
                Cache.AddItem("C:\\TestScrubLocation\\ContainerTests\\MiMT_File\\testfile.zip");


                using (var TestZip = new OdinSearch_ZippedFileInfo("C:\\TestScrubLocation\\ContainerTests\\MiMT_File\\testfile.zip\\Demo.data"))
                {
                    Assert.IsTrue(TestZip.Length == 512);
                    Assert.IsTrue(string.Compare("Demo.data", TestZip.Name) == 0);
                }


                
            }
        }
    }
}

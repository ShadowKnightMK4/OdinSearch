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
            throw new NotImplementedException();


             
        }
    }
}

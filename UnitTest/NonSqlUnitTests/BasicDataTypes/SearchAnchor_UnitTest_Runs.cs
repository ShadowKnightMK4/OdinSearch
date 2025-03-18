using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonSqlUnitTests.BasicDataTypes
{
    [TestClass]
    public class SearchAnchor_UnitTest_Runs
    {
        static CommonFileTestClass FileCache = new CommonFileTestClass();
        [ClassInitialize]
        public static void Init(TestContext te)
        {
            FileCache.SetupState(typeof(SearchAnchor_UnitTest_Runs));
        }

        [ClassCleanup]
        public static void MyCleanState()
        {
            FileCache.CleanState();
        }

        [TestMethod]
        public void SearchAnchor_AddAnchor_MultiSemiColor_Check()
        {
            FileCache.MakeAFolder("Folder1", System.IO.FileAttributes.Normal);
            FileCache.MakeAFolder("Folder2", System.IO.FileAttributes.Normal);
            FileCache.MakeAFolder("Folder3", System.IO.FileAttributes.Normal);

            
            SearchAnchor Testme = new(false);
            List<string> list = new List<string>();
            list.Add(Path.Combine(FileCache.GetRootLocation(), "Folder1"));
            list.Add(Path.Combine(FileCache.GetRootLocation(), "Folder2"));
            list.Add(Path.Combine(FileCache.GetRootLocation(), "Folder3"));
            Testme.AddAnchor(string.Join(";", list));

            Assert.IsTrue(Testme.roots.Count == list.Count);
            foreach (string s in list)
            {
                if (Testme.roots.Any( p=> { return p.FullName == s; }) == false)
                {
                    Assert.Fail("DIDNT work");
                }
            }
        }
    }
}

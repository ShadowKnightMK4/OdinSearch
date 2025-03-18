using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine;
namespace NonSqlUnitTests.BasicDataTypes
{
    [TestClass]
    public class OdinSearch_UnitTest_MakeInstanceOf
    {
        static CommonFileTestClass FileCache = new CommonFileTestClass();
        [ClassCleanup]
        public static void MyCleanState()
        {
            FileCache.CleanState();
        }

        [ClassInitialize]
        public static void MySetupState(TestContext te)
        {
            FileCache.SetupState(typeof(OdinSearch_UnitTest_MakeInstanceOf));
        }
        [TestMethod]
        public void OdenSearch_CanMakeInstance()
        {
            OdinSearch test = new OdinSearch();
            Assert.IsNotNull(test);
        }

    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonSqlUnitTests.BasicDataTypes
{
    [TestClass]
    public class OdinSearch_UnitTest_AssignsBase
    { 
        static CommonFileTestClass FileCache = new CommonFileTestClass();
        [ClassCleanup]
        public static void MyCleanState()
        {
            FileCache.CleanState();
        }

        [ClassInitialize]
        public static void MySetupState(TestContext e)
        {
            FileCache.SetupState(typeof(OdinSearch_UnitTest_AssignsBase));
        }

        [TestMethod]
        public void OdinSearch_UnitTest_CanReadWrite_SoleMatch_bool()
        {
            OdinSearch TestMe = new();
            Assert.IsNotNull(TestMe);

            TestMe.SoleMatch = true;
            Assert.IsTrue(TestMe.SoleMatch);
            TestMe.SoleMatch = false;
            Assert.IsFalse(TestMe.SoleMatch);
        }

        [TestMethod]    
        public void OdinSearch_UnitTest_CanWriteRead_DebugVerboseMode_bool()
        {
            OdinSearch TestMe = new();
            Assert.IsNotNull(TestMe);

            TestMe.DebugVerboseMode = true;
            Assert.IsTrue(TestMe.DebugVerboseMode);
            TestMe.DebugVerboseMode = false;
            Assert.IsFalse(TestMe.DebugVerboseMode);
        }


        [TestMethod]
        public void OdinSearch_UnitTest_IsZombied_ReturnsFalse_OutsideOf_Search_bool()
        {
            OdinSearch TestMe = new();
            Assert.IsNotNull(TestMe);

            Assert.IsFalse(TestMe.IsZombied);
        }


        [TestMethod]
        public void OdinSearch_UnitTest_HasActiveSearchThreads_ReturnsFalse_OutsideOf_Search_bool()
        {
            OdinSearch TestMe = new();
            Assert.IsNotNull(TestMe);

            Assert.IsFalse(TestMe.HasActiveSearchThreads);
        }


        [TestMethod]
        public void OdinSearch_UnitTest_CanWriteRead_ThreadSynchResults_bool()
        {
            OdinSearch TestMe = new();
            Assert.IsNotNull(TestMe);

            TestMe.ThreadSynchResults = true;
            Assert.IsTrue(TestMe.ThreadSynchResults);

            TestMe.ThreadSynchResults = false;
            Assert.IsFalse(TestMe.ThreadSynchResults);
        }


        [TestMethod]
        public void OdinSearch_UnitTest_DupMode_Starts_AsDefault_enum()
        {
            OdinSearch TestMe = new();
            Assert.IsNotNull(TestMe);


            Assert.AreEqual(TestMe.DupMode, OdinSearch_DupCheck_Mode.Default);
        }

        [TestMethod]
        public void OdinSearch_UnitTest_DupMode_CanAssign_Everything_enum()
        {
            OdinSearch TestMe = new();
            Assert.IsNotNull(TestMe);


            Assert.AreEqual(TestMe.DupMode, OdinSearch_DupCheck_Mode.Default);

            TestMe.DupMode = OdinSearch_DupCheck_Mode.Everything;
            Assert.AreEqual(TestMe.DupMode, OdinSearch_DupCheck_Mode.Everything);
        }


        [TestMethod]
        public void OdinSearch_UnitTest_DupMode_CanAssign_Hints_enum()
        {
            OdinSearch TestMe = new();
            Assert.IsNotNull(TestMe);


            Assert.AreEqual(TestMe.DupMode, OdinSearch_DupCheck_Mode.Default);

            TestMe.DupMode = OdinSearch_DupCheck_Mode.OnlyHints;
            Assert.AreEqual(TestMe.DupMode, OdinSearch_DupCheck_Mode.OnlyHints);
        }



        [TestMethod]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void OdinSearch_UnitTest_DupMode_Rejects_InvalidMode()
        {
            OdinSearch TestMe = new();
            Assert.IsNotNull(TestMe);


            Assert.AreEqual(TestMe.DupMode, OdinSearch_DupCheck_Mode.Default);

            TestMe.DupMode = (OdinSearch_DupCheck_Mode)int.MaxValue;
            Assert.Fail($"This routine didn't throw the expected exception. {typeof(InvalidEnumArgumentException).Name}. Ensure the property for DupCheck actually throws the exception.");
        }


        [TestMethod]
        public void OdinSearch_UnitTest_CanReadWrite_SkipSanityCheck()
        {
            OdinSearch TestMe = new();
            Assert.IsNotNull(TestMe);

            TestMe.SkipSanityCheck = true;
            Assert.IsTrue(TestMe.SkipSanityCheck);
            TestMe.SkipSanityCheck = false;
            Assert.IsFalse(TestMe.SkipSanityCheck);
        }


        [TestMethod]
        public void OdinSearch_UnitTest_WorkerThreadCount_Zero_OutsideOf_Search()
        {
            OdinSearch TestMe = new();
            Assert.IsNotNull(TestMe);

            Assert.IsTrue(TestMe.WorkerThreadCount == 0);
        }


        [TestMethod]
        public void OdinSearch_UnitTest_WorkerThreadCrashed_False_OutsideOf_Search()
        {
            OdinSearch TestMe = new();
            Assert.IsNotNull(TestMe);

            Assert.IsTrue(TestMe.WorkerThreadCrashed == false);
        }



    }
}

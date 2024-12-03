using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonSqlUnitTests.BasicDataTypes
{
    /// <summary>
    /// This class tests the class for accepting good assigns and throwing exceptions for bad ones.
    /// </summary>
    [TestClass]
    public class SearchTarget_UnitTest_Assigns
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
            FileCache.SetupState(typeof(SearchTarget_UnitTest_Assigns));
        }

        [TestMethod]
        public void SearchTarget_CanMakeInstance()
        {
            SearchTarget Testme = new();
            Assert.IsNotNull(Testme);
        }

        [DataTestMethod]
        [DataRow(SearchTarget.MatchStyleFileAttributes.Skip)]
        [DataRow(SearchTarget.MatchStyleFileAttributes.Reserved)]
        [DataRow(SearchTarget.MatchStyleFileAttributes.Invert)]
        //[DataRow(SearchTarget.MatchStyleFileAttributes.Reserved)]
        [DataRow(SearchTarget.MatchStyleFileAttributes.MatchAll)]
        [DataRow(SearchTarget.MatchStyleFileAttributes.MatchAny)]

        public void SearchTarget_CanReadWrite_Valid_MatchStyleFileAttributes1_enum(SearchTarget.MatchStyleFileAttributes Input)
        {
            SearchTarget TestMe = new();
            Assert.IsNotNull(TestMe);

            TestMe.AttribMatching1Style = Input;
            Assert.AreEqual(TestMe.AttribMatching1Style, Input);
        }


        [DataTestMethod]
        [DataRow(SearchTarget.MatchStyleFileAttributes.Skip)]
        [DataRow(SearchTarget.MatchStyleFileAttributes.Reserved)]
        [DataRow(SearchTarget.MatchStyleFileAttributes.Invert)]
        //[DataRow(SearchTarget.MatchStyleFileAttributes.Reserved)]
        [DataRow(SearchTarget.MatchStyleFileAttributes.MatchAll)]
        [DataRow(SearchTarget.MatchStyleFileAttributes.MatchAny)]

        public void SearchTarget_CanReadWrite_Valid_MatchStyleFileAttributes2_enum(SearchTarget.MatchStyleFileAttributes Input)
        {
            SearchTarget TestMe = new();
            Assert.IsNotNull(TestMe);

            TestMe.AttribMatching2Style = Input;
            Assert.AreEqual(TestMe.AttribMatching2Style, Input);
        }




        #region file size min
        [TestMethod]
        public void SearchTarget__CanReadWrite_FileSizeMin()
        {
            long val1 = 50;
            long val2 = 1235214;
            SearchTarget TestMe = new();
            Assert.IsNotNull(TestMe);

            
            TestMe.FileSizeMin = val1;
            Assert.AreEqual(TestMe.FileSizeMin, val1);

            TestMe.FileSizeMin = val2;
            Assert.AreEqual(TestMe.FileSizeMin, val2);
        }
        #endregion

        #region file size max

        [TestMethod]
        public void SearchTarget__CanReadWrite_FileSizeMax()
        {
            long val1 = 50000;
            long val2 = 123314;
            SearchTarget TestMe = new();
            Assert.IsNotNull(TestMe);


            TestMe.FileSizeMax = val1;
            Assert.AreEqual(TestMe.FileSizeMax, val1);

            TestMe.FileSizeMax = val2;
            Assert.AreEqual(TestMe.FileSizeMax, val2);
        }
        #endregion

        #region file attributes 1
        [DataTestMethod]
        [DataRow(0)] // special case for the first attribute check. It means skip, is considered valid despite not being an actual part of the enum
        [DataRow(FileAttributes.Archive)]
        [DataRow(FileAttributes.Compressed)]
        [DataRow(FileAttributes.Device)]
        [DataRow(FileAttributes.Directory)]
        [DataRow(FileAttributes.Encrypted)]
        [DataRow(FileAttributes.Hidden)]
        [DataRow(FileAttributes.IntegrityStream)]
        [DataRow(FileAttributes.Normal)]
        [DataRow(FileAttributes.NoScrubData)]
        [DataRow(FileAttributes.NotContentIndexed)]
        [DataRow(FileAttributes.Offline)]
        [DataRow(FileAttributes.ReadOnly)]
        [DataRow(FileAttributes.ReparsePoint)]
        [DataRow(FileAttributes.SparseFile)]
        [DataRow(FileAttributes.System)]
        [DataRow(FileAttributes.Temporary)]

        public void SearchTarget__CanReadWrite_ValidFileAttributes_AttributeMatching1(FileAttributes One)
        {
            SearchTarget TestMe = new();
            TestMe.AttributeMatching1 = One;
            Assert.AreEqual(TestMe.AttributeMatching1, One);
        }

        [DataTestMethod]
        [DataRow(-2)]
        [DataRow(262144)] // Invalid
        [DataRow(524288)] // Invalid
        [DataRow(1048576)] // Invalid
        [DataRow(2097152)] // Invalid
        [DataRow(4194304)] // Invalid
        [DataRow(8388608)] // Invalid
        [DataRow(16777216)] // Invalid
        [DataRow(33554432)] // Invalid
        [DataRow(67108864)] // Invalid
        [DataRow(134217728)] // Invalid
        [DataRow(268435456)] // Invalid
        [DataRow(536870912)] // Invalid
        [DataRow(1073741824)] // Invalid
        [DataRow(int.MaxValue)] // Invalid
        [ExpectedException(typeof(InvalidEnumArgumentException))]

        public void SearchTarget__RejectsBad_FileAttributes_AttributeMatching1(FileAttributes One)
        {
            SearchTarget TestMe = new();
            TestMe.AttributeMatching1 = One;
            Assert.Fail($"The code for assigning AttributeMatching1 failed to reject invalid attributes by throwing an exception");
        }
        #endregion

        #region file attributes 2
        // attriub2
        [DataTestMethod]
        //[DataRow(0)] // special case for the first attribute check. It means skip, is considered valid despite not being an actual part of the enum
        [DataRow(FileAttributes.Archive)]
        [DataRow(FileAttributes.Compressed)]
        [DataRow(FileAttributes.Device)]
        [DataRow(FileAttributes.Directory)]
        [DataRow(FileAttributes.Encrypted)]
        [DataRow(FileAttributes.Hidden)]
        [DataRow(FileAttributes.IntegrityStream)]
        [DataRow(FileAttributes.Normal)]
        [DataRow(FileAttributes.NoScrubData)]
        [DataRow(FileAttributes.NotContentIndexed)]
        [DataRow(FileAttributes.Offline)]
        [DataRow(FileAttributes.ReadOnly)]
        [DataRow(FileAttributes.ReparsePoint)]
        [DataRow(FileAttributes.SparseFile)]
        [DataRow(FileAttributes.System)]
        [DataRow(FileAttributes.Temporary)]

        public void SearchTarget__CanReadWrite_ValidFileAttributes_AttributeMatching2(FileAttributes One)
        {
            SearchTarget TestMe = new();
            TestMe.AttributeMatching2 = One;
            Assert.AreEqual(TestMe.AttributeMatching2, One);
        }

        [DataTestMethod]
        [DataRow(262144)] // Invalid
        [DataRow(524288)] // Invalid
        [DataRow(1048576)] // Invalid
        [DataRow(2097152)] // Invalid
        [DataRow(4194304)] // Invalid
        [DataRow(8388608)] // Invalid
        [DataRow(16777216)] // Invalid
        [DataRow(33554432)] // Invalid
        [DataRow(67108864)] // Invalid
        [DataRow(134217728)] // Invalid
        [DataRow(268435456)] // Invalid
        [DataRow(536870912)] // Invalid
        [DataRow(1073741824)] // Invalid
        [DataRow(int.MaxValue)] // Invalid
        [ExpectedException(typeof(InvalidEnumArgumentException))]

        public void SearchTarget__RejectsBad_FileAttributes_AttributeMatching2(FileAttributes One)
        {
            SearchTarget TestMe = new();
            TestMe.AttributeMatching2 = One;
            Assert.Fail($"The code for assigning AttributeMatching2 failed to reject invalid attributes by throwing an exception");
        }

        #endregion

        #region check file size
        [TestMethod]
        void SearchTarget_CanReadWrite_CheckFileSize()
        {
            SearchTarget TestMe = new();
            Assert.IsFalse(TestMe.CheckFileSize);
            TestMe.CheckFileSize = true;
            Assert.IsTrue(TestMe.CheckFileSize);
        }
        #endregion

        #region creationanchor1
        [DataTestMethod]
        [DataRow("2024-08-02T11:51:15Z")] // ISO 8601
        [DataRow("08/02/2024 11:51:15 AM")] // US format
        [DataRow("02/08/2024 11:51:15")] // European format
        [DataRow("Friday, August 2, 2024 11:51:15 AM")] // Full date/time pattern (long)
        [DataRow("2024-08-02")] // Date only
        [DataRow("11:51:15")] // Time only
        [DataRow("2024-08-02 11:51:15.123")] // Date and time with milliseconds
        [DataRow("2024-08-02T11:51:15+00:00")] // ISO 8601 with timezone
        [DataRow("2024-08-02T11:51:15.1234567Z")] // ISO 8601 with nanoseconds
        [DataRow("2024-08-02 11:51:15 PM")] // 12-hour clock with PM]
        public void SearchTarget_CanReadWrite_CreationAnchor1(string x)
        {
            bool test;
            var time = DateTime.Parse(x);
            SearchTarget TestMe = new();
            TestMe.CreationAnchor = time;
            test = (0 == time.CompareTo(TestMe.CreationAnchor));
            Assert.IsTrue(test);
        }
        #endregion

        #region creation anchor2
        [DataTestMethod]
        [DataRow("2024-08-02T11:51:15Z")] // ISO 8601
        [DataRow("08/02/2024 11:51:15 AM")] // US format
        [DataRow("02/08/2024 11:51:15")] // European format
        [DataRow("Friday, August 2, 2024 11:51:15 AM")] // Full date/time pattern (long)
        [DataRow("2024-08-02")] // Date only
        [DataRow("11:51:15")] // Time only
        [DataRow("2024-08-02 11:51:15.123")] // Date and time with milliseconds
        [DataRow("2024-08-02T11:51:15+00:00")] // ISO 8601 with timezone
        [DataRow("2024-08-02T11:51:15.1234567Z")] // ISO 8601 with nanoseconds
        [DataRow("2024-08-02 11:51:15 PM")] // 12-hour clock with PM]
        public void SearchTarget_CanReadWrite_CreationAnchor2(string x)
        {
            bool test;
            var time = DateTime.Parse(x);
            SearchTarget TestMe = new();
            TestMe.CreationAnchor2 = time;
            test = (0 == time.CompareTo(TestMe.CreationAnchor2));
            Assert.IsTrue(test);
        }
        #endregion

        [DataTestMethod]
        [DataRow(SearchTarget.MatchStyleDateTime.Disable)]
        [DataRow(SearchTarget.MatchStyleDateTime.NoLaterThanThis)]
        [DataRow(SearchTarget.MatchStyleDateTime.NoEarlierThanThis)]
        public void SearchTarget_CanReadWrite_CreationMatchStyle1_goodenum(SearchTarget.MatchStyleDateTime Style)
        {
            SearchTarget TestMe = new();
            TestMe.CreationAnchorCheck1 = Style;
        }

        [DataTestMethod]
        [DataRow(-9992)]
        [DataRow(4)]
        [DataRow(-1)]
        [DataRow(3)]
        [DataRow(99992)]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void SearchTarget_CanReadWrite_CreationMatchStyle1_badenum(SearchTarget.MatchStyleDateTime Style)
        {
            SearchTarget TestMe = new();
            TestMe.CreationAnchorCheck1 = Style;
            Assert.Fail("CreationAnchorCheck1 failed to sucessfully reject invalid input. Ensure it does.");
        }



        [DataTestMethod]
        [DataRow(SearchTarget.MatchStyleDateTime.Disable)]
        [DataRow(SearchTarget.MatchStyleDateTime.NoLaterThanThis)]
        [DataRow(SearchTarget.MatchStyleDateTime.NoEarlierThanThis)]
        public void SearchTarget_CanReadWrite_CreationMatchStyle2_goodenum(SearchTarget.MatchStyleDateTime Style)
        {
            SearchTarget TestMe = new();
            TestMe.CreationAnchorCheck2 = Style;
        }

        [DataTestMethod]
        [DataRow(-9992)]
        [DataRow(4)]
        [DataRow(-1)]
        [DataRow(3)]
        [DataRow(99992)]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void SearchTarget_CanReadWrite_CreationMatchStyle2_badenum(SearchTarget.MatchStyleDateTime Style)
        {
            SearchTarget TestMe = new();
            TestMe.CreationAnchorCheck2 = Style;
            Assert.Fail("CreationAnchorCheck2 failed to sucessfully reject invalid input. Ensure it does.");
        }


        [DataTestMethod]
        [DataRow(SearchTarget.MatchStyleDateTime.Disable)]
        [DataRow(SearchTarget.MatchStyleDateTime.NoLaterThanThis)]
        [DataRow(SearchTarget.MatchStyleDateTime.NoEarlierThanThis)]
        public void SearchTarget_CanReadWrite_AccessAnchorCheck1_goodenum(SearchTarget.MatchStyleDateTime Style)
        {
            SearchTarget TestMe = new();
            TestMe.AccessAnchorCheck1 = Style;
        }

        [DataTestMethod]
        [DataRow(-9992)]
        [DataRow(4)]
        [DataRow(-1)]
        [DataRow(3)]
        [DataRow(99992)]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void SearchTarget_CanReadWrite_AccessAnchorCheck1_badenum(SearchTarget.MatchStyleDateTime Style)
        {
            SearchTarget TestMe = new();
            TestMe.AccessAnchorCheck1 = Style;
            Assert.Fail("AccessAnchorCheck1 failed to sucessfully reject invalid input. Ensure it does.");
        }


        [DataTestMethod]
        [DataRow(SearchTarget.MatchStyleDateTime.Disable)]
        [DataRow(SearchTarget.MatchStyleDateTime.NoLaterThanThis)]
        [DataRow(SearchTarget.MatchStyleDateTime.NoEarlierThanThis)]
        public void SearchTarget_CanReadWrite_AccessAnchorCheck2_goodenum(SearchTarget.MatchStyleDateTime Style)
        {
            SearchTarget TestMe = new();
            TestMe.AccessAnchorCheck2 = Style;
        }

        [DataTestMethod]
        [DataRow(-9992)]
        [DataRow(4)]
        [DataRow(-1)]
        [DataRow(3)]
        [DataRow(99992)]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void SearchTarget_CanReadWrite_AccessAnchorCheck2_badenum(SearchTarget.MatchStyleDateTime Style)
        {
            SearchTarget TestMe = new();
            TestMe.AccessAnchorCheck2 = Style;
            Assert.Fail("AccessAnchorCheck2 failed to sucessfully reject invalid input. Ensure it does.");
        }




        [DataTestMethod]
        [DataRow(SearchTarget.MatchStyleDateTime.Disable)]
        [DataRow(SearchTarget.MatchStyleDateTime.NoLaterThanThis)]
        [DataRow(SearchTarget.MatchStyleDateTime.NoEarlierThanThis)]
        public void SearchTarget_CanReadWrite_WriteAnchorCheck1_goodenum(SearchTarget.MatchStyleDateTime Style)
        {
            SearchTarget TestMe = new();
            TestMe.WriteAnchorCheck1 = Style;
        }

        [DataTestMethod]
        [DataRow(-9992)]
        [DataRow(4)]
        [DataRow(-1)]
        [DataRow(3)]
        [DataRow(99992)]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void SearchTarget_CanReadWrite_WriteAnchorCheck1_badenum(SearchTarget.MatchStyleDateTime Style)
        {
            SearchTarget TestMe = new();
            TestMe.WriteAnchorCheck1 = Style;
            Assert.Fail("WriteAnchorCheck1 failed to sucessfully reject invalid input. Ensure it does.");
        }





        [DataTestMethod]
        [DataRow(SearchTarget.MatchStyleDateTime.Disable)]
        [DataRow(SearchTarget.MatchStyleDateTime.NoLaterThanThis)]
        [DataRow(SearchTarget.MatchStyleDateTime.NoEarlierThanThis)]
        public void SearchTarget_CanReadWrite_WriteAnchorCheck2_goodenum(SearchTarget.MatchStyleDateTime Style)
        {
            SearchTarget TestMe = new();
            TestMe.WriteAnchorCheck2 = Style;
        }

        [DataTestMethod]
        [DataRow(-9992)]
        [DataRow(4)]
        [DataRow(-1)]
        [DataRow(3)]
        [DataRow(99992)]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void SearchTarget_CanReadWrite_WriteAnchorCheck2_badenum(SearchTarget.MatchStyleDateTime Style)
        {
            SearchTarget TestMe = new();
            TestMe.WriteAnchorCheck2 = Style;
            Assert.Fail("WriteAnchorCheck1 failed to sucessfully reject invalid input. Ensure it does.");
        }
    }

}
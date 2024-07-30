using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonSqlUnitTests.OlderTests.Basic_DataTypes
{
    [TestClass]
    public class UnitTest_OdenSearch_SearchTargets
    {
        string alt_name = "*.dll";
        string original_name = "*.exe";
        static CommonFileTestClass FileState = new();
        [ClassCleanup]
        public static void MyClean()
        {
            FileState.CleanState();
        }

        [ClassInitialize]
        public static void MySetup(TestContext e)
        {
            FileState.SetupState(typeof(UnitTest_OdenSearch_SearchTargets));
        }


        /// <summary>
        /// add two SearchTarget to list, see if the OdinSearch class takes it. Assert the items are added ok.
        /// </summary>
        /// <remarks>Covers  attemptiny to add via the <see cref="OdinSearch.AddSearchTarget(IEnumerable{SearchTarget})"/></remarks>
        [TestMethod]
        public void OdinSearch_SearchTarget_CanAssignMultiTargetOK()
        {
            SearchTarget target = new SearchTarget();
            SearchTarget anything = SearchTarget.AllFiles;

            target.FileName.Add(original_name);
            OdinSearch test = new();
            List<SearchTarget> targets = new List<SearchTarget>();
            targets.Add(target);
            targets.Add(anything);

            test.AddSearchTarget(targets);

            var arr = test.GetSearchTargetsAsArray();
            Assert.IsNotNull(arr);
            Assert.IsTrue(arr.Length == 2);
            Assert.IsTrue(arr[0].FileName.Count == 1);
            Assert.IsTrue(arr[0].FileName[0] == original_name);

            Assert.AreEqual(arr[1], anything);
        }
        [TestMethod]
        public void OdinSearch_SearchTarget_CanAssignSingleTargetOk()
        {
            SearchTarget target = new SearchTarget();
            target.FileName.Add(original_name);
            OdinSearch test = new();
            test.AddSearchTarget(target);

            var arr = test.GetSearchTargetsAsArray();
            Assert.IsNotNull(arr);
            Assert.IsTrue(arr.Length == 1);
            Assert.IsTrue(arr[0].FileName.Count == 1);
            Assert.IsTrue(arr[0].FileName[0] == original_name);
        }
    }
}

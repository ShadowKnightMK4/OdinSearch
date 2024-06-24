using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine;
using OdinSearchEngine.SearchSupport;
namespace NonSqlUnitTests
{
    [TestClass]
    public class DupSearchUnitTests
    {
        [TestMethod]
        public void DoesItWork_Theory()
        {
            DupSearchPruning testme = new DupSearchPruning();
            bool first = testme.CheckToPrune("C:\\Windows");
            bool second = testme.CheckToPrune("C:\\Windows");

            Assert.IsFalse(first);
            Assert.IsTrue(second);
        }

        [TestMethod]
        public void DoesItWork_Actually()
        {
            OdinSearch TestMe = new();
            SearchAnchor One = new SearchAnchor(false);
            SearchAnchor Two = new SearchAnchor(false);
            One.EnumSubFolders = true;
            Two.EnumSubFolders = true;
            One.AddAnchor("C:\\Euphoria");
            Two.AddAnchor("C:\\Euphoria\\demo");

            TestMe.AddSearchAnchor( new SearchAnchor[] { One, Two });
            TestMe.AddSearchTarget(SearchTarget.AllFiles);
            One.EnumSubFolders = true;
            Two.EnumSubFolders = true;

            Thread.Sleep(200);
            TestMe.Search(new OdinSearchEngine.OdinSearch_OutputConsumerTools.OdinSearch_OutputConsumerGatherResults());
            TestMe.WorkerThreadJoin();
            TestMe.KillSearch();

            Console.WriteLine("Note this one requires seeing the output manually");
        }
    }
}

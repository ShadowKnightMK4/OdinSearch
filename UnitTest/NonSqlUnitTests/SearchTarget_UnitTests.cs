using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine;

namespace NonSqlUnitTests
{
    [TestClass]
    public class SearchTarget_UnitTests
    {
        [TestMethod]
        public void SearchTarget_EqualCompareOperators()
        {
            
            SearchTarget target = new SearchTarget();
            SearchTarget target3 = new SearchTarget();

            if ( (!target3.Equals(target)) || (target3 != target) )
            {
                Assert.Fail("Compare Failed");
            }
            Assert.IsTrue(target == target3);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NonSqlUnitTests.ConsumerBaseTests
{
    [TestClass]
    public class FolderSizeCount
    {
        [TestMethod]
        public void Test()
        {
            OdinSearch TestEngine  = new OdinSearch();
            SearchAnchor Start = new SearchAnchor("C:\\Euphoria");
            SearchTarget All = SearchTarget.AllFiles;

            TestEngine.AddSearchAnchor(Start);
            TestEngine.AddSearchTarget(All);

            OdinSearchEngine.OdinSearch_OutputConsumerTools.FolderSizeCataloger target = new();

            TestEngine.Search(target);

            Thread.Sleep(200);
            while (!TestEngine.IsZombied )
            {
                Thread.Sleep(100);
            }
            Console.WriteLine("Results");
            
            foreach (var entry in target.FolderSizes)
            {
                Console.WriteLine($"{entry.FolderPath} is {entry.Size} bytes big" )    ;
            }

        }

    }
}

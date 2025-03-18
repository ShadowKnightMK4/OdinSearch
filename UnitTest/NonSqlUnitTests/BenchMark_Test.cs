using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NonSqlUnitTests
{
    [TestClass]
    public class BenchMark_Test
    {
        [TestMethod]
        public void a()
        {
            Task x = Task.Run(() =>
            {
                OdinSearch Search = new();
                SearchTarget x = SearchTarget.AllFiles;
                SearchAnchor start = new SearchAnchor(@"C:\Windows");
                start.EnumSubFolders = true;
                var Process = new OdinSearchEngine.OdinSearch_OutputConsumerTools.OdinSearch_OutputSimpleConsole();
                Process[OdinSearchEngine.OdinSearch_OutputConsumerTools.OdinSearch_OutputSimpleConsole.FlushAlways] = true;
                Search.AddSearchAnchor(start);
                Search.AddSearchTarget(x);
                Search.Search(Process);
                while (Search.HasActiveSearchThreads)
                {
                    Thread.Sleep(2000);
                }
            });
            //if (!x.Wait(3000*20))
            x.Wait(); return;
            {
                throw new TimeoutException("Ran out of time");
            }
        }
    }
}

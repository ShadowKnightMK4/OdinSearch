using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine;
using OdinSearchEngine.OdinSearch_OutputConsumerTools.ExternalBased;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NonSqlUnitTests
{
    [TestClass]
    public class OdinSearch_extern_unmanaged_plugin_test

    { 
        /// <summary>
        /// Update this to the plugin to test with.
        /// </summary>
        public string StaticHardcodedPlugin = "C:\\Users\\Thoma\\source\\repos\\FileInventory\\x64\\Debug\\ExternalComsPlugin.dll";

        [TestMethod]
        public void TestExternUnmanaged()
        {
            OdinSearch Demo = new OdinSearch();
            OdinSearch_OutputConsumer_UnmanagedPlugin Test = new();
            Test.SetPluginLocation(StaticHardcodedPlugin);

            SearchAnchor a = new();
            SearchTarget b = new();
            b.FileName.Add(SearchTarget.MatchAnyFileName);

            Demo.AddSearchAnchor(a);
            Demo.AddSearchTarget(b);

            Demo.Search(Test);


            while (true)
            {
                Thread.Sleep(100);
                if (Demo.IsZombied)
                {
                    Test.ResolvePendingActions();
                    break;
                }
            }

        }
    }
}

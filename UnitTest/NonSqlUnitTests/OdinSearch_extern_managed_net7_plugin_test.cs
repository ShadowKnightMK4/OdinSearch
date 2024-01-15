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
    public class OdinSearch_extern_managed_net7_plugin_test

    {
        /// <summary>
        /// Update this to the plugin to test with.
        /// </summary>
        public string StaticHardcodedPlugin = "C:\\Users\\Thoma\\source\\repos\\FileInventory\\ExternalManagedComsPlugin\\bin\\Debug\\net7.0\\ExternalManagedComsPlugin.dll";

        [TestMethod]
        public void TestExternManaged()
        {
            OdinSearch Demo = new OdinSearch();
            OdinSearch_OutputConsumer_ExternManaged Test = new(StaticHardcodedPlugin, null, "OdinSearch_OutputConsumerBaseTest");
            
            SearchAnchor a = new(false);

            SearchTarget b = new();
            b.FileName.Add(SearchTarget.MatchAnyFile);
            a.AddAnchor("C:\\Windows");
            a.EnumSubFolders = false;
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

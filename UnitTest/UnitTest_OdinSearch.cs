﻿using OdinSearchEngine;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class UnitTest_OdinSearch
    {
        OdinSearch Demo = null;
        [TestInitialize]
        public void Init()
        { 
            Demo = new OdinSearch();
        }
        [TestMethod]
        public void OdinSearch_CanInstance()
        {
            Assert.IsNotNull(Demo);
        }

        /// <summary>
        /// used for testing. It sets various public bools when doing 
        /// </summary>
        internal class OdinSearch_Output_UnitTesting_class: OdinSearch_OutputConsumerBase
        {
            public bool WasMatchCalled = false;
            public bool WasBlockedCalled = false;
            public bool WasMessagingCalled = false;
            public bool WasItemNotMatchingCalled = false;
            
            public override void Blocked(string Blocked)
            {
                
            }

            public override void Messaging(string Message)
            {
                
            }

            public override void Match(FileSystemInfo info)
            {
                WasMatchCalled = true;
                return;
            }
        }


        [TestMethod]
        public void OdinSearch_Can_SearchForWindowsFolder()
        {
            OdinSearch_Output_UnitTesting_class coms = new OdinSearch_Output_UnitTesting_class();
            Assert.IsNotNull(Demo);
            Demo.Reset();


            var TestAnchor = new SearchAnchor(false);
            

            var TestSearch = new SearchTarget();
            
            Demo.AddSearchAnchor(TestAnchor);
            Demo.AddSearchTarget(TestSearch);

            TestAnchor.AddAnchor("C:\\");
            TestSearch.FileName.Add("Windows");
            TestSearch.FileNameMatching = SearchTarget.MatchStyleString.MatchAny;
            Demo.Search(coms);

            
            {
                Demo.WorkerThreadJoin();
            }

            if (coms.WasMatchCalled == false)
            {
                Assert.Fail("Attempt to locate C:\\Windows with the OdinSearch failed");
            }
        }
    }
}

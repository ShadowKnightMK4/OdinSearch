using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class UnitTestOdinSearch_OutputConsumerSql_UnitTests
    {
        OdinSearch_OutputConsumerSql Demo=null;
        SqlException LocalConnectOk_Catche = null;

        [TestInitialize]
        public void Init()
        {
            try
            {
                Demo = new OdinSearch_OutputConsumerSql(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Testdb.mdf"));
            }
            catch (SqlException e)
            {
                LocalConnectOk_Catche = e;
            }
        }

        [TestMethod]
        public void LocalDB_ConnectOk()
        {
            // init() test actualy connects. THis tests for Demo != null and the LocalConnectOk_Catche = null

            Assert.IsNotNull(Demo);
            if (LocalConnectOk_Catche != null)
            {
                Assert.Fail("SQL Exception on connect: " + LocalConnectOk_Catche.Message);
            }


        }
    }
}

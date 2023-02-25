using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine.OdinSearch_OutputConsumerTools;
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
            if (!Directory.Exists("V:\\DevDB"))
            {
                Directory.CreateDirectory("V:\\DevDB");
            }
            try
            {
                Demo = new OdinSearch_OutputConsumerSql(string.Empty);
            }
            catch (SqlException e)
            {
                LocalConnectOk_Catche = e;
            }
        }

        
        [TestMethod]
        public void LocalDB_CreateDatabaseOk()
        {
            Assert.IsNotNull(Demo);
            var connect = Demo.GetSqlConnect();

            if (!OdinSearch_OutputConsumerSql_CommandHandler.CreateSqlDatabase(connect, "TestDB2"))
            {
                Assert.Fail("Failed to create databaseok");
            }

            if (!OdinSearch_OutputConsumerSql_CommandHandler.GetSqlDatabaseList(connect).Contains("TestDB2"))
            {
                Assert.Fail("Failed to insert DB ok");
            }
        }
        [TestMethod]
        public void LocalDB_ConnectOk()
        {
            // init() test actualy connects. THis tests for Demo != null and the LocalConnectOk_Catche = null

            
            if (LocalConnectOk_Catche != null)
            {
                Assert.Fail("SQL Exception on connect: " + LocalConnectOk_Catche.Message);
            }
            Assert.IsNotNull(Demo);
        }

        [TestMethod]
        public void LocalDB_Test2()
        {
            Assert.IsNotNull(Demo);

            SqlConnection sql = Demo.GetSqlConnect();
         
            
        }
    }
}

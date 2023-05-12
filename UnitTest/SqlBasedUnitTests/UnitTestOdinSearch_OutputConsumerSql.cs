using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine;
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


        [TestMethod]
        public void FromTypeInstance_To_Sql_insert()
        {
            var connect = Demo.GetSqlConnect();
            string TestFileName = "C:\\Dummy\\A" + DateTime.Now.Ticks.ToString() + ".MD45";
            string TestDB = Path.GetFileNameWithoutExtension(TestFileName);
            string TestTable = "B" + DateTime.Now.Ticks.ToString();
            // create the temp database
            if (!OdinSearchSql.CreateSqlDatabase(connect, TestFileName))
            {
                Assert.Fail("Failed to create databaseok");
            }
            // create a table based on type

            if (!OdinSearchSql.CreateTableFromType(connect, TestTable, typeof(FileInfoExtract)))
            {
                Assert.Fail("Failed to database table and record ok");
            }

            // select the temp database
            OdinSearchSql.SelectDatabase(connect, TestDB);
            
            // does it contain a TestTab;e
            Assert.IsTrue(OdinSearchSql.GetTableList(connect).Contains(TestTable));

            // if so delete it
            OdinSearchSql.DeleteTable(connect, TestTable);

            // did delete work
            Assert.IsFalse(OdinSearchSql.GetTableList(connect).Contains(TestTable));


            // Finally delete the database
            if (!OdinSearchSql.DeleteSqlDataBase(connect, TestFileName))
            {
                Assert.Fail("Failed to delete databaseok");
            }
            // and the file
            {
                File.Delete(TestFileName);
            }
        }
            
            [TestMethod]
        public void CS_Class_To_SqlRecord_manuel_check()
        {
            string result;
            result = OdinSearchSql.ConvertClassToSqlRecord(typeof(OdinSearchEngine.FileInfoExtract));


            return;
        }
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
        public void LocalDB_CreateDatabaseOK_AndDelete()
        {
            Assert.IsNotNull(Demo);
            var connect = Demo.GetSqlConnect();
            string TestName = "C:\\Dummy\\A" + DateTime.Now.Ticks.ToString() + ".MD45";

            if (!OdinSearchSql.CreateSqlDatabase(connect, TestName))
            {
                Assert.Fail("Failed to create databaseok");
            }

            Assert.IsTrue(OdinSearchSql.GetSqlDatabaseList(connect).Contains(Path.GetFileNameWithoutExtension(TestName)));

            if (!OdinSearchSql.DeleteSqlDataBase(connect, TestName))
            {
                Assert.Fail("Failed to delete databaseok");
            }

            Assert.IsFalse(OdinSearchSql.GetSqlDatabaseList(connect).Contains(Path.GetFileNameWithoutExtension(TestName)));
        }
        
        [TestMethod]
        public void LocalDB_CreateDatabaseOk()
        {
            Assert.IsNotNull(Demo);
            var connect = Demo.GetSqlConnect();

            //if (!OdinSearch_OutputConsumerSql_CommandHandler.CreateSqlDatabase(connect, "C:\\Dummy\\Test4.Sea"))
            {
              //  Assert.Fail("Failed to create databaseok");
            }

            //if (!OdinSearch_OutputConsumerSql_CommandHandler.GetSqlDatabaseList(connect).Contains("Test4"))
            {
             //   Assert.Fail("Failed to insert DB ok");
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using Microsoft.VisualBasic;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools
{
    /// <summary>
    /// Used for running the sql commands.
    /// </summary>
    public static class OdinSearchSqlActions
    {
     
        /// <summary>
        /// Test if valid sql identifier
        /// </summary>
        /// <param name="name"></param>
        /// <remarks>Calls internal routine <see cref="OdinSearchSqlPreFabs.AssertSqlIdentifier(string)"/></remarks>
        /// <exception cref="ArgumentException">Is thrown if test fails</exception>
        /// <exception cref="ArgumentNullException">Is thrown if argument is null</exception>
        public static void NameCheck(string name)
        {
            OdinSearchSqlPreFabs.AssertSqlIdentifier(name);
              
        }

        public static void DeleteTable(SqlConnection sql, string tableName)
        {
            SqlCommand cmd = new SqlCommand(OdinSearchSqlPreFabs.BuildDeleteTableString(tableName, true), sql);
            cmd.ExecuteNonQuery();
        }
        
        /// <summary>
        /// Get list of tables from the current database
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string[] GetTableList(SqlConnection sql)
        {
            List<string> tableList = new List<string>();

            SqlCommand cmd  = new SqlCommand(OdinSearchSqlPreFabs.BuildGetUserTableList(), sql);

            using (var Reader = cmd.ExecuteReader())
            {
                while (Reader.Read()) { 
                    tableList.Add(Reader.GetString(0));
                }
            }
            return tableList.ToArray();
        }

        public static bool SelectDatabase(SqlConnection sql, string name)
        {
            OdinSearchSqlPreFabs.AssertSqlIdentifier(name);
            SqlCommand cmd = new SqlCommand(OdinSearchSqlPreFabs.BuildUseDatabase(name) , sql);
            cmd.ExecuteNonQuery();
            return true;
        }
        /// <summary>
        /// Loop thru the properties of the passed type and return SQL that describes the class
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <remarks>If there's no defined way to convert <see cref="OdinSqlPreFab_TypeGen"/>, this will throw <see cref="NotImplementedException()"/> with the offending type if the default settings don't have an answer</remarks>
        public static string ConvertClassToSqlRecord(Type t)
        {
            return OdinSearchSqlPreFabs.ClassToSQLRecord(t);
        }

        /// <summary>
        /// Create a table in the current database based on the specified tpye
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="tableName"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <exception cref="NameCheckException"></exception>
        /// <exception cref="NotImplementedException">Can be thrown due to calling <see cref="ConvertClassToSqlRecord(Type)"/></exception>
        public static bool CreateTableFromType(SqlConnection sql, string tableName, Type t)
        {
            NameCheck(tableName);
            StringBuilder use = new StringBuilder(string.Format("CREATE TABLE {0} ({1});",tableName, ConvertClassToSqlRecord(t)));

            SqlCommand sqlCommand = new SqlCommand(use.ToString(), sql);

            sqlCommand.ExecuteNonQuery();
            
            return true;

        }

        

        public static string[] GetSqlDatabaseList(SqlConnection sql)
        {
            List<string> dbnames = new List<string>();

            SqlDataReader results;
            SqlCommand cmd = new SqlCommand(OdinSearchSqlPreFabs.BuildGetDatabaseList(), sql);

            using (results = cmd.ExecuteReader())
            {
                while (results.Read())
                {
                    dbnames.Add(results.GetString(0));

                }
            }

            return dbnames.ToArray();
        }

        public static bool CreateDataBaseIfNotExisting(SqlConnection sql, string name)
        {
            string[] list = GetSqlDatabaseList(sql);
            if (list.Contains(Path.GetFileNameWithoutExtension(name)) == false)
            {
                return CreateSqlDatabase(sql, name);
            }
            return false;
        }

        /// <summary>
        /// Delete the passed Database from the system. You may need to delete the file aftwards
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool DeleteSqlDataBase(SqlConnection sql, string name)
        {
            string cmd = OdinSearchSqlPreFabs.BuildDeleteDatabaseString(Path.GetFileNameWithoutExtension(name), false) ;
            SqlCommand command = new SqlCommand(cmd, sql);

            command.ExecuteNonQuery();
            return true;
        }
        /// <summary>
        /// Create database for the connect
        /// </summary>
        /// <param name="sql">connection to use</param>
        /// <param name="name">Name of the database.  Must pass <see cref="OdinSearchSqlPreFabs.AssertSqlIdentifier(string)"/></param>
        /// <returns></returns>
        public static bool CreateSqlDatabase(SqlConnection sql, string name)
        {
            //string cmd = OdinSqlPreFabs.CreateDatabaseString(Path.GetFileNameWithoutExtension(name), OdinSqlPreFabs.DatabaseContainment.Discard, name, true, Path.GetDirectoryName(name));
            string cmd2 = OdinSearchSqlPreFabs.BuildCreateDatabaseString(Path.GetFileNameWithoutExtension(name), OdinSearchSqlPreFabs.DatabaseContainment.Discard, name, null, null, null, null);
            SqlCommand command = new SqlCommand(cmd2, sql);


            
            command.ExecuteNonQuery();
            return true;
        }
    }

    /// <summary>
    /// Prepresents a table created in the sql databasse to hold the info
    /// </summary>
    internal class SearchResultsSqlTable
    {
        public DateTime SearchExecuted;
        public string HostComputerName;
        public string HostComputerIp;
        public Int128 Matches;
        public Int128 Blocked;
    }
    /// <summary>
    ///  Takes search results and outputs them to an sql db to look over later.
    /// </summary>
    public class OdinSearch_OutputConsumerSql : OdinSearch_OutputConsumerBase
    {
        
        private bool PopulateNewSearchDatabase()
        {
           if(! OdinSearchSqlActions.CreateTableFromType(connection, "SearchResults", typeof(SearchResultsSqlTable)))
            {
                return false;
            }
            if (!OdinSearchSqlActions.CreateTableFromType(connection, "MatchResults", typeof(FileSystemInfo)))
            {
                return false;
            }
            return true;
        }
    
        
        private bool CreateOrSelectDatabase(string location)
        {
            return OdinSearchSqlActions.CreateDataBaseIfNotExisting(connection, location);
        }
        
        private string BuildConnectionString(string StorageLoc)
        {/*
            string ret = null;
            --Next, create the database and specify the file path
SqlCommand command = new SqlCommand("CREATE DATABASE MyDatabase ON PRIMARY " +
    "(NAME = MyDatabase_Data, " +
    "FILENAME = 'C:\\Path\\To\\MyDatabase.mdf', " +
    "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
    "LOG ON (NAME = MyDatabase_Log, " +
    "FILENAME = 'C:\\Path\\To\\MyDatabase.ldf', " +
    "SIZE = 1MB, " +
    "MAXSIZE = 5MB, " +
    "FILEGROWTH = 10%)", connection);

            --Execute the command to create the database
connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            */
            var useme = new SqlConnectionStringBuilder();
            {
                useme.ApplicationIntent = ApplicationIntent.ReadWrite;
                useme.DataSource = @"(localdb)\MSSQLLocalDB";
                useme.IntegratedSecurity = true;

                useme.ApplicationName = "OdinSearch UnitTest";
                useme.AttachDBFilename = StorageLoc;
                useme.Encrypt = false;
                return useme.ToString();
                //ret = "Server=(LocalDB)\\MSSQLLocalDB;Integrated Security=true;AttachDBFileName=\"" + StorageLoc+ "\"" + ";database=\"TestDB\"";
                //ret = @"Server = (localdb)\MSSQLLocalDB; Integrated Security = true;AttachDbFilename=|DataDirectory|" + "\"" + StorageLoc+ "\"";
            }
            //return ret;
        }


        public OdinSearch_OutputConsumerSql(SqlConnection localConnectionSql)
        {
            connection = localConnectionSql;
            localConnectionSql.Open();
        }

        public OdinSearch_OutputConsumerSql(SqlConnectionStringBuilder Connect)
        {
            connection = new SqlConnection(Connect.ToString());
            connection.Open();
        }
        public OdinSearch_OutputConsumerSql(string SqlConnectionString)
        {
            connection = new SqlConnection();
            connection.ConnectionString = BuildConnectionString(SqlConnectionString);
            connection.Open();
        }

        
        public OdinSearch_OutputConsumerSql(SqlConnection Connection, string StorageLocation)
        {
            connection = Connection;
            connection.Open();

            OdinSearchSqlActions.CreateSqlDatabase(connection, StorageLocation);
        }
        public OdinSearch_OutputConsumerSql(string ConnectionString, string StorageLocation)
        {
            connection = new SqlConnection();
            connection.ConnectionString = BuildConnectionString(string.Empty);
            if (ConnectionString == string.Empty)
            {
                
                
            }
            else
            {

            }
            connection.Open();
            if (!OdinSearchSqlActions.CreateDataBaseIfNotExisting(connection, StorageLocation))
            {
                OdinSearchSqlActions.SelectDatabase(connection, StorageLocation);
            }
        }


        SqlConnection connection;

        [Obsolete("Intented only for debug. Runtime/no debug should use the wrappers once defined")]
        public SqlConnection GetSqlConnect()
        {
            return connection;
        }

        public override void Dispose()
        {
            connection.Dispose();
        }
        public override void Blocked(string Blocked)
        {
            
        }
        public override void Match(FileSystemInfo info)
        {
            
        }

        public override void WasNotMatched(FileSystemInfo info)
        {

        }

        public override void Messaging(string Message)
        {
            
        }
    }
}

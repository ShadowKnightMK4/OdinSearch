using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools
{
    /// <summary>
    /// Used for running the sql commands
    /// </summary>
    public static class OdinSearch_OutputConsumerSql_CommandHandler
    {
        /// <summary>
        /// This is thrown by the routines in <see cref="OdinSearch_OutputConsumerSql_CommandHandler"/> when namecheck fails
        /// </summary>
        public class NameCheckException : FormatException
        {
            public const string DefaultNameMessage = "The identifer is invalid:  ";
            public NameCheckException(string message) : base(message)
            {

            }
        }
        /// <summary>
        /// This is used to check if a named item in sql (database, table, ect..) is legal before sending it to the system.
        /// </summary>
        /// <param name="name"></param>
        /// <see href="https://dev.mysql.com/doc/refman/8.0/en/identifiers.html"/>
        /// <see href="https://stackoverflow.com/questions/925696/mysql-create-database-with-special-characters-in-the-name"/>
        /// <returns></returns>
        public static bool NameCheck(string name)
        {
            var BadChar = Path.GetInvalidFileNameChars();
            if (name == null) return false;
            if (name.Contains((char)0)) return false;

            if (name.Length == 0)
            {
                return false;
            }

            if (name.StartsWith(' ') || name.EndsWith(' ')) return false;
            if (name.StartsWith('\t') || name.EndsWith('\t')) return false;
            if (name.StartsWith('\r') || name.EndsWith('\r')) return false;
            if (name.StartsWith('\n') || name.EndsWith('\n')) return false;

            if (name.Contains('\''))
                return false;
            if (name.Contains('\"'))
                return false;
            foreach (char c in name)
            {
                if (BadChar.Contains(c)) return false;
            }

            return true;
        }

        public static string[] GetSqlDatabaseList(SqlConnection sql)
        {
            List<string> dbnames = new List<string>();

            SqlDataReader results;
            SqlCommand cmd = new SqlCommand("SELECT name, database_id, create_date FROM sys.databes", sql);

            using (results = cmd.ExecuteReader())
            {
                while (results.Read())
                {

                }
            }

            return dbnames.ToArray();
        }
        /// <summary>
        /// Create database for the connect
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static bool CreateSqlDatabase(SqlConnection sql, string name)
        {
            SqlCommand command = new SqlCommand("CREATE DATABASE ", sql);

            if (!NameCheck(name))
            {
                throw new NameCheckException(NameCheckException.DefaultNameMessage + name);
            }


            var arg = command.CreateParameter();
            arg.ParameterName = "name";
            arg.Value = name;
            arg.DbType = System.Data.DbType.String;


            command.Parameters.Add(arg);
            command.CommandText += arg.Value;
            command.ExecuteNonQuery();
            return true;
        }
    }
    /// <summary>
    ///  Takes search results and outputs them to an sql db to look over later.
    /// </summary>
    public class OdinSearch_OutputConsumerSql : OdinSearch_OutputConsumerBase
    {
        private string BuildStorageLoc(string StorageLoc)
        {
            string ret = null;

            var useme = new SqlConnectionStringBuilder();
            {
                useme.ApplicationIntent = ApplicationIntent.ReadWrite;
                useme.DataSource = @"(localDb)\MSSQLLocalDB";
                useme.IntegratedSecurity = true;

                useme.ApplicationName = "OdinSearch UnitTest";
                useme.AttachDBFilename = StorageLoc;
                useme.Encrypt = false;
                ret = useme.ToString();
                //ret = "Server=(LocalDB)\\MSSQLLocalDB;Integrated Security=true;AttachDBFileName=\"" + StorageLoc+ "\"" + ";database=\"TestDB\"";
                //ret = @"Server = (localdb)\MSSQLLocalDB; Integrated Security = true;AttachDbFilename=|DataDirectory|" + "\"" + StorageLoc+ "\"";
            }
            return ret;
        }


        public OdinSearch_OutputConsumerSql(SqlConnectionStringBuilder Connect)
        {
            LocalConnectionSql = new SqlConnection(Connect.ToString());
            LocalConnectionSql.Open();
        }
        public OdinSearch_OutputConsumerSql(string StorageLoc)
        {
            LocalConnectionSql = new SqlConnection();
            LocalConnectionSql.ConnectionString = BuildStorageLoc(StorageLoc);
            LocalConnectionSql.Open();


        }


        SqlConnection LocalConnectionSql;

        [Obsolete("Intented only for debug. Runtime/no debug should use the wrappers once defined")]
        public SqlConnection GetSqlConnect()
        {
            return LocalConnectionSql;
        }

        public override void Dispose()
        {
            LocalConnectionSql.Dispose();
        }
        public override void Blocked(string Blocked)
        {
            throw new NotImplementedException();
        }
        public override void Match(FileSystemInfo info)
        {
            throw new NotImplementedException();
        }

        public override void WasNotMatched(FileSystemInfo info)
        {

        }

        public override void Messaging(string Message)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Net.Http.Headers;

namespace OdinSearchEngine
{
    /// <summary>
    /// Takes search results and outputs them to an sql db to look over later.
    /// </summary>
    public class OdinSearch_OutputConsumerSql: OdinSearch_OutputConsumerBase
    {
        private string BuildStorageLoc(string StorageLoc)
        {
            string ret=null;

            var useme = new SqlConnectionStringBuilder();
            {
                useme.ApplicationIntent = ApplicationIntent.ReadWrite;
                useme.DataSource = @"(localdb)\MSSQLLocalDB";
                                useme.IntegratedSecurity = true;
                useme.InitialCatalog = "Test Database";
                useme.ApplicationName = "OdinSearch UnitTest";
                useme.AttachDBFilename = StorageLoc;
                useme.Encrypt = false;
                ret = useme.ToString();
                ret = @"Server=(LocalDB)\MSSQLLocalDB;Integrated Security=true";
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

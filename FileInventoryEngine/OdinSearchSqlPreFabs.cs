using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine
{   
    
    /// <summary>
    /// Contains the Sql stuff that the engine uses to deal sql databases.
    /// </summary>
    internal static class OdinSearchSqlPreFabs
    {
        public static string CSToSqlType(PropertyInfo t)
        {
            return CSToSqlType(t.PropertyType);
        }
        public static string CSToSqlType(Type t)
        {
            if (t ==typeof(string))
            {
                return "NVARCHAR(MAX)";
            }
            if (t == typeof(DateTime))
            {
                return "DATETIME";
            }
            
            if (t == typeof(long))
            {
                return "BIGINT";
            }

            if (t == typeof(byte))
            {
                if (t.IsArray)
                    return "BINARY";
                else
                    return "VARBINARY(MAX)";
            }

            if (t == typeof(Byte[]))
            {
                return "VARBINARY(MAX)";
            }
            
            if (t == typeof(bool))
            {
                return "BIT";
            }

            if (t == typeof(DirectoryInfo))
            {
                return "NVARCHAR(MAX)";
            }

            if (t == typeof(FileAttributes))
            {
                return "INT";
            }

            throw new NotImplementedException("No case for the type of " + t.Name + "in the CSToSqlType");
        }
        /// <summary>
        /// Output a SQL statement to create records of the public properties for the class
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string  ClassToSQLRecord(Type t)
        {
            
            StringBuilder ret = new StringBuilder();
            PropertyInfo[] Info = t.GetProperties();
            for (int step = 0; step < Info.Length;step++)
            {
               var special_convert = Info[step].GetCustomAttribute<OdinSqlPreFab_TypeGen>();
                var SkipThis = Info[step].GetCustomAttribute<OdinSearchSqlSkipAttrib>();
                
                if (SkipThis != null) continue;

                

                if (special_convert != null)
                {
                    

                    if (special_convert.OverrideName!= null) 
                    {
                        AssertSqlIdentifier(special_convert.OverrideName);
                        ret.Append(string.Format("{0} ", special_convert.OverrideName));
                    }
                    else
                    {
                        AssertSqlIdentifier(Info[step].Name);
                        ret.Append(string.Format("{0} ", Info[step].Name));
                    }

                    ret.Append(string.Format("{0} ", special_convert.BaseType));

                    if (special_convert.IdentitySet)
                    {
                        ret.Append(string.Format("IDENTITY({0},{1}) ", special_convert.IdentityStart, special_convert.IdentityTickUp));
                    }
                    if (special_convert.IsPrimary)
                    {
                        ret.Append("PRIMARY ");
                    }
                    if (special_convert.NotNull)
                    {
                        ret.Append("NOT NULL");
                    }
                    
                }
                else
                {
                    ret.Append(string.Format("{0} {1} ", Info[step].Name, CSToSqlType(Info[step])));
                    
                    
                }
                if ((step != Info.Length-1))
                {
                    ret.Append(",\r\n");
                }
                else
                {
                    ret.Append("\r\n");
                }
            }
            return ret.ToString();
        }
        
        
        static string[] sqlReservedWords = new string[] {
    "ADD", "EXTERNAL", "PROCEDURE", "ALL", "FETCH", "PUBLIC", "ALTER", "FILE", "RAISERROR",
    "AND", "FILLFACTOR", "READ", "ANY", "FOR", "READTEXT", "AS", "FOREIGN", "RECONFIGURE",
    "ASC", "FREETEXT", "REFERENCES", "AUTHORIZATION", "FREETEXTTABLE", "REPLICATION", "BACKUP",
    "FROM", "RESTORE", "BEGIN", "FULL", "RESTRICT", "BETWEEN", "FUNCTION", "RETURN", "BREAK",
    "GOTO", "REVERT", "BROWSE", "GRANT", "REVOKE", "BULK", "GROUP", "RIGHT", "BY", "HAVING",
    "ROLLBACK", "CASCADE", "HOLDLOCK", "ROWCOUNT", "CASE", "IDENTITY", "ROWGUIDCOL", "CHECK",
    "IDENTITY_INSERT", "RULE", "CHECKPOINT", "IDENTITYCOL", "SAVE", "CLOSE", "IF", "SCHEMA",
    "CLUSTERED", "IN", "SECURITYAUDIT", "COALESCE", "INDEX", "SELECT", "COLLATE", "INNER",
    "SEMANTICKEYPHRASETABLE", "COLUMN", "INSERT", "SEMANTICSIMILARITYDETAILSTABLE", "COMMIT",
    "INTERSECT", "SEMANTICSIMILARITYTABLE", "COMPUTE", "INTO", "SESSION_USER", "CONSTRAINT",
    "IS", "SET", "CONTAINS", "JOIN", "SETUSER", "CONTAINSTABLE", "KEY", "SHUTDOWN", "CONTINUE",
    "KILL", "SOME", "CONVERT", "LEFT", "STATISTICS", "CREATE", "LIKE", "SYSTEM_USER", "CROSS",
    "LINENO", "TABLE", "CURRENT", "LOAD", "TABLESAMPLE", "CURRENT_DATE", "MERGE", "TEXTSIZE",
    "CURRENT_TIME", "NATIONAL", "THEN", "CURRENT_TIMESTAMP", "NOCHECK", "TO", "CURRENT_USER",
    "NONCLUSTERED", "TOP", "CURSOR", "NOT", "TRAN", "DATABASE", "NULL", "TRANSACTION", "DBCC",
    "NULLIF", "TRIGGER", "DEALLOCATE", "OF", "TRUNCATE", "DECLARE", "OFF", "TRY_CONVERT",
    "DEFAULT", "OFFSETS", "TSEQUAL", "DELETE", "ON", "UNION", "DENY", "OPEN", "UNIQUE",
    "DESC", "OPENDATASOURCE", "UNPIVOT", "DISK", "OPENQUERY", "UPDATE", "DISTINCT", "OPENROWSET",
    "UPDATETEXT", "DISTRIBUTED", "OPENXML", "USE", "DOUBLE", "OPTION", "USER", "DROP", "OR",
    "VALUES", "DUMP", "ORDER", "VARYING", "ELSE", "OUTER", "VIEW", "END", "OVER", "WAITFOR",
    "ERRLVL", "PERCENT", "WHEN", "ESCAPE", "PIVOT", "WHERE", "EXCEPT", "PLAN", "WHILE", "EXEC",
    "PRECISION", "WITH", "EXECUTE", "PRIMARY", "WITHIN GROUP", "EXISTS", "PRINT", "WRITETEXT", "EXIT",
    "PROC" };

        /// <summary>
        /// If we can create the file or open it, it's likely a valid file name
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string paranoid_filename(string filename)
        {
            bool AlreadyExists = false;
            try
            {
                using (var OpenMe = File.Open(filename, FileMode.Open))
                {
                    AlreadyExists = true;
                }
            }
            catch (FileNotFoundException)
            {
                AlreadyExists= false;
            }

            if (AlreadyExists)
            {
                return filename;
            }
            else
            {
                try
                {
                    using (var CreateMe = File.Open(filename, FileMode.CreateNew))
                    {
                        AlreadyExists = true;
                    }
                    
                }
                catch (IOException)
                {
                    throw;
                }
                if (AlreadyExists)
                {
                    File.Delete(filename); return filename;
                }
            }

            Debug.WriteLine("Ensure Parinoid File actual has code to indicate valid filename");
            return filename;
        }
        /// <summary>
        /// Returns if the name is is the list of reserved sql words as given by microsoft and chat gpt.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool isReservedSqlWord(string name)
        {
            name = name.ToUpperInvariant();
            return sqlReservedWords.Contains(name);
        }

        /// <summary>
        /// Is the ID not a reserved word and fit Sql identifier rules?
        /// </summary>
        /// <param name="Id">string to test</param>
        /// <exception cref="ArgumentNullException">Thrown if Id is null</exception>
        /// <exception cref="ArgumentException">Thrown if If FAILS test</exception>
        public static void AssertSqlIdentifier(string Id)
        {
            if (string.IsNullOrEmpty(Id)) { throw new ArgumentNullException("id"); }
            if (!((char.IsLetter(Id[0])) || ("_#@".Contains(Id[0]) == true)))
            {
                throw new ArgumentException("Name for Table/record/database failed general check \"" + Id + "\"");
            }


            if (isReservedSqlWord(Id))
            {
                throw new ArgumentException("Name for Table/record/database is a reserved sql word \"" + Id + "\"");
            }

            for (int step = 1; step < Id.Length; step++) 
            { 
                if (!char.IsLetter(Id[step]))
                {
                    if (!char.IsNumber(Id[step]))
                    {
                        if (!"@$_".Contains(Id[step]))
                        {
                            throw new ArgumentException(nameof(Id));
                        }
                    }
                }
            }

        }

        public enum DatabaseContainment
        {
            /// <summary>
            /// No containment
            /// </summary>
            None = 0,
            /// <summary>
            /// Partial Containment
            /// </summary>
            Partial = 1,
            /// <summary>
            /// Do not add a contaiment statement
            /// </summary>
            Discard = 2
        }


        /// <summary>
        /// used to emit file locatation to the string builder. Adds '\'' quotes
        /// </summary>
        /// <param name="Output"></param>
        /// <param name="first"></param>
        /// <param name="rest"></param>
        static void BuildFileList(StringBuilder Output, string first, List<string> rest)
        {
            
            if (string.IsNullOrEmpty(first) == false)
            {
                Output.Append(string.Format("\'{0}\' ", first));
            }
            if ((rest != null) && (rest.Count > 0))
            {
                Output.Append(", ");
                for (int step =0; step < rest.Count; step++)
                {
                    Output.Append(string.Format("\'{0}\'", rest[step]));
                    if (step != rest.Count-1)
                    {
                        Output.Append(",");
                    }
                }
            }
        }


        /// <summary>
        /// Return a string to get list of databases in the connection
        /// </summary>
        /// <returns></returns>
        public static string BuildGetDatabaseList()
        {
            return "SELECT name FROM sys.databases";
        }
        public static string BuildCreateTable(string Database, string name)
        {
            throw new NotImplementedException();
        }

        

        /// <summary>
        /// Get list of tables current database
        /// </summary>
        /// <returns></returns>
        public static string BuildGetUserTableList()
        {
            return "SELECT name FROM sys.tables";
        }

        /// <summary>
        /// return a string to tele a table
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="IfExists"></param>
        /// <returns></returns>
        public static string BuildDeleteTableString(string TableName, bool IfExists)
        {
            return BuildDeleteDatabaseOrTableString(false, TableName, IfExists);
        }


        /// <summary>
        /// return a string to delete a database 
        /// </summary>
        /// <param name="DatabaseName"></param>
        /// <param name="IfExists"></param>
        /// <returns></returns>
        public static string BuildDeleteDatabaseString(string DatabaseName, bool IfExists)
        {
            return BuildDeleteDatabaseOrTableString(true, DatabaseName, IfExists);
        }

        /// <summary>
        /// return a string to delete a database or table
        /// </summary>
        /// <param name="UseDataBase">True for Database, false for table</param>
        /// <param name="DatabaseName"></param>
        /// <param name="IfExists"></param>
        /// <returns></returns>
        static string BuildDeleteDatabaseOrTableString(bool UseDataBase, string DatabaseName, bool IfExists)
        {
            string DataBase;
            string Exists = string.Empty;
            if (IfExists)
            {
                Exists = "IF EXISTS";
            }
            if (UseDataBase)
            {
                DataBase = "DATABASE";
            }
            else
            {
                DataBase = "TABLE";
            }


            StringBuilder ret = new StringBuilder();
            ret.Append(string.Format("DROP {0} {1} {2}; ", DataBase, Exists, DatabaseName));

            return ret.ToString();

        }

        /// <summary>
        /// return a string to create a database
        /// </summary>
        /// <param name="Databasename">name for database</param>
        /// <param name="Contain">containment rules</param>
        /// <param name="PrimaryFile">Primary file</param>
        /// <param name="AdditionalFiles">Additiobnal file</param>
        /// <param name="PrimayLogLocation">first log</param>
        /// <param name="AdditionalLogLocation">extra log</param>
        /// <param name="CollateSpecs"></param>
        /// <returns></returns>
        public static string BuildCreateDatabaseString(string Databasename,
            DatabaseContainment Contain,
            string PrimaryFile,
            List<string> AdditionalFiles, 
            string PrimayLogLocation,
            List<string> AdditionalLogLocation,
            string CollateSpecs)
        {
            
            StringBuilder FancyArgs= new StringBuilder("(");
            StringBuilder ret = new StringBuilder();


            ret.Append(string.Format("CREATE DATABASE {0}  ", Databasename));
            FancyArgs.Append(string.Format("NAME={0}", Databasename));

            switch (Contain)
            {
                case DatabaseContainment.Partial:
                    FancyArgs.Append(",CONTAINTMENT=PARTIAL,");
                    break;
                case DatabaseContainment.None:
                    FancyArgs.Append(",CONTAINTMENT=NONE,");
                    break;
            }
            
            if (FancyArgs[FancyArgs.Length-1] != ',')
            {
                FancyArgs.Append(",");
            }

            if ( (PrimaryFile!= null)  || (AdditionalFiles!= null) )
            {
                FancyArgs.Append("ON ");
               if (PrimaryFile != null)
                    FancyArgs.Append("PRIMARY ");
                BuildFileList(FancyArgs, PrimaryFile, AdditionalFiles);
            }


            if (FancyArgs[FancyArgs.Length-1] != ',')
            {
                FancyArgs.Append(",");
            }
            if ( (PrimayLogLocation!= null) || (AdditionalLogLocation != null))
            {
                FancyArgs.Append("LOG ON ");
                BuildFileList(FancyArgs, PrimayLogLocation, AdditionalLogLocation);
            }

            if (CollateSpecs != null)
            {
                FancyArgs.Append(string.Format(" COLLATE {0} ", CollateSpecs));
            }
            return ret.ToString();
        }

        [Obsolete("Use BuildCreateDatabaseString() instead. This will still work but does not support the extra stuff")]
                                                
        /// <summary>
        /// Construct a Create Database string with specifed stuff
        /// </summary>
        /// <param name="databaseName">Name of the database. Must pass <see cref="AssertSqlIdentifier(string)"/></param>
        /// <param name="Containment">How to contain the database</param>
        /// <param name="DataBaseLocation">Location to store database</param>
        /// <param name="IsPrimary"></param>
        /// <param name="LogLocation"></param>
        /// <returns></returns>
        public static string CreateDatabaseString(string databaseName, DatabaseContainment Containment, string DataBaseLocation, bool IsPrimary, string LogLocation)
        {
            StringBuilder ret = new StringBuilder();
            // check if arguments are good.
            AssertSqlIdentifier(databaseName);

            

            // now we build
            ret.Append(string.Format("CREATE DATABASE {0} ON PRIMARY (", databaseName));
            ret.Append(string.Format("NAME={0},", databaseName));
            switch (Containment)
            {
                case DatabaseContainment.Partial:
                    ret.Append("CONTAINTMENT=PARTIAL,");
                    break;
                case DatabaseContainment.None:
                    ret.Append("CONTAINTMENT=NONE,");
                    break;
            }

            ret.Append(string.Format("FILENAME=\'{0}\'); ", paranoid_filename(DataBaseLocation)));



            return ret.ToString();
        }

        public static string BuildUseDatabase(string name)
        {
            AssertSqlIdentifier(name);
            return string.Format("USE {0};", name);
        }
    }
}

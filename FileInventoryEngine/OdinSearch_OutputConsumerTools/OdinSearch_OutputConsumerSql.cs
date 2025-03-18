using System;
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
    /// <remarks>For the release which will include the source in the build, this is removed due to source not using it.</remarks>
    public static class OdinSearchSqlActions
    {

    }

    /// <summary>
    /// Placeholder for when this feature is added. Currently will throw not implemented
    /// </summary>
    /// <exception cref="NotImplementedException">Is thrown on instancing.</exception>
    public class OdinSearch_OutputConsumerSql : OdinSearch_OutputConsumerBase
    {
        public OdinSearch_OutputConsumerSql()
        {
            throw new NotImplementedException();
        }
        
    }

}

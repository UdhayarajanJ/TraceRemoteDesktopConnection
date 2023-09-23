using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceRemoteAccessingServer
{
    public static class DapperContext
    {
        private static string _connectionString = string.Empty;
        static DapperContext()
        {
            _connectionString = ConfigurationManager.AppSettings["SqlConnection"].ToString();
        }

        public static DbConnection CreateConnection()
        {
            return new System.Data.SqlClient.SqlConnection(_connectionString);
        }
    }
}

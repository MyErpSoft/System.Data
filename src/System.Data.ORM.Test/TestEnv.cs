using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ORM.Test {
    class TestEnv {
        private const string TestDbConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=C:\DOCUMENTS\GITHUB\SYSTEM.DATA\SRC\SYSTEM.DATA.ORM.TEST\TESTDB.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public static SqlConnection GetSqlConnection() {
            SqlConnection cn = new SqlConnection(TestDbConnectionString);
            cn.Open();
            return cn;
        }
    }
}

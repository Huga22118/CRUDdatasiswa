using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace WindowsFormsApp1
{
    internal class DatabaseConnect
    {
        

        public SqlConnection sqlconn()
        {
            SqlConnection sqlcon = null;
            sqlcon = new SqlConnection("Data Source=localhost;Initial Catalog=TableTep;Integrated Security=True");
            return sqlcon;
        }

    }
}

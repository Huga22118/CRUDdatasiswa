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
#if DEBUG
            string loc = "D:\\CRUDdatasiswa\\WindowsFormsApp1\\WindowsFormsApp1\\localData.mdf"; 
            /*
             
            forced to hardcode the path because i got invalid object name error. Feel free to change it with your localData.mdf from the cloned project repo (not on the published .mdf).
            I use this localData.mdf to monitor each table because i'm unable to open the local database file from the published app to monitor those tables
             - Depressed Huga looking through many ways to solve this but he didn't solve any shit :(
             */
#else
            string loc = "|DataDirectory|\\localData.mdf";
#endif
            SqlConnection sqlcon = null;
            sqlcon = new SqlConnection($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={loc};Integrated Security=True");
            return sqlcon;
        }

    }
}

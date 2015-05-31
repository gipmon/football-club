using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace FootballClub
{
    class ConnectionDB
    {
        private static SqlConnection con;
        static ConnectionDB()
        {
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            con = new SqlConnection(ConString);
        }
        public static SqlConnection getConnection()
        {
            return con;
        }
    }
}

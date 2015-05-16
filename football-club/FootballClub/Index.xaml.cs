using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FootballClub
{
    /// <summary>
    /// Interaction logic for Index.xaml
    /// </summary>
    public partial class Index : Page
    {
        private string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
        private SqlConnection con;

        public Index()
        {
            InitializeComponent();
            using (con = new SqlConnection(ConString))
            {
                FillStats(con);
            }
        }
        
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *  ##########################-----------     STATS    -----------##########################
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void FillStats(SqlConnection con)
        {
            string CmdString = "SELECT * FROM football.udf_general_stats()";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("stats");
            sda.Fill(dt);

            foreach (DataRow counts in dt.Rows)
            {
                if (counts["name"].ToString() == "total_of_players"){
                    total_of_players.Text = counts["count"].ToString();
                }
                else if(counts["name"].ToString() == "total_of_staff")
                {
                    total_of_staff.Text = counts["count"].ToString();
                }
                else if(counts["name"].ToString() == "total_internal_people")
                {
                    total_internal_people.Text = counts["count"].ToString();
                }
                else if(counts["name"].ToString() == "total_coachs")
                {
                    total_coachs.Text = counts["count"].ToString();
                }
                else if(counts["name"].ToString() == "total_coachs")
                {
                    total_coachs.Text = counts["count"].ToString();
                }
                else if (counts["name"].ToString() == "total_club_members")
                {
                    total_club_members.Text = counts["count"].ToString();
                }
                else if (counts["name"].ToString() == "total_salaries_per_month")
                {
                    total_salaries_per_month.Text = counts["count"].ToString();
                }
                else if (counts["name"].ToString() == "total_of_seats")
                {
                    total_of_seats.Text = counts["count"].ToString();
                }
                else if (counts["name"].ToString() == "total_of_teams")
                {
                    total_of_teams.Text = counts["count"].ToString();
                }
            }
        }
    }
}

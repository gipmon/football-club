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
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class Player : Page
    {
        private string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
        private SqlConnection con;

        public Player()
        {
            InitializeComponent();
            using (con = new SqlConnection(ConString))
            {
                FillDataGrid();
            }
        }
        private void FillDataGrid()
        {
            // é importante definirmos uma ordem nas queries e começar por definir views para isto tudo...
            string CmdString = "SELECT * FROM football.playersView";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("players");
            sda.Fill(dt);
            playersGrid.ItemsSource = dt.DefaultView;
        }

        private void playersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)playersGrid.SelectedItem;
                string search_bi = row.Row.ItemArray[1].ToString();
                bi.Text = search_bi;
                string CmdString = "SELECT * FROM football.playerView WHERE bi=" + search_bi;
                SqlCommand cmd = new SqlCommand(CmdString, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("player");
                sda.Fill(dt);
                DataRow r = dt.Rows[0];
                
                bi.Text = r["bi"].ToString();
                name.Text = r["name"].ToString();
                nif.Text = r["nif"].ToString();
                address.Text = r["address"].ToString();
                // gender.Text = row.Row.ItemArray[4].ToString();
                birth_date.Text = r["birth date"].ToString();
                nationality.Text = r["nationality"].ToString();
                // federation_id.Text = row.Row.ItemArray[7].ToString();
                weight.Text = r["weight"].ToString();
                height.Text = r["height"].ToString();
            }
        }

    }
}

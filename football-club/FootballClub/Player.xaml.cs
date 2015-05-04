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
        public Player()
        {
            InitializeComponent();
            FillDataGrid();
        }
        private void FillDataGrid()
        {
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(ConString))
            {
                // é importante definirmos uma ordem nas queries e começar por definir views para isto tudo...
                string CmdString = "SELECT * FROM (football.player JOIN football.person ON player.bi=person.bi)";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("player");
                sda.Fill(dt);
                playersGrid.ItemsSource = dt.DefaultView;
            }
        }

        private void playersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView row = (DataRowView)playersGrid.SelectedItem;
            try
            {
                bi.Text = row.Row.ItemArray[0].ToString();
            }
            catch (Exception)
            {

            }
            // a minha sugestao e obter os dados apartir da datagrid sempre que forem clicados e colocar no lado direito
        }

    }
}

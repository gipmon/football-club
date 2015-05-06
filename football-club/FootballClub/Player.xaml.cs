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

            // fill the teams of the player
            CmdString = "SELECT * FROM football.teamNamesView";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("teams");
            sda.Fill(dt);
            foreach (DataRow team in dt.Rows)
            {
                ListBoxItem itm = new ListBoxItem();
                itm.Content = team["name"].ToString();
                teams.Items.Add(itm);
            }
        }

        private void playersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)playersGrid.SelectedItem;
                string search_bi = row.Row.ItemArray[1].ToString();
                bi.Text = search_bi;

                string CmdString = "SELECT * FROM football.playerView WHERE bi=@intBi";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                cmd.Parameters.AddWithValue("@intBi", Convert.ToInt32(search_bi));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("player");
                sda.Fill(dt);
                DataRow r = dt.Rows[0];
                
                bi.Text = r["bi"].ToString();
                name.Text = r["name"].ToString();
                nif.Text = r["nif"].ToString();
                address.Text = r["address"].ToString();

                if (r["gender"].ToString() == "F")
                {
                    GenderFemale.IsChecked = true;
                }
                else
                {
                    GenderMale.IsChecked = true;
                }

                DateTime date = DateTime.Parse(r["birth date"].ToString());
                birth_date.Text = date.ToString("yyyy-MM-dd");
                nationality.Text = r["nationality"].ToString();
                federation_id.Text = r["federation id"].ToString();
                salary.Value = Convert.ToDouble(r["salary"].ToString());
                internal_id.Text = r["internal id"].ToString();
                weight.Text = r["weight"].ToString();
                height.Text = r["height"].ToString();

                // select the teams of the player
                CmdString = "SELECT * FROM football.playersTeamsView WHERE bi=" + search_bi;
                cmd = new SqlCommand(CmdString, con);
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable("teams_selected");
                sda.Fill(dt);

                foreach (ListBoxItem itm in teams.Items)
                {
                    itm.IsSelected = false;
                    foreach (DataRow team in dt.Rows)
                    {
                        if(team["team_name"].ToString() == itm.Content.ToString()){
                            itm.IsSelected = true;
                            break;
                        }
                    }
                }
            }
        }

        private void Player_New(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                // bi, nif and federation id is number

                // bi already in use

                // nif already in use

                // federation id already in use

                // valid date




                string gender;
                // INSERT PERSON
                if (GenderFemale.IsChecked == true)
                {
                    gender = "F";
                }
                else
                {
                    gender = "M";
                }

                string CmdString = "INSERT INTO football.person(bi, name, address, birth_date, nif, gender, nationality) VALUES (" + bi.Text + ", '"
                    + name.Text + "', '" + address.Text + "', '" + birth_date.Text + "', " + nif.Text + ", '" + gender + "', '" + nationality.Text + "')";
                SqlCommand cmd_person = new SqlCommand(CmdString, con);

                CmdString = "INSERT INTO football.internal_people(bi, salary) VALUES (" + bi.Text + ", '"+ salary.Value + "')";
                SqlCommand cmd_internal_people = new SqlCommand(CmdString, con);

                CmdString = "INSERT INTO football.player(bi, federation_id, weight, height) VALUES (" + bi.Text + ", " + federation_id.Text
                    + ", " + weight.Text + ", " + height.Text + ")";
                SqlCommand cmd_player = new SqlCommand(CmdString, con);

                try
                {
                    con.Open();
                    cmd_person.ExecuteNonQuery();
                    cmd_internal_people.ExecuteNonQuery();
                    cmd_player.ExecuteNonQuery();
                    con.Close();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

            }

            
        }

    }
}

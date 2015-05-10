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
                FillDataGrid(con);
            }
        }
        private void FillDataGrid(SqlConnection con)
        {
            /*
            * PLAYER TAB
            * */
            string CmdString = "SELECT * FROM football.udf_players_data_grid()";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("players");
            sda.Fill(dt);
            playersGrid.ItemsSource = dt.DefaultView;

            // fill the teams of the player
            CmdString = "SELECT * FROM football.udf_team_names(DEFAULT)";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("teams");
            sda.Fill(dt);

            foreach (DataRow team in dt.Rows)
            {
                ListBoxItem itm = new ListBoxItem();
                itm.Content = team[0].ToString();
                teams.Items.Add(itm);
            }

            /*
                * TEAMS TAB
                * */
            string CmdString1 = "SELECT * FROM football.teamsView";
            SqlCommand cmd1 = new SqlCommand(CmdString1, con);
            SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
            DataTable dt1 = new DataTable("teams");
            sda1.Fill(dt1);
            teamsGrid.ItemsSource = dt1.DefaultView;
        }

        private void playersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)playersGrid.SelectedItem;
                string search_bi;
                try
                {
                    // este try catch e por causa de quando autalizamos a DataGrid numa segunda vez
                    // e houve algo selecionado antes...
                    search_bi = row.Row.ItemArray[1].ToString();
                }
                catch (Exception)
                {
                    return;
                }

                bi.Text = search_bi;

                string CmdString = "SELECT * FROM football.udf_player(@intBi)";
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
                CmdString = "SELECT * FROM football.udf_team_names(@intBi)";
                cmd = new SqlCommand(CmdString, con);
                cmd.Parameters.AddWithValue("@intBi", Convert.ToInt32(search_bi));
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable("teams_selected");
                sda.Fill(dt);

                foreach (ListBoxItem itm in teams.Items)
                {
                    itm.IsSelected = false;
                    foreach (DataRow team in dt.Rows)
                    {
                        if(team[0].ToString() == itm.Content.ToString()){
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
                int biInt, nifInt, fedInt, weightInt, heightInt;

                // bi, nif and federation id is number
                if (!Int32.TryParse(bi.Text, out biInt))
                {
                    MessageBox.Show("The BI must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(nif.Text, out nifInt))
                {
                    MessageBox.Show("The NIF must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(federation_id.Text, out fedInt))
                {
                    MessageBox.Show("The Federation ID must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(weight.Text, out weightInt))
                {
                    MessageBox.Show("The weight must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(height.Text, out heightInt))
                {
                    MessageBox.Show("The height must be an Integer!");
                    return;
                }

                DateTime dt;
                if(!DateTime.TryParse(birth_date.Text, out dt))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }

                string gender;
                gender = (GenderFemale.IsChecked == true) ? "F" : "M";

                // INSERT PLAYER

                string CmdString = "EXEC football.sp_createPlayer @bi = @paramBi, @name = @paramName, @address = @paramAddress, @birth_date = @paramBirthDate, @nif = @paramNif, @gender = @paramGender, @nationality = @paramNationality, @salary = @paramSalary, @federation_id = @paramFed, @weight = @paramWeight, @height = @paramHeight";
                SqlCommand cmd_player = new SqlCommand(CmdString, con);
                cmd_player.Parameters.AddWithValue("@paramBi", biInt);
                cmd_player.Parameters.AddWithValue("@paramName", name.Text);
                cmd_player.Parameters.AddWithValue("@paramAddress", address.Text);
                cmd_player.Parameters.AddWithValue("@paramBirthDate", dt);
                cmd_player.Parameters.AddWithValue("@paramNif", nifInt);
                cmd_player.Parameters.AddWithValue("@paramGender", gender);
                cmd_player.Parameters.AddWithValue("@paramNationality", nationality.Text);
                cmd_player.Parameters.AddWithValue("@paramSalary", salary.Value);
                cmd_player.Parameters.AddWithValue("@paramFed", fedInt);
                cmd_player.Parameters.AddWithValue("@paramWeight", weightInt);
                cmd_player.Parameters.AddWithValue("@paramHeight", heightInt);

                try
                {
                    con.Open();
                    cmd_player.ExecuteNonQuery();
                    MessageBox.Show("The player has been inserted successfully!");
                    FillDataGrid(con);
                    con.Close();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                
            }
        }

        private void Player_Update(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int biInt, fedInt, weightInt, heightInt;

                // bi and federation id is number
                if (!Int32.TryParse(bi.Text, out biInt))
                {
                    MessageBox.Show("The BI must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(federation_id.Text, out fedInt))
                {
                    MessageBox.Show("The Federation ID must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(weight.Text, out weightInt))
                {
                    MessageBox.Show("The weight must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(height.Text, out heightInt))
                {
                    MessageBox.Show("The height must be an Integer!");
                    return;
                }

                DateTime dt;
                if (!DateTime.TryParse(birth_date.Text, out dt))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }

                string gender;
                gender = (GenderFemale.IsChecked == true) ? "F" : "M";

                // INSERT PLAYER

                string CmdString = "EXEC football.sp_modifyPlayer @bi = @paramBi, @name = @paramName, @address = @paramAddress, @birth_date = @paramBirthDate, @gender = @paramGender, @nationality = @paramNationality, @salary = @paramSalary, @federation_id = @paramFed, @weight = @paramWeight, @height = @paramHeight";
                SqlCommand cmd_player = new SqlCommand(CmdString, con);
                cmd_player.Parameters.AddWithValue("@paramBi", biInt);
                cmd_player.Parameters.AddWithValue("@paramName", name.Text);
                cmd_player.Parameters.AddWithValue("@paramAddress", address.Text);
                cmd_player.Parameters.AddWithValue("@paramBirthDate", dt);
                cmd_player.Parameters.AddWithValue("@paramGender", gender);
                cmd_player.Parameters.AddWithValue("@paramNationality", nationality.Text);
                cmd_player.Parameters.AddWithValue("@paramSalary", salary.Value);
                cmd_player.Parameters.AddWithValue("@paramFed", fedInt);
                cmd_player.Parameters.AddWithValue("@paramWeight", weightInt);
                cmd_player.Parameters.AddWithValue("@paramHeight", heightInt);

                try
                {
                    con.Open();
                    cmd_player.ExecuteNonQuery();
                    MessageBox.Show("The player has been updated successfully!");
                    FillDataGrid(con);
                    con.Close();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

            }
        }

        private void Player_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using (con = new SqlConnection(ConString))
                {
                    // --> Validations
                    int biInt;

                    // bi is number
                    if (!Int32.TryParse(bi.Text, out biInt))
                    {
                        MessageBox.Show("The BI must be an Integer!");
                        return;
                    }

                    // DELETE THE PLAYER

                    string CmdString = "EXEC football.sp_deletePlayer @bi = @paramBi";
                    SqlCommand cmd_player = new SqlCommand(CmdString, con);
                    cmd_player.Parameters.AddWithValue("@paramBi", biInt);

                    try
                    {
                        con.Open();
                        cmd_player.ExecuteNonQuery();
                        MessageBox.Show("The player has been deleted successfully!");
                        FillDataGrid(con);
                        con.Close();

                        // limpar as text boxs
                        name.Text = "";
                        bi.Text = "";
                        nif.Text = "";
                        address.Text = "";
                        federation_id.Text = "";
                        weight.Text = "";
                        height.Text = "";
                        birth_date.Text = "";
                        nationality.Text = "";
                        GenderMale.IsChecked = false;
                        GenderFemale.IsChecked = false;
                        salary.Value = 0;
                        internal_id.Text = "";
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }

                }
            }
        }

        private void teamsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)teamsGrid.SelectedItem;
                string search_name = row.Row.ItemArray[0].ToString();
                teamName.Text = search_name;
                string CmdString = "SELECT * FROM football.teamsView WHERE name='" + search_name+"'";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("team");
                sda.Fill(dt);
                DataRow r = dt.Rows[0];
                
                teamName.Text = r["name"].ToString();
                max_age.Text = r["max_age"].ToString();
            }
           
        }

        private void players_refresh(object sender, MouseButtonEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                this.FillDataGrid(con);
            }
        }

    }
}

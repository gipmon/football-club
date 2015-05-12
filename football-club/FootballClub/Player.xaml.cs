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
                FillDataGridTeam(con);
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *  ##########################----------- PLAYER TAB -----------##########################
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void FillDataGridPlayer(SqlConnection con)
        {
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

            playerTeams.Items.Clear();
            foreach (DataRow team in dt.Rows)
            {
                ListBoxItem itm = new ListBoxItem();
                itm.Content = team[0].ToString();
                playerTeams.Items.Add(itm);
            }

            int search_bi;
            if (Int32.TryParse(player_bi.Text, out search_bi))
            {
                playerTeamsGet(con, Convert.ToInt32(search_bi));
            }
        }

        private void sync_teams_player(SqlConnection con, Int32 biInt)
        {
            DataTable dt_playerTeams = new DataTable();
            dt_playerTeams.Columns.Add("team_name", typeof(String));
            dt_playerTeams.Columns.Add("bi", typeof(Int32));

            DataRow workRow;
            foreach (ListBoxItem itm in playerTeams.Items)
            {
                if (itm.IsSelected)
                {
                    workRow = dt_playerTeams.NewRow();
                    workRow["team_name"] = itm.Content.ToString().Trim();
                    workRow["bi"] = biInt;
                    dt_playerTeams.Rows.Add(workRow);
                }
            }
             
            // SYNC TEAMS PLAYER
            string CmdString = "football.sp_sync_playerTeams";
            SqlCommand cmd_player = new SqlCommand(CmdString, con);
            cmd_player.CommandType = CommandType.StoredProcedure;
            cmd_player.Parameters.AddWithValue("@bi", biInt);
            SqlParameter tvparam = cmd_player.Parameters.AddWithValue("@playerTeams", dt_playerTeams);
            tvparam.SqlDbType = SqlDbType.Structured;
            cmd_player.ExecuteNonQuery();
        }

        private void playerTeamsGet(SqlConnection con, Int32 biInt)
        {
            // select the teams of the player
            String CmdString = "SELECT * FROM football.udf_team_names(@bi)";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            cmd.Parameters.AddWithValue("@bi", biInt);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("teams_selected");
            sda.Fill(dt);

            foreach (ListBoxItem itm in playerTeams.Items)
            {
                itm.IsSelected = false;
                foreach (DataRow team in dt.Rows)
                {
                    if (team[0].ToString() == itm.Content.ToString())
                    {
                        itm.IsSelected = true;
                        break;
                    }
                }
            }
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

                player_bi.Text = search_bi;

                string CmdString = "SELECT * FROM football.udf_player(@bi)";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                cmd.Parameters.AddWithValue("@bi", Convert.ToInt32(search_bi));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("player");
                sda.Fill(dt);
                DataRow r = dt.Rows[0];

                player_bi.Text = r["bi"].ToString();
                player_name.Text = r["name"].ToString();
                player_nif.Text = r["nif"].ToString();
                player_address.Text = r["address"].ToString();

                if (r["gender"].ToString() == "F")
                {
                    player_GenderFemale.IsChecked = true;
                }
                else
                {
                    player_GenderMale.IsChecked = true;
                }

                DateTime date = DateTime.Parse(r["birth date"].ToString());
                player_birth_date.Text = date.ToString("yyyy-MM-dd");
                player_nationality.Text = r["nationality"].ToString();
                player_federation_id.Text = r["federation id"].ToString();
                player_salary.Value = Convert.ToDouble(r["salary"].ToString());
                player_internal_id.Text = r["internal id"].ToString();
                player_weight.Text = r["weight"].ToString();
                player_height.Text = r["height"].ToString();

                playerTeamsGet(con, Convert.ToInt32(search_bi));
            }
            
        }

        private void Player_New(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int biInt, nifInt, fedInt, weightInt, heightInt;

                // bi, nif and federation id is number
                if (!Int32.TryParse(player_bi.Text, out biInt))
                {
                    MessageBox.Show("The BI must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(player_nif.Text, out nifInt))
                {
                    MessageBox.Show("The NIF must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(player_federation_id.Text, out fedInt))
                {
                    MessageBox.Show("The Federation ID must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(player_weight.Text, out weightInt))
                {
                    MessageBox.Show("The weight must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(player_height.Text, out heightInt))
                {
                    MessageBox.Show("The height must be an Integer!");
                    return;
                }

                if (player_name.Text.Length == 0)
                {
                    MessageBox.Show("The name can't be blank!");
                    return;
                }
                if (player_address.Text.Length == 0)
                {
                    MessageBox.Show("The address can't be blank!");
                    return;
                }
                if (player_nationality.Text.Length == 0)
                {
                    MessageBox.Show("The nationality can't be blank!");
                    return;
                }


                DateTime dt;
                if (!DateTime.TryParse(player_birth_date.Text, out dt))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }

                string gender;
                if (player_GenderFemale.IsChecked == true)
                {
                    gender = "F";
                }
                else if (player_GenderMale.IsChecked == true)
                {
                    gender = "M";
                }
                else
                {
                    MessageBox.Show("Please select the gender!");
                    return;
                }

                // INSERT PLAYER

                string CmdString = "football.sp_createPlayer";
                SqlCommand cmd_player = new SqlCommand(CmdString, con);
                cmd_player.CommandType = CommandType.StoredProcedure;
                cmd_player.Parameters.AddWithValue("@bi", biInt);
                cmd_player.Parameters.AddWithValue("@name", player_name.Text);
                cmd_player.Parameters.AddWithValue("@address", player_address.Text);
                cmd_player.Parameters.AddWithValue("@birth_date", dt);
                cmd_player.Parameters.AddWithValue("@nif", nifInt);
                cmd_player.Parameters.AddWithValue("@gender", gender);
                cmd_player.Parameters.AddWithValue("@nationality", player_nationality.Text);
                cmd_player.Parameters.AddWithValue("@salary", (double)player_salary.Value);
                cmd_player.Parameters.AddWithValue("@federation_id", fedInt);
                cmd_player.Parameters.AddWithValue("@weight", weightInt);
                cmd_player.Parameters.AddWithValue("@height", heightInt);

                try
                {
                    con.Open();
                    cmd_player.ExecuteNonQuery();
                    sync_teams_player(con, biInt);
                    FillDataGridPlayer(con);
                    con.Close();
                    MessageBox.Show("The player has been inserted successfully!");
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
                if (!Int32.TryParse(player_bi.Text, out biInt))
                {
                    MessageBox.Show("The BI must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(player_federation_id.Text, out fedInt))
                {
                    MessageBox.Show("The Federation ID must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(player_weight.Text, out weightInt))
                {
                    MessageBox.Show("The weight must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(player_height.Text, out heightInt))
                {
                    MessageBox.Show("The height must be an Integer!");
                    return;
                }

                DateTime dt;
                if (!DateTime.TryParse(player_birth_date.Text, out dt))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }

                if (player_name.Text.Length == 0)
                {
                    MessageBox.Show("The name can't be blank!");
                    return;
                }
                if (player_address.Text.Length == 0)
                {
                    MessageBox.Show("The address can't be blank!");
                    return;
                }
                if (player_nationality.Text.Length == 0)
                {
                    MessageBox.Show("The nationality can't be blank!");
                    return;
                }

                string gender;
                if (player_GenderFemale.IsChecked == true)
                {
                    gender = "F";
                }
                else if (player_GenderMale.IsChecked == true)
                {
                    gender = "M";
                }
                else
                {
                    MessageBox.Show("Please select the gender!");
                    return;
                }

                // UPDATE PLAYER
                string CmdString = "football.sp_modifyPlayer";
                SqlCommand cmd_player = new SqlCommand(CmdString, con);
                cmd_player.CommandType = CommandType.StoredProcedure;
                cmd_player.Parameters.AddWithValue("@bi", biInt);
                cmd_player.Parameters.AddWithValue("@name", player_name.Text);
                cmd_player.Parameters.AddWithValue("@address", player_address.Text);
                cmd_player.Parameters.AddWithValue("@birth_date", dt);
                cmd_player.Parameters.AddWithValue("@gender", gender);
                cmd_player.Parameters.AddWithValue("@nationality", player_nationality.Text);
                cmd_player.Parameters.AddWithValue("@salary", (double)player_salary.Value);
                cmd_player.Parameters.AddWithValue("@federation_id", fedInt);
                cmd_player.Parameters.AddWithValue("@weight", weightInt);
                cmd_player.Parameters.AddWithValue("@height", heightInt);

                try
                {
                    con.Open();
                    cmd_player.ExecuteNonQuery();
                    sync_teams_player(con, biInt);
                    FillDataGridPlayer(con);
                    con.Close();
                    MessageBox.Show("The player has been updated successfully!");
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
                    if (!Int32.TryParse(player_bi.Text, out biInt))
                    {
                        MessageBox.Show("The BI must be an Integer!");
                        return;
                    }

                    // DELETE THE PLAYER

                    string CmdString = "football.sp_deletePlayer";
                    SqlCommand cmd_player = new SqlCommand(CmdString, con);
                    cmd_player.CommandType = CommandType.StoredProcedure;
                    cmd_player.Parameters.AddWithValue("@bi", biInt);

                    try
                    {
                        con.Open();
                        cmd_player.ExecuteNonQuery();
                        FillDataGridPlayer(con);
                        con.Close();

                        // limpar as text boxs
                        player_name.Text = "";
                        player_bi.Text = "";
                        player_nif.Text = "";
                        player_address.Text = "";
                        player_federation_id.Text = "";
                        player_weight.Text = "";
                        player_height.Text = "";
                        player_birth_date.Text = "";
                        player_nationality.Text = "";
                        player_GenderMale.IsChecked = false;
                        player_GenderFemale.IsChecked = false;
                        player_salary.Value = 0;
                        player_internal_id.Text = "";
                        MessageBox.Show("The player has been deleted successfully!");
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }

                }
            }
            
        }
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *  ##########################----------- COACH TAB -----------#############################
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void FillDataGridCoachs(SqlConnection con)
        {
            string CmdString = "SELECT * FROM football.udf_coachs_data_grid()";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("coachs");
            sda.Fill(dt);
            coachsGrid.ItemsSource = dt.DefaultView;
        }

        private void coachsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)coachsGrid.SelectedItem;
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

                coach_bi.Text = search_bi;

                string CmdString = "SELECT * FROM football.udf_coach(@bi)";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                cmd.Parameters.AddWithValue("@bi", Convert.ToInt32(search_bi));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("coach");
                sda.Fill(dt);
                DataRow r = dt.Rows[0];

                coach_bi.Text = r["bi"].ToString();
                coach_name.Text = r["name"].ToString();
                coach_nif.Text = r["nif"].ToString();
                coach_address.Text = r["address"].ToString();

                if (r["gender"].ToString() == "F")
                {
                    coach_GenderFemale.IsChecked = true;
                }
                else
                {
                    coach_GenderMale.IsChecked = true;
                }

                DateTime date = DateTime.Parse(r["birth date"].ToString());
                coach_birth_date.Text = date.ToString("yyyy-MM-dd");
                coach_nationality.Text = r["nationality"].ToString();
                coach_federation_id.Text = r["federation id"].ToString();
                coach_salary.Value = Convert.ToDouble(r["salary"].ToString());
                coach_internal_id.Text = r["internal id"].ToString();
                coach_role.Text = r["role"].ToString();

                // playerTeamsGet(con, Convert.ToInt32(search_bi));
            }
        }

        private void Coach_New(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int biInt, nifInt, fedInt;

                // bi, nif and federation id is number
                if (!Int32.TryParse(coach_bi.Text, out biInt))
                {
                    MessageBox.Show("The BI must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(coach_nif.Text, out nifInt))
                {
                    MessageBox.Show("The NIF must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(coach_federation_id.Text, out fedInt))
                {
                    MessageBox.Show("The Federation ID must be an Integer!");
                    return;
                }
                if (coach_role.Text.Length == 0)
                {
                    MessageBox.Show("The role can't be blank!");
                    return;
                }
                if (coach_name.Text.Length == 0)
                {
                    MessageBox.Show("The name can't be blank!");
                    return;
                }
                if (coach_address.Text.Length == 0)
                {
                    MessageBox.Show("The address can't be blank!");
                    return;
                }
                if (coach_nationality.Text.Length == 0)
                {
                    MessageBox.Show("The nationality can't be blank!");
                    return;
                }


                DateTime dt;
                if (!DateTime.TryParse(coach_birth_date.Text, out dt))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }

                string gender;
                if (coach_GenderFemale.IsChecked == true)
                {
                    gender = "F";
                }
                else if (coach_GenderMale.IsChecked == true)
                {
                    gender = "M";
                }
                else
                {
                    MessageBox.Show("Please select the gender!");
                    return;
                }

                // INSERT COACH

                string CmdString = "football.sp_createCoach";
                SqlCommand cmd_coach = new SqlCommand(CmdString, con);
                cmd_coach.CommandType = CommandType.StoredProcedure;
                cmd_coach.Parameters.AddWithValue("@bi", biInt);
                cmd_coach.Parameters.AddWithValue("@name", coach_name.Text);
                cmd_coach.Parameters.AddWithValue("@address", coach_address.Text);
                cmd_coach.Parameters.AddWithValue("@birth_date", dt);
                cmd_coach.Parameters.AddWithValue("@nif", nifInt);
                cmd_coach.Parameters.AddWithValue("@gender", gender);
                cmd_coach.Parameters.AddWithValue("@nationality", coach_nationality.Text);
                cmd_coach.Parameters.AddWithValue("@salary", (double)coach_salary.Value);
                cmd_coach.Parameters.AddWithValue("@federation_id", fedInt);
                cmd_coach.Parameters.AddWithValue("@role", coach_role.Text);

                try
                {
                    con.Open();
                    cmd_coach.ExecuteNonQuery();
                    // sync_teams_player(con, biInt);
                    FillDataGridCoachs(con);
                    con.Close();
                    MessageBox.Show("The coach has been inserted successfully!");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

            }
        }

        private void Coach_Update(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int biInt, fedInt;

                // bi and federation id is number
                if (!Int32.TryParse(coach_bi.Text, out biInt))
                {
                    MessageBox.Show("The BI must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(coach_federation_id.Text, out fedInt))
                {
                    MessageBox.Show("The Federation ID must be an Integer!");
                    return;
                }

                DateTime dt;
                if (!DateTime.TryParse(coach_birth_date.Text, out dt))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }

                if (coach_role.Text.Length == 0)
                {
                    MessageBox.Show("The role can't be blank!");
                    return;
                }
                if (coach_name.Text.Length == 0)
                {
                    MessageBox.Show("The name can't be blank!");
                    return;
                }
                if (coach_address.Text.Length == 0)
                {
                    MessageBox.Show("The address can't be blank!");
                    return;
                }
                if (coach_nationality.Text.Length == 0)
                {
                    MessageBox.Show("The nationality can't be blank!");
                    return;
                }

                string gender;
                if (coach_GenderFemale.IsChecked == true)
                {
                    gender = "F";
                }
                else if (coach_GenderMale.IsChecked == true)
                {
                    gender = "M";
                }
                else
                {
                    MessageBox.Show("Please select the gender!");
                    return;
                }

                // UPDATE COACH
                string CmdString = "football.sp_modifyCoach";
                SqlCommand cmd_coach = new SqlCommand(CmdString, con);
                cmd_coach.CommandType = CommandType.StoredProcedure;
                cmd_coach.Parameters.AddWithValue("@bi", biInt);
                cmd_coach.Parameters.AddWithValue("@name", coach_name.Text);
                cmd_coach.Parameters.AddWithValue("@address", coach_address.Text);
                cmd_coach.Parameters.AddWithValue("@birth_date", dt);
                cmd_coach.Parameters.AddWithValue("@gender", gender);
                cmd_coach.Parameters.AddWithValue("@nationality", coach_nationality.Text);
                cmd_coach.Parameters.AddWithValue("@salary", (double)coach_salary.Value);
                cmd_coach.Parameters.AddWithValue("@federation_id", fedInt);
                cmd_coach.Parameters.AddWithValue("@role", coach_role.Text);

                try
                {
                    con.Open();
                    cmd_coach.ExecuteNonQuery();
                    // sync_teams_player(con, biInt);
                    FillDataGridCoachs(con);
                    con.Close();
                    MessageBox.Show("The coach has been updated successfully!");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }

        }

        private void Coach_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using (con = new SqlConnection(ConString))
                {
                    // --> Validations
                    int biInt;

                    // bi is number
                    if (!Int32.TryParse(coach_bi.Text, out biInt))
                    {
                        MessageBox.Show("The BI must be an Integer!");
                        return;
                    }

                    // DELETE THE COACH

                    string CmdString = "football.sp_deleteCoach";
                    SqlCommand cmd_coach = new SqlCommand(CmdString, con);
                    cmd_coach.CommandType = CommandType.StoredProcedure;
                    cmd_coach.Parameters.AddWithValue("@bi", biInt);

                    try
                    {
                        con.Open();
                        cmd_coach.ExecuteNonQuery();
                        FillDataGridCoachs(con);
                        con.Close();

                        // limpar as text boxs
                        coach_name.Text = "";
                        coach_bi.Text = "";
                        coach_nif.Text = "";
                        coach_address.Text = "";
                        coach_federation_id.Text = "";
                        coach_birth_date.Text = "";
                        coach_nationality.Text = "";
                        coach_GenderMale.IsChecked = false;
                        coach_GenderFemale.IsChecked = false;
                        coach_salary.Value = 0;
                        coach_internal_id.Text = "";
                        coach_role.Text = "";
                        MessageBox.Show("The coach has been deleted successfully!");
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }
                }
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *  ##########################----------- TEAMS TAB -----------############################
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void FillDataGridTeam(SqlConnection con)
        {
            string CmdString = "SELECT * FROM football.udf_teams(DEFAULT)";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("teams");
            sda.Fill(dt);
            teamsGrid.ItemsSource = dt.DefaultView;
        }

        private void teamsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)teamsGrid.SelectedItem;
                string search_name;

                try
                {
                    // este try catch e por causa de quando autalizamos a DataGrid numa segunda vez
                    // e houve algo selecionado antes...
                    search_name = row.Row.ItemArray[0].ToString();
                }
                catch (Exception)
                {
                    return;
                }

                teamName.Text = search_name;
                string CmdString = "SELECT * FROM football.udf_teams(@name)";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                cmd.Parameters.AddWithValue("@name", search_name);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("team");
                sda.Fill(dt);
                DataRow r = dt.Rows[0];
                
                teamName.Text = r["name"].ToString();
                max_age.Value = Convert.ToDouble(r["max_age"].ToString());
            }
        }

        private void Team_New(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // validation: name can't not be lenght = 0
                if (teamName.Text.Length == 0)
                {
                    MessageBox.Show("The team name can't be blank!");
                    return;
                }

                string CmdString = "football.sp_createTeam";
                SqlCommand cmd_team = new SqlCommand(CmdString, con);
                cmd_team.CommandType = CommandType.StoredProcedure;
                cmd_team.Parameters.AddWithValue("@name", teamName.Text);
                cmd_team.Parameters.AddWithValue("@max_age", max_age.Value);

                try
                {
                    con.Open();
                    cmd_team.ExecuteNonQuery(); ;
                    FillDataGridTeam(con);
                    con.Close();
                    MessageBox.Show("The team has been inserted successfully!");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        private void Team_Update(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                string CmdString = "football.sp_modifyTeam";
                SqlCommand cmd_team = new SqlCommand(CmdString, con);
                cmd_team.CommandType = CommandType.StoredProcedure;
                cmd_team.Parameters.AddWithValue("@name", teamName.Text);
                cmd_team.Parameters.AddWithValue("@max_age", (double)max_age.Value);

                try
                {
                    con.Open();
                    cmd_team.ExecuteNonQuery();
                    FillDataGridTeam(con);
                    con.Close();
                    MessageBox.Show("The team has been updated successfully!");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        private void Team_Delete(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                string CmdString = "football.sp_deleteTeam";
                SqlCommand cmd_team = new SqlCommand(CmdString, con);
                cmd_team.CommandType = CommandType.StoredProcedure;
                cmd_team.Parameters.AddWithValue("@name", teamName.Text);

                try
                {
                    con.Open();
                    cmd_team.ExecuteNonQuery();
                    FillDataGridTeam(con);
                    con.Close();
                    MessageBox.Show("The team has been deleted successfully!");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *  ##########################----------- TAB CONTROL -----------##########################
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void PlayersTabIsSelected(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                FillDataGridPlayer(con);
            }
        }
        private void TeamsTabIsSelected(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                FillDataGridTeam(con);
            }
        }
        private void CoachsTabIsSelected(object sender, RoutedEventArgs e) 
        {
            using (con = new SqlConnection(ConString)) 
            {
                FillDataGridCoachs(con);
            }
        }

    }
}

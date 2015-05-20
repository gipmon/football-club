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
    /// Interaction logic for Practice.xaml
    /// </summary>
    public partial class Practice : Page
    {
        private string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
        private SqlConnection con;
        private string courtId;

        public Practice()
        {
            InitializeComponent();
            using (con = new SqlConnection(ConString))
            {
                FillDataGridCourts(con);
                FillDataGridPractices(con);
                FillStats(con);
            }
        }
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
        *  ########################----------- PRACTICES TAB -----------###########################
        * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void FillDataGridPractices(SqlConnection con)
        {
            string CmdString = "SELECT * FROM football.udf_practices(DEFAULT, DEFAULT, DEFAULT)";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("practices");
            sda.Fill(dt);
            practices_grid.ItemsSource = dt.DefaultView;

            // fill combo box court
            CmdString = "SELECT * FROM football.udf_courts(DEFAULT)";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("courts");
            sda.Fill(dt);

            CourtsComboBox.Items.Clear();
            foreach (DataRow court in dt.Rows)
            {
                ComboBoxItem itm = new ComboBoxItem();
                itm.Content = court[1].ToString();
                CourtsComboBox.Items.Add(itm);
            }

            // fill combo box teams
            CmdString = "SELECT * FROM football.udf_team_names()";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("teams");
            sda.Fill(dt);

            TeamsComboBox.Items.Clear();
            foreach (DataRow team in dt.Rows)
            {
                ComboBoxItem itm = new ComboBoxItem();
                itm.Content = team[0].ToString();
                TeamsComboBox.Items.Add(itm);
            }

        }
        private void practicesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)practices_grid.SelectedItem;
                try
                {
                    // este try catch e por causa de quando autalizamos a DataGrid numa segunda vez
                    // e houve algo selecionado antes...
                    DateTime date = DateTime.Parse(row.Row.ItemArray[0].ToString());
                    practice_date.Text = date.ToString("yyyy-MM-dd");
                    practice_hour.Text = row.Row.ItemArray[1].ToString();
                    TeamsComboBox.Text = row.Row.ItemArray[3].ToString();

                    string search_id = row.Row.ItemArray[2].ToString();
                    
                    String CmdString1 = "SELECT * FROM football.udf_courts(@id_court)";
                    SqlCommand cmd1 = new SqlCommand(CmdString1, con);
                    cmd1.Parameters.AddWithValue("@id_court", Convert.ToInt32(search_id));
                    SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                    DataTable dt1 = new DataTable("court_selected");
                    sda1.Fill(dt1);

                    foreach (ComboBoxItem itm in CourtsComboBox.Items)
                    {
                        itm.IsSelected = false;
                        foreach (DataRow court in dt1.Rows)
                        {
                            if (court[1].ToString() == itm.Content.ToString())
                            {
                                courtId = court[0].ToString();
                                itm.IsSelected = true;
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    return;
                }

                
            }
        }
        private void Practice_New(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // validation: team_name, id_court can't not be lenght = 0
                if (TeamsComboBox.Text.Length == 0)
                {
                    MessageBox.Show("The team name can't be blank!");
                    return;
                }
                if (CourtsComboBox.Text.Length == 0)
                {
                    MessageBox.Show("The court can't be blank!");
                    return;
                }

                DateTime date;
                if (!DateTime.TryParse(practice_date.Text, out date))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }
                TimeSpan time;
                if (!TimeSpan.TryParse(practice_hour.Text, out time))
                {
                    MessageBox.Show("Please insert a valid hour!");
                    return;
                }

                string CmdString1 = "SELECT * FROM football.udf_courts(DEFAULT)";
                SqlCommand cmd = new SqlCommand(CmdString1, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt1 = new DataTable("court_selected");
                sda.Fill(dt1);
                string courts = CourtsComboBox.Text;

                foreach (DataRow court in dt1.Rows)
                {
                    if (court[1].ToString() == courts)
                    {
                        courtId = court[0].ToString();
                        break;
                    }
                }

                int court_ID;
                if (!Int32.TryParse(courtId, out court_ID))
                {
                    MessageBox.Show("The Court must be valid!");
                    return;
                }

                string CmdString = "football.sp_createPractice";
                SqlCommand cmd_practice = new SqlCommand(CmdString, con);
                cmd_practice.CommandType = CommandType.StoredProcedure;
                cmd_practice.Parameters.AddWithValue("@id_court", court_ID);
                cmd_practice.Parameters.AddWithValue("@date", date);
                cmd_practice.Parameters.AddWithValue("@hour", time);
                cmd_practice.Parameters.AddWithValue("@team_name", TeamsComboBox.Text);

                try
                {
                    con.Open();
                    cmd_practice.ExecuteNonQuery();
                    FillStats(con);
                    FillDataGridPractices(con);
                    con.Close();
                    MessageBox.Show("The practice has been inserted successfully!");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }
        private void Practice_Update(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // validation: team_name, id_court can't not be lenght = 0
                if (TeamsComboBox.Text.Length == 0)
                {
                    MessageBox.Show("The team name can't be blank!");
                    return;
                }
                if (CourtsComboBox.Text.Length == 0)
                {
                    MessageBox.Show("The court can't be blank!");
                    return;
                }

                DateTime date;
                if (!DateTime.TryParse(practice_date.Text, out date))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }
                TimeSpan time;
                if (!TimeSpan.TryParse(practice_hour.Text, out time))
                {
                    MessageBox.Show("Please insert a valid hour!");
                    return;
                }

                string CmdString1 = "SELECT * FROM football.udf_courts(DEFAULT)";
                SqlCommand cmd = new SqlCommand(CmdString1, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt1 = new DataTable("court_selected");
                sda.Fill(dt1);
                string courts = CourtsComboBox.Text;

                foreach (DataRow court in dt1.Rows)
                {
                    if (court[0].ToString() == courts)
                    {
                        courtId = court[0].ToString();
                        break;
                    }
                }

                int court_ID;
                if (!Int32.TryParse(courtId, out court_ID))
                {
                    MessageBox.Show("The Court must be valid!");
                    return;
                }

                string CmdString = "football.sp_modifyPractice";
                SqlCommand cmd_practice = new SqlCommand(CmdString, con);
                cmd_practice.CommandType = CommandType.StoredProcedure;
                cmd_practice.Parameters.AddWithValue("@id_court", court_ID);
                cmd_practice.Parameters.AddWithValue("@date", date);
                cmd_practice.Parameters.AddWithValue("@hour", time);
                cmd_practice.Parameters.AddWithValue("@team_name", TeamsComboBox.Text);

                try
                {
                    con.Open();
                    cmd_practice.ExecuteNonQuery();
                    FillStats(con);
                    FillDataGridPractices(con);
                    con.Close();
                    practice_date.Text = "";
                    practice_hour.Text = "";
                    MessageBox.Show("The practice has been updated successfully!");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }
        private void Practice_Delete(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                if (CourtsComboBox.Text.Length == 0)
                {
                    MessageBox.Show("The court can't be blank!");
                    return;
                }

                int courtId;

                if (!Int32.TryParse(CourtsComboBox.Text, out courtId))
                {
                    MessageBox.Show("The court id must be an Integer!");
                    return;
                }
                DateTime date;
                if (!DateTime.TryParse(practice_date.Text, out date))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }
                TimeSpan time;
                if (!TimeSpan.TryParse(practice_hour.Text, out time))
                {
                    MessageBox.Show("Please insert a valid hour!");
                    return;
                }

                string CmdString = "football.sp_deletePractice";
                SqlCommand cmd_practice = new SqlCommand(CmdString, con);
                cmd_practice.CommandType = CommandType.StoredProcedure;
                cmd_practice.Parameters.AddWithValue("@id_court", courtId);
                cmd_practice.Parameters.AddWithValue("@date", date);
                cmd_practice.Parameters.AddWithValue("@hour", time);

                try
                {
                    con.Open();
                    cmd_practice.ExecuteNonQuery();
                    FillStats(con);
                    FillDataGridPractices(con);
                    con.Close();
                    TeamsComboBox.Text = "";
                    CourtsComboBox.Text = "";
                    practice_date.Text = "";
                    practice_hour.Text = "";
                    MessageBox.Show("The practice has been deleted successfully!");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
        *  ##########################----------- COURT TAB -----------#############################
        * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void FillDataGridCourts(SqlConnection con)
        {
            string CmdString = "SELECT * FROM football.udf_courts(DEFAULT)";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("coachs");
            sda.Fill(dt);
            courts_grid.ItemsSource = dt.DefaultView;
        }
        private void courtsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)courts_grid.SelectedItem;
                string search_id;
                try
                {
                    // este try catch e por causa de quando autalizamos a DataGrid numa segunda vez
                    // e houve algo selecionado antes...
                    search_id = row.Row.ItemArray[0].ToString();
                }
                catch (Exception)
                {
                    return;
                }

                courts_id.Text = search_id;

                string CmdString = "SELECT * FROM football.udf_courts(@id_court)";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                cmd.Parameters.AddWithValue("@id_court", Convert.ToInt32(search_id));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("courts");
                sda.Fill(dt);
                DataRow r = dt.Rows[0];

                courts_address.Text = r["address"].ToString();
            }
        }
        private void Court_New(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // validation: address can't not be lenght = 0
                if (courts_address.Text.Length == 0)
                {
                    MessageBox.Show("The address name can't be blank!");
                    return;
                }

                string CmdString = "football.sp_createCourt";
                SqlCommand cmd_court = new SqlCommand(CmdString, con);
                cmd_court.CommandType = CommandType.StoredProcedure;
                cmd_court.Parameters.AddWithValue("@address", courts_address.Text);

                try
                {
                    con.Open();
                    cmd_court.ExecuteNonQuery();
                    FillDataGridPractices(con);
                    FillStats(con);
                    FillDataGridCourts(con);
                    con.Close();
                    MessageBox.Show("The court has been inserted successfully!");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }
        private void Court_Update(object sender, RoutedEventArgs e)
        {
            int courtsId;

            if (!Int32.TryParse(courts_id.Text, out courtsId))
            {
                MessageBox.Show("The court id must be an Integer!");
                return;
            }

            using (con = new SqlConnection(ConString))
            {
                string CmdString = "football.sp_modifyCourt";
                SqlCommand cmd_court = new SqlCommand(CmdString, con);
                cmd_court.CommandType = CommandType.StoredProcedure;
                cmd_court.Parameters.AddWithValue("@id_court", courtsId);
                cmd_court.Parameters.AddWithValue("@address", courts_address.Text);

                try
                {
                    con.Open();
                    cmd_court.ExecuteNonQuery();
                    FillDataGridPractices(con);
                    FillStats(con);
                    FillDataGridCourts(con);
                    con.Close();
                    MessageBox.Show("The court has been updated successfully!");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }
        private void Court_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using (con = new SqlConnection(ConString))
                {
                    // --> Validations
                    int courtsId;

                    if (!Int32.TryParse(courts_id.Text, out courtsId))
                    {
                        MessageBox.Show("The court id must be an Integer!");
                        return;
                    }

                    // DELETE THE COURT

                    string CmdString = "football.sp_deleteCourt";
                    SqlCommand cmd_court = new SqlCommand(CmdString, con);
                    cmd_court.CommandType = CommandType.StoredProcedure;
                    cmd_court.Parameters.AddWithValue("@id_court", courtsId);

                    try
                    {
                        con.Open();
                        cmd_court.ExecuteNonQuery();
                        FillDataGridPractices(con);
                        FillStats(con);
                        FillDataGridCourts(con);
                        con.Close();

                        // limpar as text boxs
                        courts_id.Text = "";
                        courts_address.Text = "";
                        MessageBox.Show("The court has been deleted successfully!");
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }
                }
            }
        }
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *  ##########################----------- STATS  TAB -----------############################
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void FillStats(SqlConnection con)
        {
            // number_practices_per_court
            string CmdString = "SELECT * FROM football.udf_number_practices_per_court()";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("number_players_per_team");
            sda.Fill(dt);
            number_practices_per_court.ItemsSource = dt.DefaultView;

            CmdString = "SELECT * FROM football.udf_average_hour_of_training_by_court()";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("average_hour_of_training_by_court");
            sda.Fill(dt);
            average_hour_of_training_by_court.ItemsSource = dt.DefaultView;

            /*CmdString = "SELECT * FROM football.udf_latest_team_to_train_in_each_court()";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("latest_team_to_train_in_each_court");
            sda.Fill(dt);
            latest_team_to_train_in_each_court.ItemsSource = dt.DefaultView;*/

            CmdString = "SELECT * FROM football.udf_team_that_trained_more_by_court()";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("team_that_trained_more_by_court");
            sda.Fill(dt);
            team_that_trained_more_by_court.ItemsSource = dt.DefaultView;
        }
    }
}

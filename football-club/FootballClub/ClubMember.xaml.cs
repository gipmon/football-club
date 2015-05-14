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
    /// Interaction logic for ClubMember.xaml
    /// </summary>
    public partial class ClubMember : Page
    {
        private string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
        private SqlConnection con;
        private string section_id;

        public ClubMember()
        {
            InitializeComponent();
            using (con = new SqlConnection(ConString))
            {
                FillDataGridMembers(con);
                FillDataGridAnnualSpots(con);
                
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *  ##########################----------- MEMBERS TAB -----------##########################
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void FillDataGridMembers(SqlConnection con)
        {
            string CmdString = "SELECT * FROM football.udf_members_data_grid(DEFAULT)";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("members");
            sda.Fill(dt);
            clubMembersGrid.ItemsSource = dt.DefaultView;

        }

        private void clubMembersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)clubMembersGrid.SelectedItem;
                string search_bi;
                try
                {
                    // este try catch e por causa de quando autalizamos a DataGrid numa segunda vez
                    // e houve algo selecionado antes...
                    search_bi = row.Row.ItemArray[2].ToString();
                }
                catch (Exception)
                {
                    return;
                }

                member_bi.Text = search_bi;

                string CmdString = "SELECT * FROM football.udf_member(@bi)";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                cmd.Parameters.AddWithValue("@bi", Convert.ToInt32(search_bi));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("member");
                sda.Fill(dt);
                DataRow r = dt.Rows[0];

                member_bi.Text = r["bi"].ToString();
                member_name.Text = r["name"].ToString();
                member_nif.Text = r["nif"].ToString();
                member_address.Text = r["address"].ToString();

                if (r["gender"].ToString() == "F")
                {
                    member_GenderFemale.IsChecked = true;
                }
                else
                {
                    member_GenderMale.IsChecked = true;
                }

                DateTime date = DateTime.Parse(r["birth date"].ToString());
                member_birth_date.Text = date.ToString("yyyy-MM-dd");
                member_nationality.Text = r["nationality"].ToString();
                member_number.Text = r["n_member"].ToString();
                member_shares_value.Value = Convert.ToDouble(r["shares_value"].ToString());
                string shares = r["shares_in_day"].ToString();
                if (r["shares_in_day"].ToString() == "True")
                {
                     member_shares_in_day.IsChecked = true;
                }
                else
                {
                     member_shares_in_day.IsChecked = false;
                }
            }
            
        }

        private void Member_New(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int biInt, nifInt;

                // bi, nif and federation id is number
                if (!Int32.TryParse(member_bi.Text, out biInt))
                {
                    MessageBox.Show("The BI must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(member_nif.Text, out nifInt))
                {
                    MessageBox.Show("The NIF must be an Integer!");
                    return;
                }

                if (member_name.Text.Length == 0)
                {
                    MessageBox.Show("The name can't be blank!");
                    return;
                }
                if (member_address.Text.Length == 0)
                {
                    MessageBox.Show("The address can't be blank!");
                    return;
                }
                if (member_nationality.Text.Length == 0)
                {
                    MessageBox.Show("The nationality can't be blank!");
                    return;
                }


                DateTime dt;
                if (!DateTime.TryParse(member_birth_date.Text, out dt))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }

                string gender;
                if (member_GenderFemale.IsChecked == true)
                {
                    gender = "F";
                }
                else if (member_GenderMale.IsChecked == true)
                {
                    gender = "M";
                }
                else
                {
                    MessageBox.Show("Please select the gender!");
                    return;
                }

                string shares_in_day;
                if (member_shares_in_day.IsChecked == true)
                {
                    shares_in_day = "True";
                }
                else if (member_shares_in_day.IsChecked == false)
                {
                    shares_in_day = "False";
                }
                else
                {
                    MessageBox.Show("Please select the shares situation!");
                    return;
                }


                // INSERT MEMBER

                string CmdString = "football.sp_createMember";
                SqlCommand cmd_member = new SqlCommand(CmdString, con);
                cmd_member.CommandType = CommandType.StoredProcedure;
                cmd_member.Parameters.AddWithValue("@bi", biInt);
                cmd_member.Parameters.AddWithValue("@name", member_name.Text);
                cmd_member.Parameters.AddWithValue("@address", member_address.Text);
                cmd_member.Parameters.AddWithValue("@birth_date", dt);
                cmd_member.Parameters.AddWithValue("@nif", nifInt);
                cmd_member.Parameters.AddWithValue("@gender", gender);
                cmd_member.Parameters.AddWithValue("@nationality", member_nationality.Text);
                cmd_member.Parameters.AddWithValue("@shares_value", (double)member_shares_value.Value);
                cmd_member.Parameters.AddWithValue("@shares_in_day", shares_in_day);

                try
                {
                    con.Open();
                    cmd_member.ExecuteNonQuery();
                    FillDataGridAnnualSpots(con);
                    FillDataGridMembers(con);
                    con.Close();
                    MessageBox.Show("The member has been inserted successfully!");
                }
                catch (Exception exc)
                {
                   MessageBox.Show(exc.Message);
                }

            }

        }

        private void Member_Update(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int biInt;

                // bi and federation id is number
                if (!Int32.TryParse(member_bi.Text, out biInt))
                {
                    MessageBox.Show("The BI must be an Integer!");
                    return;
                }
              
                DateTime dt;
                if (!DateTime.TryParse(member_birth_date.Text, out dt))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }

                if (member_name.Text.Length == 0)
                {
                    MessageBox.Show("The name can't be blank!");
                    return;
                }
                if (member_address.Text.Length == 0)
                {
                    MessageBox.Show("The address can't be blank!");
                    return;
                }
                if (member_nationality.Text.Length == 0)
                {
                    MessageBox.Show("The nationality can't be blank!");
                    return;
                }

                string gender;
                if (member_GenderFemale.IsChecked == true)
                {
                    gender = "F";
                }
                else if (member_GenderMale.IsChecked == true)
                {
                    gender = "M";
                }
                else
                {
                    MessageBox.Show("Please select the gender!");
                    return;
                }

                string shares_in_day;
                if (member_shares_in_day.IsChecked == true)
                {
                    shares_in_day = "True";
                }
                else if (member_shares_in_day.IsChecked == false)
                {
                    shares_in_day = "False";
                }
                else
                {
                    MessageBox.Show("Please select the shares situation!");
                    return;
                }

                // UPDATE MEMBER
                string CmdString = "football.sp_modifyMember";
                SqlCommand cmd_member = new SqlCommand(CmdString, con);
                cmd_member.CommandType = CommandType.StoredProcedure;
                cmd_member.Parameters.AddWithValue("@bi", biInt);
                cmd_member.Parameters.AddWithValue("@name", member_name.Text);
                cmd_member.Parameters.AddWithValue("@address", member_address.Text);
                cmd_member.Parameters.AddWithValue("@birth_date", dt);
                cmd_member.Parameters.AddWithValue("@gender", gender);
                cmd_member.Parameters.AddWithValue("@nationality", member_nationality.Text);
                cmd_member.Parameters.AddWithValue("@shares_value", (double)member_shares_value.Value);
                cmd_member.Parameters.AddWithValue("@shares_in_day", shares_in_day);
                
                try
                {
                    con.Open();
                    cmd_member.ExecuteNonQuery();
                    FillDataGridAnnualSpots(con);
                    FillDataGridMembers(con);
                    con.Close();
                    MessageBox.Show("The member has been updated successfully!");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }

        }

        private void Member_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using (con = new SqlConnection(ConString))
                {
                    // --> Validations
                    int biInt;

                    // bi is number
                    if (!Int32.TryParse(member_bi.Text, out biInt))
                    {
                        MessageBox.Show("The BI must be an Integer!");
                        return;
                    }

                    // DELETE THE MEMBER

                    string CmdString = "football.sp_deleteMember";
                    SqlCommand cmd_member = new SqlCommand(CmdString, con);
                    cmd_member.CommandType = CommandType.StoredProcedure;
                    cmd_member.Parameters.AddWithValue("@bi", biInt);

                    try
                    {
                        con.Open();
                        cmd_member.ExecuteNonQuery();
                        FillDataGridAnnualSpots(con);
                        FillDataGridMembers(con);
                        con.Close();

                        // limpar as text boxs
                        member_name.Text = "";
                        member_bi.Text = "";
                        member_nif.Text = "";
                        member_address.Text = "";
                        member_shares_in_day.IsChecked = false;
                        member_birth_date.Text = "";
                        member_nationality.Text = "";
                        member_GenderMale.IsChecked = false;
                        member_GenderFemale.IsChecked = false;
                        member_shares_value.Value = 0;
                        member_number.Text = "";
                        MessageBox.Show("The member has been deleted successfully!");
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }

                }
            }

        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *  ##########################----------- ANNUAL SPOT TAB -----------#############################
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void FillDataGridAnnualSpots(SqlConnection con)
        {
            string CmdString = "SELECT * FROM football.udf_annualSpots(DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT)";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("annual_spots");
            sda.Fill(dt);
            annualSpotsGrid.ItemsSource = dt.DefaultView;

            
            // fill the sections of the stadium
            CmdString = "SELECT * FROM football.udf_sections()";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("sections");
            sda.Fill(dt);

            spot_section.Items.Clear();
            foreach (DataRow court in dt.Rows)
            {
                ComboBoxItem itm = new ComboBoxItem();
                itm.Content = court[0].ToString();
                spot_section.Items.Add(itm);
            }

        }

        private void annualSpotsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)annualSpotsGrid.SelectedItem;
                string search_bi;
                try
                {
                    // este try catch e por causa de quando autalizamos a DataGrid numa segunda vez
                    // e houve algo selecionado antes...
                    search_bi = row.Row.ItemArray[2].ToString();
                }
                catch (Exception)
                {
                    return;
                }

                spot_bi.Text = search_bi;

                string search_spot;
                try
                {
                    // este try catch e por causa de quando autalizamos a DataGrid numa segunda vez
                    // e houve algo selecionado antes...
                    search_spot = row.Row.ItemArray[6].ToString();
                }
                catch (Exception)
                {
                    return;
                }

                spot_number.Text = search_spot;

                string search_row;
                try
                {
                    // este try catch e por causa de quando autalizamos a DataGrid numa segunda vez
                    // e houve algo selecionado antes...
                    search_row = row.Row.ItemArray[5].ToString();
                }
                catch (Exception)
                {
                    return;
                }


                string search_section;
                try
                {
                    // este try catch e por causa de quando autalizamos a DataGrid numa segunda vez
                    // e houve algo selecionado antes...
                    search_section = row.Row.ItemArray[4].ToString();
                }
                catch (Exception)
                {
                    return;
                }

                string search_season;
                try
                {
                    // este try catch e por causa de quando autalizamos a DataGrid numa segunda vez
                    // e houve algo selecionado antes...
                    search_season = row.Row.ItemArray[7].ToString();
                }
                catch (Exception)
                {
                    return;
                }

                spot_season.Text = search_season;

                string CmdString = "SELECT * FROM football.udf_annualSpots_full(@n_spot, @row, @id_section, @bi, @season)";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                cmd.Parameters.AddWithValue("@n_spot", Convert.ToInt32(search_spot));
                cmd.Parameters.AddWithValue("@row", search_row);
                cmd.Parameters.AddWithValue("@id_section", Convert.ToInt32(search_section));
                cmd.Parameters.AddWithValue("@bi", Convert.ToInt32(search_bi));
                cmd.Parameters.AddWithValue("@season", Convert.ToInt32(search_season));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("annualSpot");
                sda.Fill(dt);
                DataRow r = dt.Rows[0];

                spot_bi.Text = r["bi"].ToString();
                spot_number.Text = r["spot number"].ToString();
                spot_row.Text = r["row"].ToString();
                spot_season.Text = r["season"].ToString();

                DateTime date = DateTime.Parse(r["start_date"].ToString());
                spot_initial_date.Text = date.ToString("yyyy-MM-dd");
                spot_duration.Text = r["duration"].ToString();
                spot_value.Value = Convert.ToDouble(r["value"].ToString());

                String CmdString1 = "SELECT * FROM football.udf_sections_annual(@bi, @n_spot, @row, @id_section, @season)";
                SqlCommand cmd1 = new SqlCommand(CmdString1, con);
                cmd1.Parameters.AddWithValue("@bi", Convert.ToInt32(search_bi));
                cmd1.Parameters.AddWithValue("@n_spot", Convert.ToInt32(search_spot));
                cmd1.Parameters.AddWithValue("@row", search_row);
                cmd1.Parameters.AddWithValue("@id_section", Convert.ToInt32(search_section));
                cmd1.Parameters.AddWithValue("@season", Convert.ToInt32(search_season));

                SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                DataTable dt1 = new DataTable("section_selected");
                sda1.Fill(dt1);

                foreach (ComboBoxItem itm in spot_section.Items)
                {
                    itm.IsSelected = false;
                    foreach (DataRow section in dt1.Rows)
                    {
                        if (section[0].ToString() == itm.Content.ToString())
                        {
                            section_id = section[1].ToString();
                            itm.IsSelected = true;
                            break;
                        }
                    }
                }
                
            }
        }

        private void AnnualSpot_New(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int biInt, nSpotInt, sectionidInt, seasonInt, durationInt;

                if (!Int32.TryParse(spot_bi.Text, out biInt))
                {
                    MessageBox.Show("The BI must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(spot_number.Text, out nSpotInt))
                {
                    MessageBox.Show("The Spot Number must be an Integer!");
                    return;
                }

                if (!Int32.TryParse(spot_season.Text, out seasonInt))
                {
                    MessageBox.Show("The Season must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(spot_duration.Text, out durationInt))
                {
                    MessageBox.Show("The Duration must be an Integer!");
                    return;
                }

                if (spot_row.Text.Length == 0)
                {
                    MessageBox.Show("The row can't be blank!");
                    return;
                }

                DateTime dt;
                if (!DateTime.TryParse(spot_initial_date.Text, out dt))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }


                string CmdString1 = "SELECT * FROM football.udf_sections()";
                SqlCommand cmd = new SqlCommand(CmdString1, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt1 = new DataTable("section_selected");
                sda.Fill(dt1);
                string section_text = spot_section.Text;

                foreach (DataRow section in dt1.Rows)
                {
                    if (section[0].ToString() == section_text)
                    {
                        section_id = section[1].ToString();
                        break;
                    }
                }


                if (!Int32.TryParse(section_id, out sectionidInt))
                {
                    MessageBox.Show("The Section must be valid!");
                    return;
                }

                // INSERT ANNUAL SPOT

                string CmdString = "football.sp_createAnnualSpot";
                SqlCommand cmd_annualSpot = new SqlCommand(CmdString, con);
                cmd_annualSpot.CommandType = CommandType.StoredProcedure;
                cmd_annualSpot.Parameters.AddWithValue("@bi", biInt);
                cmd_annualSpot.Parameters.AddWithValue("@start_date", dt);
                cmd_annualSpot.Parameters.AddWithValue("@n_spot", nSpotInt);
                cmd_annualSpot.Parameters.AddWithValue("@row", spot_row.Text);
                cmd_annualSpot.Parameters.AddWithValue("@value", spot_value.Value);
                cmd_annualSpot.Parameters.AddWithValue("@id_section", sectionidInt);
                cmd_annualSpot.Parameters.AddWithValue("@season", seasonInt);
                cmd_annualSpot.Parameters.AddWithValue("@duration", durationInt);

                try
                {
                    con.Open();
                    cmd_annualSpot.ExecuteNonQuery();
                    FillDataGridMembers(con);
                    FillDataGridAnnualSpots(con);
                    con.Close();
                    MessageBox.Show("The annual spot has been inserted successfully!");

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

            }
        }

        private void AnnualSpot_Update(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int biInt, nSpotInt, sectionidInt, seasonInt, durationInt;

                if (!Int32.TryParse(spot_bi.Text, out biInt))
                {
                    MessageBox.Show("The BI must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(spot_number.Text, out nSpotInt))
                {
                    MessageBox.Show("The Spot Number must be an Integer!");
                    return;
                }

                if (!Int32.TryParse(spot_season.Text, out seasonInt))
                {
                    MessageBox.Show("The Season must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(spot_duration.Text, out durationInt))
                {
                    MessageBox.Show("The Duration must be an Integer!");
                    return;
                }

                if (spot_row.Text.Length == 0)
                {
                    MessageBox.Show("The row can't be blank!");
                    return;
                }

                DateTime dt;
                if (!DateTime.TryParse(spot_initial_date.Text, out dt))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }

                string CmdString1 = "SELECT * FROM football.udf_sections()";
                SqlCommand cmd = new SqlCommand(CmdString1, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt1 = new DataTable("section_selected");
                sda.Fill(dt1);
                string section_text = spot_section.Text;

                foreach (DataRow section in dt1.Rows)
                {
                    if (section[0].ToString() == section_text)
                    {
                        section_id = section[1].ToString();
                        break;
                    }
                }


                if (!Int32.TryParse(section_id, out sectionidInt))
                {
                    MessageBox.Show("The Section must be valid!");
                    return;
                }


                // UPDATE ANNUAL SPOT

                string CmdString = "football.sp_modifyAnnualSpot";
                SqlCommand cmd_annualSpot = new SqlCommand(CmdString, con);
                cmd_annualSpot.CommandType = CommandType.StoredProcedure;
                cmd_annualSpot.Parameters.AddWithValue("@bi", biInt);
                cmd_annualSpot.Parameters.AddWithValue("@start_date", dt);
                cmd_annualSpot.Parameters.AddWithValue("@n_spot", nSpotInt);
                cmd_annualSpot.Parameters.AddWithValue("@row", spot_row.Text);
                cmd_annualSpot.Parameters.AddWithValue("@value", spot_value.Value);
                cmd_annualSpot.Parameters.AddWithValue("@id_section", sectionidInt);
                cmd_annualSpot.Parameters.AddWithValue("@season", seasonInt);
                cmd_annualSpot.Parameters.AddWithValue("@duration", durationInt);

                try
                {
                    con.Open();
                    cmd_annualSpot.ExecuteNonQuery();
                    FillDataGridMembers(con);
                    FillDataGridAnnualSpots(con);
                    con.Close();
                    MessageBox.Show("The annual spot has been updated successfully!");

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

            }
        }

        private void AnnualSpot_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using (con = new SqlConnection(ConString))
                {
                    // --> Validations
                    int biInt, nSpotInt, sectionidInt, seasonInt;

                    // bi, spot number, section id and season are number
                    if (!Int32.TryParse(spot_bi.Text, out biInt))
                    {
                        MessageBox.Show("The BI must be an Integer!");
                        return;
                    }
                    if (!Int32.TryParse(spot_number.Text, out nSpotInt))
                    {
                        MessageBox.Show("The Spot Number must be an Integer!");
                        return;
                    }

                    if (!Int32.TryParse(spot_season.Text, out seasonInt))
                    {
                        MessageBox.Show("The Season must be an Integer!");
                        return;
                    }

                    if (spot_row.Text.Length == 0)
                    {
                        MessageBox.Show("The row can't be blank!");
                        return;
                    }

                    string CmdString1 = "SELECT * FROM football.udf_sections()";
                    SqlCommand cmd = new SqlCommand(CmdString1, con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt1 = new DataTable("section_selected");
                    sda.Fill(dt1);
                    string section_text = spot_section.Text;

                    foreach (DataRow section in dt1.Rows)
                    {
                        if (section[0].ToString() == section_text)
                        {
                            section_id = section[1].ToString();
                            break;
                        }
                    }


                    if (!Int32.TryParse(section_id, out sectionidInt))
                    {
                        MessageBox.Show("The Section must be valid!");
                        return;
                    }


                    // DELETE THE ANNUAL SPOT

                    string CmdString = "football.sp_deleteAnnualSpot";
                    SqlCommand cmd_annualSpot = new SqlCommand(CmdString, con);
                    cmd_annualSpot.CommandType = CommandType.StoredProcedure;
                    cmd_annualSpot.Parameters.AddWithValue("@bi", biInt);
                    cmd_annualSpot.Parameters.AddWithValue("@n_spot", nSpotInt);
                    cmd_annualSpot.Parameters.AddWithValue("@season", seasonInt);
                    cmd_annualSpot.Parameters.AddWithValue("@row", spot_row.Text);
                    cmd_annualSpot.Parameters.AddWithValue("@id_section", sectionidInt);

                    try
                    {
                        con.Open();
                        cmd_annualSpot.ExecuteNonQuery();
                        FillDataGridMembers(con);
                        FillDataGridAnnualSpots(con);

                        con.Close();

                        // limpar as text boxs
                        spot_bi.Text = "";
                        spot_number.Text = "";
                        spot_row.Text = "";
                        spot_section.Text = "";
                        spot_value.Value = 0;
                        spot_duration.Text = "";
                        spot_initial_date.Text = "";
                        spot_season.Text = "";

                        MessageBox.Show("The annual spot has been deleted successfully!");

                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }

                }
            }
        }
    }
}

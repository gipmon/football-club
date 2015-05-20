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
                FillDataGridAnnualSeats(con);
                fillStats(con);

                
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
                member_shares_in_day.Text = r["shares_in_day"].ToString();
               
            }
            
        }

        private void Member_New(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int biInt, nifInt, shares_in_dayInt;

                // bi, nif and federation id and shares_in_day is number
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
                if (!Int32.TryParse(member_shares_in_day.Text, out shares_in_dayInt))
                {
                    MessageBox.Show("The Shares In Day must be an Integer!");
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
                cmd_member.Parameters.AddWithValue("@shares_in_day", shares_in_dayInt);

                try
                {
                    con.Open();
                    cmd_member.ExecuteNonQuery();
                    FillDataGridAnnualSeats(con);
                    fillStats(con);
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
                int biInt, shares_in_dayInt;

                // bi and federation id is number
                if (!Int32.TryParse(member_bi.Text, out biInt))
                {
                    MessageBox.Show("The BI must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(member_shares_in_day.Text, out shares_in_dayInt))
                {
                    MessageBox.Show("The Shares In Day must be an Integer!");
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
                cmd_member.Parameters.AddWithValue("@shares_in_day", shares_in_dayInt);
                
                try
                {
                    con.Open();
                    cmd_member.ExecuteNonQuery();
                    FillDataGridAnnualSeats(con);
                    fillStats(con);
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
                        FillDataGridAnnualSeats(con);
                        fillStats(con);
                        FillDataGridMembers(con);
                        
                        con.Close();

                        // limpar as text boxs
                        member_name.Text = "";
                        member_bi.Text = "";
                        member_nif.Text = "";
                        member_address.Text = "";
                        member_shares_in_day.Text = "";
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

        private void Member_Clear(object sender, RoutedEventArgs e)
        {
            member_name.Text = "";
            member_bi.Text = "";
            member_nif.Text = "";
            member_address.Text = "";
            member_shares_in_day.Text = "";
            member_birth_date.Text = "";
            member_nationality.Text = "";
            member_GenderMale.IsChecked = false;
            member_GenderFemale.IsChecked = false;
            member_shares_value.Value = 0;
            member_number.Text = "";
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *  ##########################----------- ANNUAL SEAT TAB -----------#############################
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void FillDataGridAnnualSeats(SqlConnection con)
        {
            string CmdString = "SELECT * FROM football.udf_annualSeats(DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT)";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("annual_seats");
            sda.Fill(dt);
            annualSeatsGrid.ItemsSource = dt.DefaultView;

            
            // fill the sections of the stadium
            CmdString = "SELECT * FROM football.udf_sections(DEFAULT)";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("sections");
            sda.Fill(dt);

            seat_section.Items.Clear();
            foreach (DataRow section in dt.Rows)
            {
                ComboBoxItem itm = new ComboBoxItem();
                itm.Content = section[0].ToString();
                seat_section.Items.Add(itm);
            }

        }

        private void annualSeatsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)annualSeatsGrid.SelectedItem;
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

                seat_bi.Text = search_bi;

                string search_seat;
                try
                {
                    // este try catch e por causa de quando autalizamos a DataGrid numa segunda vez
                    // e houve algo selecionado antes...
                    search_seat = row.Row.ItemArray[6].ToString();
                }
                catch (Exception)
                {
                    return;
                }

                seat_number.Text = search_seat;

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

                seat_season.Text = search_season;

                string CmdString = "SELECT * FROM football.udf_annualSeats_full(@n_seat, @row, @id_section, @bi, @season)";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                cmd.Parameters.AddWithValue("@n_seat", Convert.ToInt32(search_seat));
                cmd.Parameters.AddWithValue("@row", search_row);
                cmd.Parameters.AddWithValue("@id_section", Convert.ToInt32(search_section));
                cmd.Parameters.AddWithValue("@bi", Convert.ToInt32(search_bi));
                cmd.Parameters.AddWithValue("@season", Convert.ToInt32(search_season));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("annualSeat");
                sda.Fill(dt);
                DataRow r = dt.Rows[0];

                seat_bi.Text = r["bi"].ToString();
                seat_number.Text = r["seat number"].ToString();
                seat_row.Text = r["row"].ToString();
                seat_season.Text = r["season"].ToString();

                DateTime date = DateTime.Parse(r["start_date"].ToString());
                seat_initial_date.Text = date.ToString("yyyy-MM-dd");
                seat_duration.Text = r["duration"].ToString();
                seat_value.Value = Convert.ToDouble(r["value"].ToString());

                String CmdString1 = "SELECT * FROM football.udf_sections_annual(@bi, @n_seat, @row, @id_section, @season)";
                SqlCommand cmd1 = new SqlCommand(CmdString1, con);
                cmd1.Parameters.AddWithValue("@bi", Convert.ToInt32(search_bi));
                cmd1.Parameters.AddWithValue("@n_seat", Convert.ToInt32(search_seat));
                cmd1.Parameters.AddWithValue("@row", search_row);
                cmd1.Parameters.AddWithValue("@id_section", Convert.ToInt32(search_section));
                cmd1.Parameters.AddWithValue("@season", Convert.ToInt32(search_season));

                SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                DataTable dt1 = new DataTable("section_selected");
                sda1.Fill(dt1);

                foreach (ComboBoxItem itm in seat_section.Items)
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

        private void AnnualSeat_New(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int biInt, nSeatInt, sectionidInt, seasonInt, durationInt;

                if (!Int32.TryParse(seat_bi.Text, out biInt))
                {
                    MessageBox.Show("The BI must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(seat_number.Text, out nSeatInt))
                {
                    MessageBox.Show("The Seat Number must be an Integer!");
                    return;
                }

                if (!Int32.TryParse(seat_season.Text, out seasonInt))
                {
                    MessageBox.Show("The Season must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(seat_duration.Text, out durationInt))
                {
                    MessageBox.Show("The Duration must be an Integer!");
                    return;
                }

                if (seat_row.Text.Length == 0)
                {
                    MessageBox.Show("The row can't be blank!");
                    return;
                }

                DateTime dt;
                if (!DateTime.TryParse(seat_initial_date.Text, out dt))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }


                string CmdString1 = "SELECT * FROM football.udf_sections(DEFAULT)";
                SqlCommand cmd = new SqlCommand(CmdString1, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt1 = new DataTable("section_selected");
                sda.Fill(dt1);
                string section_text = seat_section.Text;

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

                // INSERT ANNUAL SEAT

                string CmdString = "football.sp_createAnnualSeat";
                SqlCommand cmd_annualSeat = new SqlCommand(CmdString, con);
                cmd_annualSeat.CommandType = CommandType.StoredProcedure;
                cmd_annualSeat.Parameters.AddWithValue("@bi", biInt);
                cmd_annualSeat.Parameters.AddWithValue("@start_date", dt);
                cmd_annualSeat.Parameters.AddWithValue("@n_seat", nSeatInt);
                cmd_annualSeat.Parameters.AddWithValue("@row", seat_row.Text);
                cmd_annualSeat.Parameters.AddWithValue("@value", seat_value.Value);
                cmd_annualSeat.Parameters.AddWithValue("@id_section", sectionidInt);
                cmd_annualSeat.Parameters.AddWithValue("@season", seasonInt);
                cmd_annualSeat.Parameters.AddWithValue("@duration", durationInt);

                try
                {
                    con.Open();
                    cmd_annualSeat.ExecuteNonQuery();
                    FillDataGridMembers(con);
                    fillStats(con);
                    FillDataGridAnnualSeats(con);

                    con.Close();
                    MessageBox.Show("The annual seat has been inserted successfully!");

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

            }
        }

        private void AnnualSeat_Update(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int biInt, nSeatInt, sectionidInt, seasonInt, durationInt;

                if (!Int32.TryParse(seat_bi.Text, out biInt))
                {
                    MessageBox.Show("The BI must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(seat_number.Text, out nSeatInt))
                {
                    MessageBox.Show("The Seat Number must be an Integer!");
                    return;
                }

                if (!Int32.TryParse(seat_season.Text, out seasonInt))
                {
                    MessageBox.Show("The Season must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(seat_duration.Text, out durationInt))
                {
                    MessageBox.Show("The Duration must be an Integer!");
                    return;
                }

                if (seat_row.Text.Length == 0)
                {
                    MessageBox.Show("The row can't be blank!");
                    return;
                }

                DateTime dt;
                if (!DateTime.TryParse(seat_initial_date.Text, out dt))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }

                string CmdString1 = "SELECT * FROM football.udf_sections(DEFAULT)";
                SqlCommand cmd = new SqlCommand(CmdString1, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt1 = new DataTable("section_selected");
                sda.Fill(dt1);
                string section_text = seat_section.Text;

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


                // UPDATE ANNUAL SEAT

                string CmdString = "football.sp_modifyAnnualSeat";
                SqlCommand cmd_annualSeat = new SqlCommand(CmdString, con);
                cmd_annualSeat.CommandType = CommandType.StoredProcedure;
                cmd_annualSeat.Parameters.AddWithValue("@bi", biInt);
                cmd_annualSeat.Parameters.AddWithValue("@start_date", dt);
                cmd_annualSeat.Parameters.AddWithValue("@n_seat", nSeatInt);
                cmd_annualSeat.Parameters.AddWithValue("@row", seat_row.Text);
                cmd_annualSeat.Parameters.AddWithValue("@value", seat_value.Value);
                cmd_annualSeat.Parameters.AddWithValue("@id_section", sectionidInt);
                cmd_annualSeat.Parameters.AddWithValue("@season", seasonInt);
                cmd_annualSeat.Parameters.AddWithValue("@duration", durationInt);

                try
                {
                    con.Open();
                    cmd_annualSeat.ExecuteNonQuery();
                    FillDataGridMembers(con);
                    fillStats(con);
                    FillDataGridAnnualSeats(con);

                    con.Close();
                    MessageBox.Show("The Annual Seat has been updated successfully!");

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

            }
        }

        private void AnnualSeat_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using (con = new SqlConnection(ConString))
                {
                    // --> Validations
                    int biInt, nSeatInt, sectionidInt, seasonInt;

                    // bi, seat number, section id and season are number
                    if (!Int32.TryParse(seat_bi.Text, out biInt))
                    {
                        MessageBox.Show("The BI must be an Integer!");
                        return;
                    }
                    if (!Int32.TryParse(seat_number.Text, out nSeatInt))
                    {
                        MessageBox.Show("The Seat Number must be an Integer!");
                        return;
                    }

                    if (!Int32.TryParse(seat_season.Text, out seasonInt))
                    {
                        MessageBox.Show("The Season must be an Integer!");
                        return;
                    }

                    if (seat_row.Text.Length == 0)
                    {
                        MessageBox.Show("The row can't be blank!");
                        return;
                    }

                    string CmdString1 = "SELECT * FROM football.udf_sections(DEFAULT)";
                    SqlCommand cmd = new SqlCommand(CmdString1, con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt1 = new DataTable("section_selected");
                    sda.Fill(dt1);
                    string section_text = seat_section.Text;

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


                    // DELETE THE ANNUAL SEAT

                    string CmdString = "football.sp_deleteAnnualSeat";
                    SqlCommand cmd_annualSeat = new SqlCommand(CmdString, con);
                    cmd_annualSeat.CommandType = CommandType.StoredProcedure;
                    cmd_annualSeat.Parameters.AddWithValue("@bi", biInt);
                    cmd_annualSeat.Parameters.AddWithValue("@n_seat", nSeatInt);
                    cmd_annualSeat.Parameters.AddWithValue("@season", seasonInt);
                    cmd_annualSeat.Parameters.AddWithValue("@row", seat_row.Text);
                    cmd_annualSeat.Parameters.AddWithValue("@id_section", sectionidInt);

                    try
                    {
                        con.Open();
                        cmd_annualSeat.ExecuteNonQuery();
                        FillDataGridMembers(con);
                        fillStats(con);
                        FillDataGridAnnualSeats(con);

                        con.Close();

                        // limpar as text boxs
                        seat_bi.Text = "";
                        seat_number.Text = "";
                        seat_row.Text = "";
                        seat_section.Text = "";
                        seat_value.Value = 0;
                        seat_duration.Text = "";
                        seat_initial_date.Text = "";
                        seat_season.Text = "";

                        MessageBox.Show("The annual seat has been deleted successfully!");

                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }

                }
            }
        }
        private void AnnualSeat_Clear(object sender, RoutedEventArgs e)
        {
            // limpar as text boxs
            seat_bi.Text = "";
            seat_number.Text = "";
            seat_row.Text = "";
            seat_section.Text = "";
            seat_value.Value = 0;
            seat_duration.Text = "";
            seat_initial_date.Text = "";
            seat_season.Text = "";
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *  ##########################----------- STATS  TAB -----------############################
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void fillStats(SqlConnection con)
        {
            string CmdString = "SELECT * FROM football.udf_members_stats()";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("stats");
            sda.Fill(dt);

            foreach (DataRow counts in dt.Rows)
            {
                if (counts["name"].ToString() == "shares_in_day_false")
                {
                    without_shares_in_day.Text = counts["result"].ToString();
                }
                else if (counts["name"].ToString() == "average_shares")
                {
                    average_shares_value.Text = counts["result"].ToString() + "$";
                }
                else if (counts["name"].ToString() == "total_of_members")
                {
                    number_of_club_members.Text = counts["result"].ToString();
                }
                else if (counts["name"].ToString() == "total_of_annual_seats")
                {
                    number_of_annual_seats.Text = counts["result"].ToString();
                }
              
            }

            // number annual seats per season
            CmdString = "SELECT * FROM football.udf_annual_seats_per_season_count()";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("number_of_annual_seats_per_season");
            sda.Fill(dt);
            number_of_annual_seats_per_season.ItemsSource = dt.DefaultView;

            // next birthday
            CmdString = "SELECT * FROM football.udf_members_stats_next_birthday()";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("next_birthday");
            sda.Fill(dt);
            next_birthday.ItemsSource = dt.DefaultView;
        }
    }
}

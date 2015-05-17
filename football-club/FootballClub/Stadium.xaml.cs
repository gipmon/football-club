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
    /// Interaction logic for Stadium.xaml
    /// </summary>
    public partial class Stadium : Page
    {

        private string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
        private SqlConnection con;
        private string section_id;

        public Stadium()
        {
            InitializeComponent();
            using (con = new SqlConnection(ConString))
            {
                FillDataGridSpots(con);
                FillDataGridSections(con);
                fillStats(con);

            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *  ##########################----------- SPOTS TAB -----------#############################
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void FillDataGridSpots(SqlConnection con)
        {
            string CmdString = "SELECT * FROM football.udf_spots(DEFAULT, DEFAULT, DEFAULT) ORDER BY 'section name' ASC";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("spots");
            sda.Fill(dt);
            spotsGrid.ItemsSource = dt.DefaultView;


            // fill the sections of the stadium
            CmdString = "SELECT * FROM football.udf_sections(DEFAULT)";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("sections");
            sda.Fill(dt);

            spot_section.Items.Clear();
            foreach (DataRow section in dt.Rows)
            {
                ComboBoxItem itm = new ComboBoxItem();
                itm.Content = section[0].ToString();
                spot_section.Items.Add(itm);
            }

        }

        private void spotsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)spotsGrid.SelectedItem;

                string search_spot;
                try
                {
                    // este try catch e por causa de quando autalizamos a DataGrid numa segunda vez
                    // e houve algo selecionado antes...
                    search_spot = row.Row.ItemArray[2].ToString();
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
                    search_row = row.Row.ItemArray[3].ToString();
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
                    search_section = row.Row.ItemArray[1].ToString();
                }
                catch (Exception)
                {
                    return;
                }


                string CmdString = "SELECT * FROM football.udf_spots(@n_spot, @row, @id_section)";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                cmd.Parameters.AddWithValue("@n_spot", Convert.ToInt32(search_spot));
                cmd.Parameters.AddWithValue("@row", search_row);
                cmd.Parameters.AddWithValue("@id_section", Convert.ToInt32(search_section));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("spot");
                sda.Fill(dt);
                DataRow r = dt.Rows[0];

                spot_number.Text = r["spot number"].ToString();
                spot_row.Text = r["row"].ToString();

                String CmdString1 = "SELECT * FROM football.udf_sections_spots(@n_spot, @row, @id_section)";
                SqlCommand cmd1 = new SqlCommand(CmdString1, con);
                cmd1.Parameters.AddWithValue("@n_spot", Convert.ToInt32(search_spot));
                cmd1.Parameters.AddWithValue("@row", search_row);
                cmd1.Parameters.AddWithValue("@id_section", Convert.ToInt32(search_section));

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

        private void Spot_New(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int nSpotInt, sectionidInt;

                if (!Int32.TryParse(spot_number.Text, out nSpotInt))
                {
                    MessageBox.Show("The Spot Number must be an Integer!");
                    return;
                }

                if (spot_row.Text.Length == 0)
                {
                    MessageBox.Show("The row can't be blank!");
                    return;
                }

                string CmdString1 = "SELECT * FROM football.udf_sections(DEFAULT)";
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

                // INSERT SPOT

                string CmdString = "football.sp_createSpot";
                SqlCommand cmd_spot = new SqlCommand(CmdString, con);
                cmd_spot.CommandType = CommandType.StoredProcedure;
                cmd_spot.Parameters.AddWithValue("@n_spot", nSpotInt);
                cmd_spot.Parameters.AddWithValue("@row", spot_row.Text);
                cmd_spot.Parameters.AddWithValue("@id_section", sectionidInt);

                try
                {
                    con.Open();
                    cmd_spot.ExecuteNonQuery();
                    FillDataGridSections(con);
                    FillDataGridSpots(con);
                    fillStats(con);

                    
                    con.Close();
                    MessageBox.Show("The spot has been inserted successfully!");

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

            }
        }


        private void Spot_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using (con = new SqlConnection(ConString))
                {
                    // --> Validations
                    int nSpotInt, sectionidInt;

                    // spot number and section id are number
                    
                    if (!Int32.TryParse(spot_number.Text, out nSpotInt))
                    {
                        MessageBox.Show("The Spot Number must be an Integer!");
                        return;
                    }

                    if (spot_row.Text.Length == 0)
                    {
                        MessageBox.Show("The row can't be blank!");
                        return;
                    }

                    string CmdString1 = "SELECT * FROM football.udf_sections(DEFAULT)";
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


                    // DELETE THE SPOT

                    string CmdString = "football.sp_deleteSpot";
                    SqlCommand cmd_spot = new SqlCommand(CmdString, con);
                    cmd_spot.CommandType = CommandType.StoredProcedure;
                    cmd_spot.Parameters.AddWithValue("@n_spot", nSpotInt);
                    cmd_spot.Parameters.AddWithValue("@row", spot_row.Text);
                    cmd_spot.Parameters.AddWithValue("@id_section", sectionidInt);

                    try
                    {
                        con.Open();
                        cmd_spot.ExecuteNonQuery();
                        FillDataGridSections(con);
                        FillDataGridSpots(con);
                        fillStats(con);


                        con.Close();

                        // limpar as text boxs
                        spot_number.Text = "";
                        spot_row.Text = "";
                        spot_section.Text = "";

                        MessageBox.Show("The spot has been deleted successfully!");

                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }

                }
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
        *  ##########################----------- SECTIONS TAB -----------#############################
        * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void FillDataGridSections(SqlConnection con)
        {
            string CmdString = "SELECT * FROM football.udf_sections(DEFAULT);";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("sections");
            sda.Fill(dt);
            sectionsGrid.ItemsSource = dt.DefaultView;

        }
        
        private void sectionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)sectionsGrid.SelectedItem;

                string search_id;
                try
                {
                    // este try catch e por causa de quando autalizamos a DataGrid numa segunda vez
                    // e houve algo selecionado antes...
                    search_id = row.Row.ItemArray[1].ToString();
                }
                catch (Exception)
                {
                    return;
                }

                sections_id.Text = search_id;


                string CmdString = "SELECT * FROM football.udf_sections(@id_section)";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                cmd.Parameters.AddWithValue("@id_section", Convert.ToInt32(search_id));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("section");
                sda.Fill(dt);
                DataRow r = dt.Rows[0];

                sections_type.Text = r["section name"].ToString();


            }
        }

        
        private void Section_New(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {


                if (sections_type.Text.Length == 0)
                {
                    MessageBox.Show("The type can't be blank!");
                    return;
                }

                // INSERT SECTION 

                string CmdString = "football.sp_createSection";
                SqlCommand cmd_section = new SqlCommand(CmdString, con);
                cmd_section.CommandType = CommandType.StoredProcedure;
                cmd_section.Parameters.AddWithValue("@type", sections_type.Text);

                try
                {
                    con.Open();
                    cmd_section.ExecuteNonQuery();
                    FillDataGridSpots(con);
                    FillDataGridSections(con);
                    fillStats(con);

                    con.Close();
                    MessageBox.Show("The section has been inserted successfully!");

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

            }
        }

        private void Section_Update(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int sectionidInt;

                if (!Int32.TryParse(sections_id.Text, out sectionidInt))
                {
                    MessageBox.Show("The Section ID must be an Integer!");
                    return;
                }


                if (sections_type.Text.Length == 0)
                {
                    MessageBox.Show("The type can't be blank!");
                    return;
                }


                // UPDATE SECTION

                string CmdString = "football.sp_modifySection";
                SqlCommand cmd_section = new SqlCommand(CmdString, con);
                cmd_section.CommandType = CommandType.StoredProcedure;
                cmd_section.Parameters.AddWithValue("@type", sections_type.Text);
                cmd_section.Parameters.AddWithValue("@id_section", sectionidInt);

                try
                {
                    con.Open();
                    cmd_section.ExecuteNonQuery();
                    FillDataGridSpots(con);
                    FillDataGridSections(con);
                    fillStats(con);

                    con.Close();
                    MessageBox.Show("The section has been updated successfully!");

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

            }
        }



        private void Section_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using (con = new SqlConnection(ConString))
                {
                    // --> Validations
                    int sectionidInt;

                    // section id are number

                    if (!Int32.TryParse(sections_id.Text, out sectionidInt))
                    {
                        MessageBox.Show("The Section ID must be an Integer!");
                        return;
                    }

            
                    // DELETE THE SECTION

                    string CmdString = "football.sp_deleteSection";
                    SqlCommand cmd_section = new SqlCommand(CmdString, con);
                    cmd_section.CommandType = CommandType.StoredProcedure;
                    cmd_section.Parameters.AddWithValue("@id_section", sectionidInt);

                    try
                    {
                        con.Open();
                        cmd_section.ExecuteNonQuery();
                        FillDataGridSpots(con);

                        FillDataGridSections(con);
                        fillStats(con);

                        con.Close();

                        // limpar as text boxs
                        sections_id.Text = "";
                        sections_type.Text = "";

                        MessageBox.Show("The section has been deleted successfully!");

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
        private void fillStats(SqlConnection con)
        {
            string CmdString = "SELECT * FROM football.udf_stadium_stats()";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("stats");
            sda.Fill(dt);

            foreach (DataRow counts in dt.Rows)
            {
                if (counts["name"].ToString() == "total_Sections")
                {
                    total_sections.Text = counts["result"].ToString();
                }
                else if (counts["name"].ToString() == "total_of_seats")
                {
                    total_seats.Text = counts["result"].ToString();
                }
              
            }

            // number of seats per section
            CmdString = "SELECT * FROM football.udf_seats_per_section_count()";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("number_seats_per_section");
            sda.Fill(dt);
            number_seats_per_section.ItemsSource = dt.DefaultView;

            // number of annual seats per section
            CmdString = "SELECT * FROM football.udf_annual_seats_per_section_count()";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("number_annual_seats_per_section");
            sda.Fill(dt);
            number_of_annual_seats_per_section.ItemsSource = dt.DefaultView;
        }
        
    }
}

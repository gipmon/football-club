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

        public Practice()
        {
            InitializeComponent();
            using (con = new SqlConnection(ConString))
            {
                FillDataGridCourts(con);
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
                    cmd_court.ExecuteNonQuery(); ;
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
    }
}

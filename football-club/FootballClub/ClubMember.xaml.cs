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
        public ClubMember()
        {
            InitializeComponent();
            using (con = new SqlConnection(ConString))
            {
                FillDataGridMembers(con);
                
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
    }
}

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
    /// Interaction logic for Staff.xaml
    /// </summary>
    public partial class Staff : Page
    {
        private string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
        private SqlConnection con;
        private string dep_id;

        public Staff()
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
            * STAFF TAB
            * */
            string CmdString = "SELECT * FROM football.udf_staff_data_grid()";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("staff");
            sda.Fill(dt);
            staffGrid.ItemsSource = dt.DefaultView;

            // fill the departments of the staff
            CmdString = "SELECT * FROM football.udf_department_names(DEFAULT)";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("departments");
            sda.Fill(dt);
            foreach (DataRow department in dt.Rows)
            {
                ComboBoxItem itm = new ComboBoxItem();
                itm.Content = department["department_name"].ToString();
                departments.Items.Add(itm);
            }

            /*
            * DEPARTMENTS TAB
            * */
            string CmdString1 = "SELECT * FROM football.departmentsView";
            SqlCommand cmd1 = new SqlCommand(CmdString1, con);
            SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
            DataTable dt1 = new DataTable("department");
            sda.Fill(dt1);
            departmentGrid.ItemsSource = dt1.DefaultView;

        }

        private void staffGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)staffGrid.SelectedItem;
                string search_bi;
                try
                {
                    search_bi = row.Row.ItemArray[1].ToString();
                }
                catch (Exception)
                {
                    return;
                }
                bi.Text = search_bi;
                string CmdString = "SELECT * FROM football.udf_staff(@intBi)";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                cmd.Parameters.AddWithValue("@intBi", Convert.ToInt32(search_bi));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("staff");
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
                salary.Value = Convert.ToDouble(r["salary"].ToString());
                internal_id.Text = r["internal id"].ToString();
                role.Text = r["role"].ToString();

                // select the department of the staff
                CmdString = "SELECT * FROM football.udf_department_names(@intBi)";
                cmd = new SqlCommand(CmdString, con);
                cmd.Parameters.AddWithValue("@intBi", Convert.ToInt32(search_bi));
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable("department_selected");
                sda.Fill(dt);


                foreach (ComboBoxItem itm in departments.Items)
                {
                    itm.IsSelected = false;
                    foreach (DataRow department in dt.Rows)
                    {
                        if (department[0].ToString() == itm.Content.ToString())
                        {
                            dep_id = department[1].ToString();
                            itm.IsSelected = true;
                            break;
                        }
                    }
                }
                
            }

        }

        private void Staff_New(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int biInt, nifInt, depInt;

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

              
                
                DateTime dt;
                if (!DateTime.TryParse(birth_date.Text, out dt))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }

                string gender;
                gender = (GenderFemale.IsChecked == true) ? "F" : "M";

                string CmdString1 = "SELECT * FROM football.udf_department_names(DEFAULT)";
                SqlCommand cmd = new SqlCommand(CmdString1, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt1 = new DataTable("department_selected");
                sda.Fill(dt1);
                string dep = departments.Text;

                foreach (ComboBoxItem itm in departments.Items)
                {
                    itm.IsSelected = false;
                    foreach (DataRow department in dt1.Rows)
                    {
                        if (department[0].ToString() == dep)
                        {
                            dep_id = department[1].ToString();
                            itm.IsSelected = true;
                            break;
                        }
                    }
                }

                if (!Int32.TryParse(dep_id, out depInt))
                {
                    MessageBox.Show("The dep must be an Integer!");
                    return;
                }


                string CmdString = "EXEC football.sp_createStaff @bi = @paramBi, @name = @paramName, @address = @paramAddress, @birth_date = @paramBirthDate, @nif = @paramNif, @gender = @paramGender, @nationality = @paramNationality, @salary = @paramSalary, @department_id = @paramDepartment, @role = @paramRole";
                SqlCommand cmd_player = new SqlCommand(CmdString, con);
                cmd_player.Parameters.AddWithValue("@paramBi", biInt);
                cmd_player.Parameters.AddWithValue("@paramName", name.Text);
                cmd_player.Parameters.AddWithValue("@paramAddress", address.Text);
                cmd_player.Parameters.AddWithValue("@paramBirthDate", dt);
                cmd_player.Parameters.AddWithValue("@paramNif", nifInt);
                cmd_player.Parameters.AddWithValue("@paramGender", gender);
                cmd_player.Parameters.AddWithValue("@paramNationality", nationality.Text);
                cmd_player.Parameters.AddWithValue("@paramSalary", salary.Value);
                cmd_player.Parameters.AddWithValue("@paramDepartment", depInt);
                cmd_player.Parameters.AddWithValue("@paramRole", role.Text);

                try
                {
                    con.Open();
                    cmd_player.ExecuteNonQuery();
                    MessageBox.Show("The staff has been inserted successfully!");
                    FillDataGrid(con);
                    con.Close();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

            }


        }

        private void departmentGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)departmentGrid.SelectedItem;
                string search_id = row.Row.ItemArray[0].ToString();
                department_id.Text = search_id;
                string CmdString = "SELECT * FROM football.departmentsView WHERE id=" + search_id;
                SqlCommand cmd = new SqlCommand(CmdString, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("departments");
                sda.Fill(dt);
                DataRow r = dt.Rows[0];

                department_id.Text = r["id"].ToString();
                department_name.Text = r["name"].ToString();
                department_address.Text = r["address"].ToString();

            }

        }
    }
}

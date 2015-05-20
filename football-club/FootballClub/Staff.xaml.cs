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
                FillDataGridStaff(con);
                FillDataGridDepartments(con);
                fillStats(con);
            }

        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
       *  ##########################----------- STAFF TAB -----------##########################
       * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private void FillDataGridStaff(SqlConnection con)
        {
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
            staff_departments.Items.Clear();
            //MessageBox.Show("clear");
            foreach (DataRow department in dt.Rows)
            {
                ComboBoxItem itm = new ComboBoxItem();
                itm.Content = department[0].ToString();
                staff_departments.Items.Add(itm);
            }

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
                staff_bi.Text = search_bi;
                string CmdString = "SELECT * FROM football.udf_staff(@staff_bi)";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                cmd.Parameters.AddWithValue("@staff_bi", Convert.ToInt32(search_bi));
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("staff");
                sda.Fill(dt);
                DataRow r = dt.Rows[0];

                staff_bi.Text = r["bi"].ToString();
                staff_name.Text = r["name"].ToString();
                staff_nif.Text = r["nif"].ToString();
                staff_address.Text = r["address"].ToString();
                if (r["gender"].ToString() == "F")
                {
                    staff_GenderFemale.IsChecked = true;
                }
                else
                {
                    staff_GenderMale.IsChecked = true;
                }
                DateTime date = DateTime.Parse(r["birth date"].ToString());
                staff_birth_date.Text = date.ToString("yyyy-MM-dd");
                staff_nationality.Text = r["nationality"].ToString();
                staff_salary.Value = Convert.ToDouble(r["salary"].ToString());
                staff_internal_id.Text = r["internal id"].ToString();
                staff_role.Text = r["role"].ToString();

                String CmdString1 = "SELECT * FROM football.udf_department_names(@staff_bi)";
                SqlCommand cmd1 = new SqlCommand(CmdString1, con);
                cmd1.Parameters.AddWithValue("@staff_bi", Convert.ToInt32(search_bi));
                SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                DataTable dt1 = new DataTable("department_selected");
                sda1.Fill(dt1);

                foreach (ComboBoxItem itm in staff_departments.Items)
                {
                    itm.IsSelected = false;
                    foreach (DataRow department in dt1.Rows)
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

                if (!Int32.TryParse(staff_bi.Text, out biInt))
                {
                    MessageBox.Show("The BI must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(staff_nif.Text, out nifInt))
                {
                    MessageBox.Show("The NIF must be an Integer!");
                    return;
                }

                 if (staff_name.Text.Length == 0)
                {
                    MessageBox.Show("The name can't be blank!");
                    return;
                }
                if (staff_address.Text.Length == 0)
                {
                    MessageBox.Show("The address can't be blank!");
                    return;
                }
                if (staff_nationality.Text.Length == 0)
                {
                    MessageBox.Show("The nationality can't be blank!");
                    return;
                }

                DateTime dt;
                if (!DateTime.TryParse(staff_birth_date.Text, out dt))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }

                string gender;
                if (staff_GenderFemale.IsChecked == true)
                {
                    gender = "F";
                }
                else if (staff_GenderMale.IsChecked == true)
                {
                    gender = "M";
                }
                else
                {
                    MessageBox.Show("Please select the gender!");
                    return;
                }

                string CmdString1 = "SELECT * FROM football.udf_department_names(DEFAULT)";
                SqlCommand cmd = new SqlCommand(CmdString1, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt1 = new DataTable("department_selected");
                sda.Fill(dt1);
                string dep = staff_departments.Text;

                foreach (DataRow department in dt1.Rows)
                {
                    if (department[0].ToString() == dep)
                    {
                        dep_id = department[1].ToString();
                        break;
                    }
                }
                

                if (!Int32.TryParse(dep_id, out depInt))
                {
                    MessageBox.Show("The Department must be valid!");
                    return;
                }

                // INSERT STAFF

                string CmdString = "football.sp_createStaff";
                SqlCommand cmd_staff = new SqlCommand(CmdString, con);
                cmd_staff.CommandType = CommandType.StoredProcedure;
                cmd_staff.Parameters.AddWithValue("@bi", biInt);
                cmd_staff.Parameters.AddWithValue("@name", staff_name.Text);
                cmd_staff.Parameters.AddWithValue("@address", staff_address.Text);
                cmd_staff.Parameters.AddWithValue("@birth_date", dt);
                cmd_staff.Parameters.AddWithValue("@nif", nifInt);
                cmd_staff.Parameters.AddWithValue("@gender", gender);
                cmd_staff.Parameters.AddWithValue("@nationality", staff_nationality.Text);
                cmd_staff.Parameters.AddWithValue("@salary", staff_salary.Value);
                cmd_staff.Parameters.AddWithValue("@department_id", depInt);
                cmd_staff.Parameters.AddWithValue("@role", staff_role.Text);

                try
                {
                    con.Open();
                    cmd_staff.ExecuteNonQuery();
                    FillDataGridDepartments(con);
                    fillStats(con);
                    FillDataGridStaff(con);

                    con.Close();
                    MessageBox.Show("The staff has been inserted successfully!");

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

            }


        }

        private void Staff_Update(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                 // --> Validations
                int biInt, nifInt, depInt;
                
                if (!Int32.TryParse(staff_bi.Text, out biInt))
                {
                    MessageBox.Show("The BI must be an Integer!");
                    return;
                }
                if (!Int32.TryParse(staff_nif.Text, out nifInt))
                {
                    MessageBox.Show("The NIF must be an Integer!");
                    return;
                }

                 if (staff_name.Text.Length == 0)
                {
                    MessageBox.Show("The name can't be blank!");
                    return;
                }
                if (staff_address.Text.Length == 0)
                {
                    MessageBox.Show("The address can't be blank!");
                    return;
                }
                if (staff_nationality.Text.Length == 0)
                {
                    MessageBox.Show("The nationality can't be blank!");
                    return;
                }

                DateTime dt;
                if (!DateTime.TryParse(staff_birth_date.Text, out dt))
                {
                    MessageBox.Show("Please insert a valid date!");
                    return;
                }

                string gender;
                if (staff_GenderFemale.IsChecked == true)
                {
                    gender = "F";
                }
                else if (staff_GenderMale.IsChecked == true)
                {
                    gender = "M";
                }
                else
                {
                    MessageBox.Show("Please select the gender!");
                    return;
                }
                string CmdString1 = "SELECT * FROM football.udf_department_names(DEFAULT)";
                SqlCommand cmd = new SqlCommand(CmdString1, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt1 = new DataTable("department_selected");
                sda.Fill(dt1);
                string dep = staff_departments.Text;

                foreach (DataRow department in dt1.Rows)
                {
                    if (department[0].ToString() == dep)
                    {
                        dep_id = department[1].ToString();
                        break;
                    }
                }
                

                if (!Int32.TryParse(dep_id, out depInt))
                {
                    MessageBox.Show("The Dpartment must be Valid!");
                    return;
                }


                // UPDATE STAFF

                string CmdString = "football.sp_modifyStaff";
                SqlCommand cmd_staff = new SqlCommand(CmdString, con);
                cmd_staff.CommandType = CommandType.StoredProcedure;
                cmd_staff.Parameters.AddWithValue("@bi", biInt);
                cmd_staff.Parameters.AddWithValue("@name", staff_name.Text);
                cmd_staff.Parameters.AddWithValue("@address", staff_address.Text);
                cmd_staff.Parameters.AddWithValue("@birth_date", dt);
                cmd_staff.Parameters.AddWithValue("@gender", gender);
                cmd_staff.Parameters.AddWithValue("@nationality", staff_nationality.Text);
                cmd_staff.Parameters.AddWithValue("@salary", staff_salary.Value);
                cmd_staff.Parameters.AddWithValue("@department_id", depInt);
                cmd_staff.Parameters.AddWithValue("@role", staff_role.Text);

                try
                {
                    con.Open();
                    cmd_staff.ExecuteNonQuery();
                    FillDataGridDepartments(con);
                    fillStats(con);
                    FillDataGridStaff(con);


                    con.Close();
                    MessageBox.Show("The staff has been updated successfully!");

                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

            }
        }

        private void Staff_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using (con = new SqlConnection(ConString))
                {
                    // --> Validations
                    int biInt;

                    // bi is number
                    if (!Int32.TryParse(staff_bi.Text, out biInt))
                    {
                        MessageBox.Show("The BI must be an Integer!");
                        return;
                    }

                    // DELETE THE STAFF

                    string CmdString = "football.sp_deleteStaff";
                    SqlCommand cmd_staff = new SqlCommand(CmdString, con);
                    cmd_staff.CommandType = CommandType.StoredProcedure;
                    cmd_staff.Parameters.AddWithValue("@bi", biInt);

                    try
                    {
                        con.Open();
                        cmd_staff.ExecuteNonQuery();
                        FillDataGridDepartments(con);
                        fillStats(con);
                        FillDataGridStaff(con);


                        con.Close();

                        // limpar as text boxs
                        staff_name.Text = "";
                        staff_bi.Text = "";
                        staff_nif.Text = "";
                        staff_address.Text = "";
                        staff_departments.Text = "";
                        staff_role.Text = "";
                        staff_birth_date.Text = "";
                        staff_nationality.Text = "";
                        staff_GenderMale.IsChecked = false;
                        staff_GenderFemale.IsChecked = false;
                        staff_salary.Value = 0;
                        staff_internal_id.Text = "";
                        MessageBox.Show("The staff has been deleted successfully!");

                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }

                }
            }
        }

        private void Staff_Clear(object sender, RoutedEventArgs e)
        {
            // limpar as text boxs
            staff_name.Text = "";
            staff_bi.Text = "";
            staff_nif.Text = "";
            staff_address.Text = "";
            staff_departments.Text = "";
            staff_role.Text = "";
            staff_birth_date.Text = "";
            staff_nationality.Text = "";
            staff_GenderMale.IsChecked = false;
            staff_GenderFemale.IsChecked = false;
            staff_salary.Value = 0;
            staff_internal_id.Text = "";
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *  ##########################----------- DEPARTMENT TAB -----------#############################
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void FillDataGridDepartments(SqlConnection con)
        {
            string CmdString = "SELECT * FROM football.udf_departments(DEFAULT)";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("departments");
            sda.Fill(dt);
            departmentGrid.ItemsSource = dt.DefaultView;
        }

        private void departmentGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                DataRowView row = (DataRowView)departmentGrid.SelectedItem;
                string search_id;
                try
                {
                    search_id = row.Row.ItemArray[1].ToString();
                }
                catch(Exception)
                {
                    return;
                }
                department_id.Text = search_id;
                string CmdString = "SELECT * FROM football.udf_departments(@department_id)";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                cmd.Parameters.AddWithValue("@department_id", search_id);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("department");
                sda.Fill(dt);
                DataRow r = dt.Rows[0];

                department_id.Text = r["department_id"].ToString();
                department_name.Text = r["department_name"].ToString();
                department_address.Text = r["address"].ToString();

            }

        }

        private void Department_New(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {

                // validation: name and address can't not be lenght = 0
                if (department_name.Text.Length == 0)
                {
                    MessageBox.Show("The department name can't be blank!");
                    return;
                }

                if (department_address.Text.Length == 0)
                {
                    MessageBox.Show("The department address can't be blank!");
                    return;
                }

                string CmdString = "football.sp_createDepartment";
                SqlCommand cmd_department = new SqlCommand(CmdString, con);
                cmd_department.CommandType = CommandType.StoredProcedure;
                cmd_department.Parameters.AddWithValue("@name", department_name.Text);
                cmd_department.Parameters.AddWithValue("@address", department_address.Text);

                try
                {
                    con.Open();
                    cmd_department.ExecuteNonQuery();
                    FillDataGridStaff(con);
                    fillStats(con);
                    FillDataGridDepartments(con);


                    con.Close();
                    MessageBox.Show("The department has been inserted successfully!");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        private void Department_Update(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int idInt;

                // id is number
                if (!Int32.TryParse(department_id.Text, out idInt))
                {
                    MessageBox.Show("The Department ID must be an Integer!");
                    return;
                }

                string CmdString = "football.sp_modifyDepartment";
                SqlCommand cmd_department = new SqlCommand(CmdString, con);
                cmd_department.CommandType = CommandType.StoredProcedure;
                cmd_department.Parameters.AddWithValue("@name", department_name.Text);
                cmd_department.Parameters.AddWithValue("@department_id", idInt);

                cmd_department.Parameters.AddWithValue("@address", department_address.Text);

                try
                {
                    con.Open();
                    cmd_department.ExecuteNonQuery();
                    FillDataGridStaff(con);
                    fillStats(con);
                    FillDataGridDepartments(con);


                    con.Close();
                    MessageBox.Show("The department has been updated successfully!");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        private void Department_Delete(object sender, RoutedEventArgs e)
        {
            using (con = new SqlConnection(ConString))
            {
                // --> Validations
                int idInt;

                // id is number
                if (!Int32.TryParse(department_id.Text, out idInt))
                {
                    MessageBox.Show("The Department ID must be an Integer!");
                    return;
                }
                string CmdString = "football.sp_deleteDepartment";
                SqlCommand cmd_department = new SqlCommand(CmdString, con);
                cmd_department.CommandType = CommandType.StoredProcedure;
                cmd_department.Parameters.AddWithValue("@department_id", idInt);

                try
                {
                    con.Open();
                    cmd_department.ExecuteNonQuery();
                    FillDataGridStaff(con);
                    fillStats(con);
                    FillDataGridDepartments(con);

                    con.Close();
                    MessageBox.Show("The department has been deleted successfully!");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *  ##########################----------- STATS  TAB -----------############################
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void fillStats(SqlConnection con)
        {
            string CmdString = "SELECT * FROM football.udf_staff_department_stats()";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("stats");
            sda.Fill(dt);

            foreach (DataRow counts in dt.Rows)
            {
                if (counts["name"].ToString() == "bigger_nacionality")
                {
                    bigger_nacionality.Text = counts["result"].ToString();
                }
                else if (counts["name"].ToString() == "total_salary_of_staff")
                {
                    salaries_by_staff.Text = counts["result"].ToString() + "$";
                }
                else if (counts["name"].ToString() == "total_of_departments")
                {
                    total_departments.Text = counts["result"].ToString();
                }
                else if (counts["name"].ToString() == "average_age_of_staff")
                {
                    average_age.Text = counts["result"].ToString();
                }
              
            }

            // number_staff_per_department
            CmdString = "SELECT * FROM football.udf_staff_department_count()";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("number_staff_per_department");
            sda.Fill(dt);
            number_staff_per_department.ItemsSource = dt.DefaultView;

            // next birthday
            CmdString = "SELECT * FROM football.udf_staff_department_stats_next_birthday()";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("next_birthday");
            sda.Fill(dt);
            next_birthday.ItemsSource = dt.DefaultView;
        }
    }
}

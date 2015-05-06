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

        public Staff()
        {
            InitializeComponent();
            using (con = new SqlConnection(ConString))
            {
                FillDataGrid();
            }

        }

        private void FillDataGrid()
        {
            string CmdString = "SELECT * FROM football.staffView";
            SqlCommand cmd = new SqlCommand(CmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("staff");
            sda.Fill(dt);
            staffGrid.ItemsSource = dt.DefaultView;

            // fill the departments of the staff
            CmdString = "SELECT * FROM football.departmentsView";
            cmd = new SqlCommand(CmdString, con);
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable("departments");
            sda.Fill(dt);
            foreach (DataRow department in dt.Rows)
            {
                ComboBoxItem itm = new ComboBoxItem();
                itm.Content = department["name"].ToString();
                departments.Items.Add(itm);
            }

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
                string search_bi = row.Row.ItemArray[1].ToString();
                bi.Text = search_bi;
                string CmdString = "SELECT * FROM football.individualStaffView WHERE bi=" + search_bi;
                SqlCommand cmd = new SqlCommand(CmdString, con);
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
                CmdString = "SELECT * FROM football.staffDepartmentView WHERE bi=" + search_bi;
                cmd = new SqlCommand(CmdString, con);
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable("department_selected");
                sda.Fill(dt);

                foreach (ComboBoxItem itm in departments.Items)
                {
                    itm.IsSelected = false;
                    foreach (DataRow department in dt.Rows)
                    {
                        if (department["name"].ToString() == itm.Content.ToString())
                        {
                            itm.IsSelected = true;
                            break;
                        }
                    }
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

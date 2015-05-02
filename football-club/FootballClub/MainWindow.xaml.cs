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
using MahApps.Metro.Controls;

namespace FootballClub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Staff(object sender, RoutedEventArgs e)
        {
            Staff staff = new Staff();
            this.NavigateTo(staff);
        }

        private void Button_Player(object sender, RoutedEventArgs e)
        {
            Player player = new Player();
            this.NavigateTo(player);
        }

        private void Button_ClubMember(object sender, RoutedEventArgs e)
        {
            ClubMember clubMember = new ClubMember();
            this.NavigateTo(clubMember);
        }

        private void Button_Practices(object sender, RoutedEventArgs e)
        {
            Practice practice = new Practice();
            this.NavigateTo(practice);
        }

        private void Button_Stadium(object sender, RoutedEventArgs e)
        {
            Stadium stadium = new Stadium();
            this.NavigateTo(stadium);
        }

        private void Button_Settings(object sender, RoutedEventArgs e) { }

        public void NavigateTo(object o)
        {
            Frame1.Navigate(o);
        }

       
    }
}

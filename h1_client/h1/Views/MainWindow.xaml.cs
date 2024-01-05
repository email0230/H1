using h1.Models;
using h1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace h1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //DBMethods.DebugMethod();
        }

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			AddUserWindow addUserWindow = new AddUserWindow();
			addUserWindow.Show();
		}

		private void HotelDesignerButton_Click(object sender, RoutedEventArgs e)
		{
            HotelDesignerWindow hotelDesigner = new HotelDesignerWindow();
            hotelDesigner.Show();
            
		}
		private void RoomAssign_Click(object sender, RoutedEventArgs e)
		{
            RoomAssignmentWindow assignWindow = new RoomAssignmentWindow();
            assignWindow.Show();
		}

        private void RoomList_Click(object sender, RoutedEventArgs e)
        {
            RoomListWindow listWindow = new RoomListWindow();
            listWindow.Show();
        }

        private void debugDlvButton_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Group> groups = DebugInitGroupCollection();

            string a = DLVHandler.GenerateQuery(groups);
        }

        private static ObservableCollection<Group> DebugInitGroupCollection()
        {
            ObservableCollection<Group> groups = new ObservableCollection<Group>();

            // Declare and add statically 2-3 groups, each with 2 guests
            ObservableCollection<Guest> guests1 = new ObservableCollection<Guest>
            {
                new Guest{LastName = "Nowakowski", FirstName = "Piotr" },
                new Guest{LastName = "adasd", FirstName = "ssss" }
            };
            Group group1 = new Group(guests1, "aa");
            groups.Add(group1);

            ObservableCollection<Guest> guests2 = new ObservableCollection<Guest>
            {
                new Guest{LastName = "sddd", FirstName = "aaaa" },
                new Guest{LastName = "bbb", FirstName = "pitor" }
            };
            Group group2 = new Group(guests2, "grr");
            groups.Add(group2);

            group1.WantGroupToStayTogether = true;
            group1.WantNoiseReduction = true;
            group2.WantModularFurniture = true;
            group2.WantSecurityFeatures = true;

            return groups;
        }
    }
}

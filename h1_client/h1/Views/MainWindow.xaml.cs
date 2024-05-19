using h1.Models;
using h1.Views;
using MongoDB.Bson;
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

		private void HotelDesignerButton_Click(object sender, RoutedEventArgs e)
		{
            //check if hotel exists yet
            if (HotelExists())
            {
                MessageBoxResult result = MessageBox.Show(
                    "Warning!! This action will remove the guests currently within the system! (applies on confirmation)",
                    "Warning", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
            }

            HotelDesignerWindow hotelDesigner = new HotelDesignerWindow();
            hotelDesigner.Show();
            
		}

        private bool HotelExists()
        {
            BsonDocument a = DBMethods.GetHotel();
            return !(a == null || a.IsBsonNull);
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
            throw new NotImplementedException();
        }


    }
}

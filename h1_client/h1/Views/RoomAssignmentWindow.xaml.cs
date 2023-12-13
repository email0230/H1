using h1.Models;
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
using System.Windows.Shapes;

namespace h1.Views
{
	/// <summary>
	/// Interaction logic for RoomAssignmentWindow.xaml
	/// </summary>
	public partial class RoomAssignmentWindow : Window
	{
		Hotel hotel = Hotel.GetInstance();
        public ObservableCollection<Group> Groups { get; set; } = new ObservableCollection<Group>();
        public RoomAssignmentWindow()
		{
			InitializeComponent();
			GetGuests();

        }

		private void GetGuests()
		{
			var guests = DBMethods.GetGuests();
            GuestSummaryListView.ItemsSource = guests;
        }

        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

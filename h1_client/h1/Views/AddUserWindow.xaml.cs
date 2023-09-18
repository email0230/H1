using h1.Models;
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
using System.Windows.Shapes;
using System.Xml.Linq;

namespace h1
{
	/// <summary>
	/// Interaction logic for AddUserWindow.xaml
	/// </summary>
	public partial class AddUserWindow : Window
	{
		public AddUserWindow()
		{
			InitializeComponent();
		}

		private void Submit_Click(object sender, RoutedEventArgs e)
		{
			// Extract values from the form controls
			string name = txtName.Text;
			DateTime? arrivalDate = dpArrivalDate.SelectedDate;
			DateTime? departureDate = dpDepartureDate.SelectedDate;

			// Extract the selected room number from the ComboBox control
			ComboBoxItem selectedRoomItem = cmbRoomNumber.SelectedItem as ComboBoxItem;
			string roomNumber = selectedRoomItem?.Content.ToString(); // Use null-conditional operator

			// Call the validation method from the FormValidator class
			if (FormValidator.ValidateInsert1Form(name, arrivalDate, departureDate, roomNumber))
			{
				// If validation passes, continue with the submission
				MessageBox.Show($"Hello, {name}! Arrival Date: {arrivalDate}, Departure Date: {departureDate}, Room Number: {roomNumber}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				DBMethods.Insert(name, arrivalDate, departureDate, roomNumber);
				Close();
			}
		}
	}
}

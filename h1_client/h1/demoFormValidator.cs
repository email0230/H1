using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace h1
{
	internal class demoFormValidator
	{
		public static bool ValidateForm(string name, DateTime? arrivalDate, DateTime? departureDate, string roomNumber)
		{
			if (string.IsNullOrEmpty(name))
			{
				MessageBox.Show("Please enter a name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			if (!arrivalDate.HasValue)
			{
				MessageBox.Show("Please select an arrival date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			if (!departureDate.HasValue)
			{
				MessageBox.Show("Please select a departure date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			if (departureDate < arrivalDate)
			{
				MessageBox.Show("Departure date cannot be earlier than arrival date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			if (string.IsNullOrEmpty(roomNumber))
			{
				MessageBox.Show("Please select a room number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			// You can add additional room number validation here if needed (asside, will need to compare this to an array of available rooms at some point)

			return true; // All validation checks pass
		}
	}
}

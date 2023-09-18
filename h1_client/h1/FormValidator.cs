using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace h1
{
	internal class FormValidator
	{
		public static bool ValidateInsert1Form(string name, DateTime? arrivalDate, DateTime? departureDate, string roomNumber)
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

			// You can add additional room number validation here if needed (asssssside, will need to compare this to an array of available rooms at some point)

			return true; // All validation checks pass
		}

		public static bool ValidateHotelForm(string hotelName) //TODO: add list of rooms here later!!!
		{
			if (string.IsNullOrEmpty(hotelName))
			{
				MessageBox.Show("Please enter a name for your hotel.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			//TODO: add more validation checks once the hotel implementation matures

			return true; // All validation checks pass
		}
	}
}

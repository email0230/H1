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

		public static bool ValidateHotelForm(string hotelName, string r1, string r2, string r3)
		{
			if (string.IsNullOrEmpty(hotelName))
			{
				MessageBox.Show("Please enter a name for your hotel.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			int rooms1Person = Int32.Parse(r1);
            int rooms2Persons = Int32.Parse(r1);
            int rooms3Persons = Int32.Parse(r1);

            if (rooms1Person == 0 && rooms2Persons == 0 && rooms3Persons == 0)
            {
                ShowNoRoomsErrorPrompt();
				return false;
            }

            return true; // All validation checks pass
		}

        private static void ShowNoRoomsErrorPrompt()
        {
            MessageBox.Show("A hotel must have some rooms.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

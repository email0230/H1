using h1.Models;
using MongoDB.Bson.IO;
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
using Newtonsoft.Json;

namespace h1.Views
{
	/// <summary>
	/// Interaction logic for HotelDesignerWindow.xaml
	/// </summary>
	public partial class HotelDesignerWindow : Window
	{
		Hotel hotel = Hotel.GetInstance();

        public HotelDesignerWindow()
		{
			InitializeComponent();
			SetTextBoxValuesFromHotel();
        }

		private void SetTextBoxValuesFromHotel()
		{
			HotelNameTextBox.Text = hotel.Name;
			int[] roomCounts = hotel.GetHotelRoomCounts();
			RoomsFor1PersonTextBox.Text = roomCounts[0].ToString();
			RoomsFor2PersonsTextBox.Text = roomCounts[1].ToString();
			RoomsFor3PersonsTextBox.Text = roomCounts[2].ToString();
		}

		private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Allow only numeric characters (0-9) and handle decimal points if needed
			if (!char.IsDigit(e.Text[0]) && e.Text[0] != '.')
			{
				e.Handled = true; // Block non-numeric input
			}
		}

		private void SubmitButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want to overwrite the current hotel configuration?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.Yes)
            {
                DBMethods.DeleteAllGuests();
                SubmitHotelForm();
            }
        }
        private List<Room> GetRooms()
        {
            string roomsFor1Person = RoomsFor1PersonTextBox.Text;
            string roomsFor2Persons = RoomsFor2PersonsTextBox.Text;
            string roomsFor3Persons = RoomsFor3PersonsTextBox.Text;

            List<Room> roomsList = new List<Room>();

            if (int.TryParse(roomsFor1Person, out int rooms1Person) &&
                int.TryParse(roomsFor2Persons, out int rooms2Persons) &&
                int.TryParse(roomsFor3Persons, out int rooms3Persons))
            {
                

                for (int i = 0; i < rooms1Person; i++)
                {
                    Room room = new Room { Capacity = 1 };
                    roomsList.Add(room);
                }
                for (int i = 0; i < rooms2Persons; i++)
                {
                    Room room = new Room { Capacity = 2 };
                    roomsList.Add(room);
                }
                for (int i = 0; i < rooms3Persons; i++)
                {
                    Room room = new Room { Capacity = 3 };
                    roomsList.Add(room);
                }
            }
            else
            {
                // Handle the case where parsing fails (non-numeric input)
                throw new NotImplementedException();
            }
            return roomsList;
        }

      

        private void SubmitHotelForm()
        {
            string name = HotelNameTextBox.Text;
            hotel.Name = name;
            hotel.LastModifiedDate = DateTime.Now;
            hotel.Rooms = GetRooms();

            if (FormValidator.ValidateHotelForm(name,
                                                RoomsFor1PersonTextBox.Text,
                                                RoomsFor2PersonsTextBox.Text,
                                                RoomsFor3PersonsTextBox.Text))
            {
                DBMethods.StoreHotel(hotel);
                
                if (PromptForRoomEdit())
                    SummonRoomList();
                Close();
            }
        }

        private bool PromptForRoomEdit() 
        {
            MessageBoxResult editRoomResult = MessageBox.Show(
                "Hotel data saved successfully. Would you like to edit the room data now?",
                "Success",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (editRoomResult == MessageBoxResult.Yes)
                return true;
			return false;
        }

        private void SummonRoomList()
        {
            RoomListWindow listWindow = new RoomListWindow();
            listWindow.Show();
        }
    }
}

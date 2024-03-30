using h1.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using ZstdSharp.Unsafe;

namespace h1.Views
{
    /// <summary>
    /// Interaction logic for RoomListWindow.xaml
    /// </summary>
    public partial class RoomListWindow : Window
    {
        Hotel hotel = Hotel.GetInstance();

        public RoomListWindow()
        {
            InitializeComponent();
            RoomsListView.ItemsSource = hotel.Rooms;
            GetHotelOccupancyStatus();
        }

        //todo: make sure this looks alright after removing the debug buttons row

        private void GetHotelOccupancyStatus() //duplicate of the one in assignment window TODO: make this better :)
        {
            var hotelRoomsFull = DBMethods.GetFullListOfRooms();
            double percentage = GetOccupancyDecimalValue(hotelRoomsFull) * 100;
            NumericalDisplay.Text = $"Occupancy: {percentage}%";
        }

        private double GetOccupancyDecimalValue(List<Room> inputList)
        {
            int totalCapacity = 0, totalOccupancy = 0;

            foreach (var room in inputList)
            {
                totalCapacity += room.Capacity;
                totalOccupancy += room.Occupancy;
            }

            try
            {
                return Math.Round((double)totalOccupancy / totalCapacity, 2);
            }
            catch (DivideByZeroException ex)
            {
                Debug.WriteLine("occupancy returned zero -_-: " + ex.Message); //todo: remove this too :D
                return double.NaN;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DebugPrintRoomStatus();

            hotel.LastModifiedDate = DateTime.Now; //so that db may find it, can be removed if a better way of getting most recent hotel is found

            if (true) //TODO: validation
            {
                DBMethods.StoreHotel(hotel);
                Close();
            }
        }

        private void RoomSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Room room) //room datacontext not called; possible solution - pass the room object instead of its Id
            {
                RoomSettingsWindow settingsWindow = new RoomSettingsWindow();
                settingsWindow.DataContext = room; //make sure the window uses the room selected
                settingsWindow.ShowDialog();
            }
        }

        //all these two functions below are for, is for them to be put in a breakpoint, so i can see their objects directly, and check if everything is in order
        #region debug
        //TODO: delete these before release
        private void debug_rooms_breakpoint_button_Click(object sender, RoutedEventArgs e)
        {
            var rooms = hotel.Rooms;
        }

        private void debug_guests_breakpoint_button_Click(object sender, RoutedEventArgs e)
        {
            var guests = DBMethods.GetGuests();
        }

        private void debug_test_button_3_Click(object sender, RoutedEventArgs e)
        {
            DebugPrintRoomStatus();
        }

        private void DebugPrintRoomStatus()
        {
            Debug.WriteLine("Debug - displaying room properties");
            Debug.Indent();
            foreach (var item in hotel.Rooms)
            {
                Debug.WriteLine($"Room #{item.Id}");
                Debug.Indent();
                Debug.WriteLine($"NoiseReduction: {item.NoiseReduction}");
                Debug.WriteLine($"SecurityFeatures: {item.SecurityFeatures}");
                Debug.WriteLine($"SmartLighting: {item.SmartLighting}");
                Debug.WriteLine($"Balcony: {item.Balcony}");
                Debug.WriteLine($"ModularFurniture: {item.ModularFurniture}");
                Debug.Unindent();
            }
            Debug.Unindent();
        } 
        #endregion
    }
}

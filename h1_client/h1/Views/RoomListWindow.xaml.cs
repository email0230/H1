using h1.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Metadata;
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

        private void GetHotelOccupancyStatus()
        {
            var hotelRoomsFull = DBMethods.GetFullListOfRooms();
            double percentage = Math.Truncate(GetOccupancyDecimalValue(hotelRoomsFull) * 100);

            // handle the roomlist not being initialized right after a hotel creation
            if (Double.IsNaN(percentage) || Double.IsInfinity(percentage))
            {
                NumericalDisplay.Text = string.Empty;
                return;
            }

            NumericalDisplay.Text = $"Occupancy: {percentage}%";
        }

        private double GetOccupancyDecimalValue(List<Room> inputList)
        {
            var (occupancy, capacity) = DBMethods.GetHotelOccupancyAndCapacity();

            try
            {
                return Math.Round((double)occupancy / capacity, 2);
            }
            catch (DivideByZeroException ex)
            {
                return double.NaN;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DebugPrintRoomStatus();

            hotel.LastModifiedDate = DateTime.Now;

            if (true)
            {
                DBMethods.StoreHotel(hotel);
                DBMethods.StoreRooms(hotel.Rooms);
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

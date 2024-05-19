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
        List<Room> roomsFull = new List<Room>();
        public RoomListWindow()
        {
            InitializeComponent();

            try
            {
                roomsFull = DBMethods.GetFullListOfRooms();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                roomsFull = null;
            }

            if (roomsFull == null || roomsFull.Count == 0)
            {
                roomsFull = hotel.Rooms;
            }

            RoomsListView.ItemsSource = roomsFull ?? new List<Room>();
            GetHotelOccupancyStatus();
        }

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
            hotel.LastModifiedDate = DateTime.Now;
            hotel.Rooms = roomsFull;
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
    }
}

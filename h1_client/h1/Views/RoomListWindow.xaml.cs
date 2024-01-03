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
            PassRoomsToListView();
        }
        private void PassRoomsToListView()
        {
            //this entire thing is vestigial from when i tried to add a guest to see if tehre are any immidate issues. feel free to trash this!
            List<Guest> guests = GuestDebugMethod();
            foreach (var item in guests)
            {
                if (item != null)
                {
                    //throw new NotImplementedException();
                }
            }
            List<Room> rooms = hotel.Rooms;
            RoomsListView.ItemsSource = rooms;
        }

        private void RoomSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int roomId) //room datacontext not called; possible solution - pass the room object instead of its Id
            {
                RoomSettingsWindow settingsWindow = new RoomSettingsWindow();
                //settingsWindow.DataContext = room; //make sure the window uses the room selected
                settingsWindow.ShowDialog();
            }
        }


        private List<Guest> GuestDebugMethod()
        {
            List<Guest> guestList = DBMethods.GetGuests();
            bool thomasExists = false;
            thomasExists = CheckListForThomas(guestList, thomasExists);
            if (!thomasExists)
                AddThomas(guestList);

            return guestList;
        }

        private static bool CheckListForThomas(List<Guest> guestList, bool doesExist)
        {
            foreach (var guest in guestList)
            {
                if (guest.LastName == "Thomas" && guest.FirstName == "Test")
                {
                    doesExist = true;
                    break;
                }
            }

            return doesExist;
        }

        private static void AddThomas(List<Guest> guestList)
        {
            Guest testGuest = new Guest
            {
                LastName = "Thomas",
                FirstName = "Test",
                //AssignedRoomNumber = 1,
            };
            DBMethods.Insert(testGuest);

            guestList.Add(testGuest);
        }


        //all these two functions below are for, is for them to be put in a breakpoint, so i can see their objects directly, and check if everything is in order
        private void debug_rooms_breakpoint_button_Click(object sender, RoutedEventArgs e)
        {
            var rooms = hotel.Rooms;
        }

        private void debug_guests_breakpoint_button_Click(object sender, RoutedEventArgs e)
        {
            var guests = DBMethods.GetGuests();
        }
    }
}

using h1.Models;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
		Hotel hotel = Hotel.GetInstance(); //probably useless
        public ObservableCollection<Group> Groups { get; set; } = new ObservableCollection<Group>();
        public RoomAssignmentWindow()
		{
			InitializeComponent();
			PopulateListWithGuests();
            DataContext = this;
        }

		private void PopulateListWithGuests()
		{
			var guests = DBMethods.GetGuests();
            GuestSummaryListView.ItemsSource = guests;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Group> formData = Groups;

            //validate, and send?
            DebugPrintGroupsVarStatus();
            DebugPrintGroupProperties();

            // Instantiate SolutionInputBuilder
            SolutionInputBuilder builder = new SolutionInputBuilder();

            string query = builder.GenerateQuery(formData);
            List<Tuple<int, int>> solution = DLVHandler.GetSolutionFromSolver(query);
            Dictionary<Guest, int> dictionary = builder.GetGuestDict();

            AssignRooms(solution, builder.GetGuestDict());

            hotel.LastModifiedDate = DateTime.Now; //so that db may find it, can be removed if a better way of getting most recent hotel is found
            
            SendHotelToDB(); //it seems the hotel bit that is sent to the db doesnt contain hotel> guests information...

            Close();
        }

        private void SendHotelToDB() //this is a triplicate from the one present in roomlist... gott afigure out a better way to outsource hotel related functions!!
        {
            string jsonHotel = Newtonsoft.Json.JsonConvert.SerializeObject(hotel);
            DBMethods.StoreHotel(jsonHotel);
        }

        private void AssignRooms(List<Tuple<int, int>> solution, Dictionary<Guest, int> dictionary)
        {
            foreach (var tuple in solution)
            {
                int guestNumber = tuple.Item1;
                int roomId = tuple.Item2;

                // Assuming the structure is Dictionary<Guest, int> where int is the numerical representation
                foreach (var kvp in dictionary)
                {
                    if (kvp.Value == guestNumber)
                    {
                        // The guest with the matching numerical representation is found
                        AssignGuest(kvp.Key, roomId);
                        break;  // Break the loop once the guest is found
                    }
                }
            }
        }

        private void AssignGuest(Guest guest, int roomId)
        {
            // Check if the guest is already assigned to a room
            if (guest.AssignedRoomNumber.HasValue)
                throw new InvalidOperationException($"Guest {guest.FirstName} {guest.LastName} is already assigned to room {guest.AssignedRoomNumber}.");
            
            Room room = hotel.FindRoomById(roomId);

            // Check if the room exists
            if (room == null)
            {
                throw new InvalidOperationException();
            }
            
            // Try to add the guest to the room
            if (!room.AddGuest(guest))
            {
                throw new InvalidOperationException("Guest addition failed!");
            }
            // Set the room information for the guest
            guest.AssignedRoomNumber = roomId;

            //maybe send guests to the guiest collection in db here????

            // Notify UI elements of the AssignedRoomNumber change
            guest.NotifyAssignedRoomNumberChanged();

            CheckIfRoomsInHotelHaveGuests();
        }

        private void CheckIfRoomsInHotelHaveGuests()
        {
            var a = hotel.Rooms;
            foreach (var room in a)
            {
                List<Guest> aaa = room.GetGuests();

                Debug.WriteLine($"DISPLAYING ROOM NR {room.Id}");
                Debug.Indent();
                foreach (var item in aaa)
                {
                    Debug.WriteLine($"found a guest: {item.LastName} {item.FirstName}");
                }
                Debug.Unindent();
            }
        }

        private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
        {
            CreateGroupElement();
        }

        private void RemoveGroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button removeButton && removeButton.DataContext is Group groupToRemove)
            {
                if (MessageBox.Show($"Are you sure you want to remove the group '{groupToRemove.GroupName}'?", "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Groups.Remove(groupToRemove);
                    DebugPrintGroupsVarStatus();
                }
            }
        }
        private void CreateGroupElement()
        {
            if (DataContext is RoomAssignmentWindow viewModel)
            {
                AddGroupTicket(viewModel);

                DebugPrintGroupsVarStatus();
            }
        }

        static void AddGroupTicket(RoomAssignmentWindow viewModel)
        {
            ObservableCollection<Guest> guests = new ObservableCollection<Guest>
            {
                new Guest{LastName = "Nowakowski", FirstName = "Piotr" },
            };
            int groupIndexNumber = GetIndexNumberForGroup(viewModel);
            Group groupAdded = new Group(guests, $"group #{groupIndexNumber}");
            viewModel.Groups.Add(groupAdded); //add a thing with a debug string and array of guests
        }

        private static int GetIndexNumberForGroup(RoomAssignmentWindow viewModel)
        {
            int length = viewModel.Groups.Count;
            
            Debug.WriteLine($"Group.Count is now {length}, returning {length+1}");

            return length + 1;
        }

        private void GroupSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            //find the corresponding Group object to which the button belongs
            if (sender is FrameworkElement element && element.DataContext is Group group)
            {
                GroupSettingsWindow settingsWindow = new GroupSettingsWindow();
                settingsWindow.DataContext = group; //make sure the window uses the group selected
                settingsWindow.ShowDialog();
            }
        }

        private void AddNewGuestButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("New guest added...");
            DebugPrintGroupsVarStatus();

            if (GetSelectedGroup() == null)
            {
                MessageBox.Show("No group selected. \nSelect a group by double-clicking it!", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
                Debug.WriteLine("Early return called - SelectedGroup was null.");
                return;
            }

            if (!ButtonBelongsToCorrectGroupTicket(sender))
            {
                MessageBox.Show("Please select the group before adding more guests to it.", "Group Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
                Debug.WriteLine("Early return called - pressed button was in the wrong group.");
                return;
            }
            
            AddGuestToGroupTicket();
        }
        private void AddGuestToGroupTicket()
        {
            Guest newGuest = new Guest { FirstName = "...", LastName = "..." };
            Group selectedGroup = GetSelectedGroup();
            
            selectedGroup.Guests.Add(newGuest);
        }

        #region Checking if button is in the selected ticket
        private bool ButtonBelongsToCorrectGroupTicket(object sender)
        {
            // Check if there is a selected item
            if (listViewElement.SelectedItem != null)
            {
                // Assuming your Group object is stored in the DataContext of the selected ListViewItem
                if (listViewElement.SelectedItem is Group selectedGroup)
                {
                    // Now you have the selected Group object

                    // Check if the button belongs to the correct group ticket
                    if (GetButtonGroupTicket(sender) == selectedGroup)
                    {
                        return true;
                    }
                }
                else
                {
                    MessageBox.Show("DEBUG: Failed to retrieve selected group.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No item selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }

        private Group GetButtonGroupTicket(object sender)
        {
            if (sender is Button addButton)
            {
                // Traverse the visual tree to find the ListViewItem
                DependencyObject depObject = addButton;
                while (depObject != null && !(depObject is ListViewItem))
                {
                    depObject = VisualTreeHelper.GetParent(depObject);
                }

                if (depObject is ListViewItem listViewItem)
                {
                    // Assuming your Group object is stored in the DataContext of ListViewItem
                    if (listViewItem.DataContext is Group group)
                    {
                        return group;
                    }
                }
            }

            return null;
        }
        #endregion

        private Group GetSelectedGroup()
        {
            Group selectedGroup = null;

            if (listViewElement.SelectedItem != null)
            {
                if (listViewElement.SelectedItem is Group group)
                {
                    selectedGroup = group;
                }
                else
                {
                    MessageBox.Show("Failed to retrieve selected group.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return selectedGroup;
        }

        #region debug
        private void DebugPrintGroupsVarStatus()
        {
            Debug.WriteLine("========================debuggin=======================");
            Debug.WriteLine("ACTIVE GROUPS WITHIN GROUPS - OBSERVABLE COLLECTION");
            Debug.Indent();
            foreach (var item in Groups)
            {
                Debug.WriteLine($"Amount of guests in group named \"{item.GroupName}\": {item.Guests.Count}");
                Debug.Indent();
                foreach (var g in item.Guests)
                {
                    Debug.WriteLine($"Guest: {g.FirstName}, {g.LastName}");
                }
                Debug.Unindent();
            }
            Debug.Unindent();
            Debug.WriteLine("========================endebuggin=====================");
        }

        private void DebugButtonClicked(object sender, RoutedEventArgs e) //good candidate to get trashed soonish
        {
            var a = Groups;
            ObservableCollection<Guest>? b = null;
            foreach (var group in a)
            {
                group.Guests = b;
                string? test = group.ToString();
            }
        }

        private void DebugPrintGroupProperties()
        {
            Debug.WriteLine("Printing group properties:");
            Debug.Indent();
            foreach (var item in Groups)
            {
                Debug.WriteLine($"{item.GroupName}");
                Debug.Indent();
                Debug.WriteLine($"GroupToStayTogether: {item.WantGroupToStayTogether}");
                Debug.WriteLine($"NoiseReduction: {item.WantNoiseReduction}");
                Debug.WriteLine($"SecurityFeatures: {item.WantSecurityFeatures}");
                Debug.WriteLine($"SmartLighting: {item.WantSmartLighting}");
                Debug.WriteLine($"Balcony: {item.WantBalcony}");
                Debug.WriteLine($"ModularFurniture: {item.WantModularFurniture}");
                Debug.Unindent();
            }
            Debug.Unindent();
        }


        #endregion

        //todo:
        /* possible dropdown, to hide guests?
         */
    }
}

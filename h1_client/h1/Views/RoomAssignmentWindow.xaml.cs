﻿using h1.Models;
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
		Hotel hotel = Hotel.GetInstance();

        public ObservableCollection<Group> Groups { get; set; } = new ObservableCollection<Group>();

        public RoomAssignmentWindow()
        {
            InitializeComponent();
            PopulateListWithGuests();
            HandleOccupancyDisplay();
            DataContext = this;
        }

        private void HandleOccupancyDisplay()
        {
            var hotelRoomsFull = DBMethods.GetFullListOfRooms();
            double percentage = GetOccupancyDecimalValue(hotelRoomsFull) * 100;
            OccupancyTextBlock.Text = $"Occupancy: {percentage}%"; //todo: this occupancy display gets broken on hotel resets, fix it
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
                Debug.WriteLine("occupancy returned zero -_-: " + ex.Message); //todo: remove this :D
                return double.NaN;
            }
        }

        private void PopulateListWithGuests()
		{
			List<Guest> guests = DBMethods.GetGuests();
            GuestSummaryListView.ItemsSource = guests;
        }

        #region guest assignment
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Group> formData = Groups;
            //validate, and send?
            
            //TODO: remove these two debug methods
            DebugPrintGroupsVarStatus();
            DebugPrintGroupProperties();

            SolutionInputBuilder builder = new SolutionInputBuilder();
            var rawRooms = hotel.Rooms;
            //List<Room> cleanRooms = RemoveInvalidRooms(rawRooms);
            string query = builder.GenerateQuery(formData, rawRooms); //here, rooms passed might need to get checked for any disabled rooms!!
            List<Tuple<int, int>> solution = DLVHandler.GetSolutionFromSolver(query);

            MatchGuestsToRooms(solution, builder.GetGuestDict());

            DBMethods.StoreHotel(hotel);

            var rooms = hotel.Rooms;
            DebugCheckIfRoomsInHotelHaveGuests(rooms);
            DBMethods.StoreRooms(rooms);

            Close();
        }

        private void MatchGuestsToRooms(List<Tuple<int, int>> solution, Dictionary<Guest, int> dictionary)
        {
            foreach (var tuple in solution)
            {
                int guestNumber = tuple.Item1;
                int roomId = tuple.Item2;

                foreach (var kvp in dictionary)
                {
                    if (kvp.Value == guestNumber)
                    {
                        // The guest with the matching numerical representation is found
                        AssignGuest(kvp.Key, roomId);
                        break;
                    }
                }
            }
        }

        private void AssignGuest(Guest guest, int roomId)
        {
            // Check if the guest is already assigned to a room
            if (guest.AssignedRoomNumber.HasValue)
            {
                throw new InvalidOperationException($"Guest {guest.FirstName} {guest.LastName} is already assigned to room {guest.AssignedRoomNumber}.");
            }

            Room room = hotel.FindRoomById(roomId);

            // Check if the room exists
            if (room == null)
            {
                throw new InvalidOperationException($"Room with ID {roomId} not found.");
            }

            // Try to add the guest to the room
            if (!room.AddGuest(guest))
            {
                Debug.WriteLine("Error: room at capacity!");
                throw new InvalidOperationException("Guest addition failed!");
            }

            guest.AssignedRoomNumber = roomId;

            DBMethods.StoreGuest(guest);

            //notify UI elements
            guest.NotifyAssignedRoomNumberChanged();
        } 

        #endregion

        

        private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoomAssignmentWindow viewModel)
            {
                AddGroupTicket(viewModel);
            }
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
   
        static void AddGroupTicket(RoomAssignmentWindow viewModel)
        {
            ObservableCollection<Guest> guests = new ObservableCollection<Guest>
            {
                new Guest{LastName = "Guest", FirstName = "Sample" },
            };

            //int groupIndexNumber = GetIndexNumberForGroup(viewModel);
            int groupIndexNumber = viewModel.Groups.Count + 1;

            Group groupAdded = new Group(guests, $"Group #{groupIndexNumber}");
            viewModel.Groups.Add(groupAdded);
        }

        private void GroupSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            //find the corresponding Group object to which the button belongs
            if (sender is FrameworkElement element && element.DataContext is Group group)
            {
                GroupSettingsWindow settingsWindow = new GroupSettingsWindow(group);
                settingsWindow.DataContext = group; //make sure the window uses the group selected
                settingsWindow.ShowDialog();
            }
        }

        private void AddNewGuestButton_Click(object sender, RoutedEventArgs e) //todo: this method is an unacceptable mess, sort it out :)
        {
            Group selected = GetSelectedGroup();
            if (selected == null)
            {
                MessageBox.Show("No group selected. \nSelect a group by double-clicking it!", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!ButtonBelongsToCorrectGroupTicket(sender))
            {
                MessageBox.Show("Please select the group before adding more guests to it.", "Group Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (selected.WantGroupToStayTogether && selected.Guests.Count >= GetBiggestRoomCapacity())
            {
                //todo: make this line do nothing and skip to line 239. and generally make this less repetitive
            }

            AddBlankGuestToGroupTicket();

            //lock button afterwards if at capacity
            if (selected.WantGroupToStayTogether && selected.Guests.Count >= GetBiggestRoomCapacity())
            {
                ToolTipService.SetShowOnDisabled((Button)sender, true); //jank

                //todo: create a warning tooltip explaining that a group cannot be bigger than 1 room, AND still want to be together
                ((Button)sender).ToolTip = "no bueno max group size reached!";
                ((Button)sender).IsEnabled = false;
            }
            else
            {
                ((Button)sender).IsEnabled = true;
                ((Button)sender).ToolTip = "Add Guest";
            }
        }

        private void AddBlankGuestToGroupTicket()
        {
            Guest newGuest = new Guest { FirstName = "...", LastName = "..." };
            Group selectedGroup = GetSelectedGroup();
            
            selectedGroup.Guests.Add(newGuest);
        }

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

        #region Checking if button is in the selected ticket
        private bool ButtonBelongsToCorrectGroupTicket(object sender)
        {
            // Assuming your Group object is stored in the DataContext of the selected ListViewItem
            if (!(listViewElement.SelectedItem is Group selectedGroup))
            {
                MessageBox.Show("DEBUG: Failed to retrieve selected group.",
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                return false;
            }

            if (GetButtonGroupTicket(sender) != selectedGroup)
            {
                return false;
            }

            //all checks passed, let it through
            return true;
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

        #region debug
        //TODO: delete these before release

        private void DebugCheckIfRoomsInHotelHaveGuests(List<Room> input)
        {
            foreach (var room in input)
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

        //todo: remove redundant code
        private void DisableAddingToATicket()
        {
            foreach (var group in Groups)
            {
                if (group.Guests.Count >= GetBiggestRoomCapacity() && group.WantGroupToStayTogether)
                {
                    
                }
            }
        }

        private bool CheckIfGroupSizeAboveMax(Group? group)
        {
            if (group.Guests.Count > GetBiggestRoomCapacity())
            {
                return true;
            }
            
            return false;
        }

        private int GetBiggestRoomCapacity()
        {
            //todo: implement this :D
            return 3;
        }

    }
}

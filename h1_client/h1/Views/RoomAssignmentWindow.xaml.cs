using h1.Models;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.CodeDom;
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

        private DateTime? ArrivalDate;
        private DateTime? DepartureDate;

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
            double percentage = Math.Truncate(GetOccupancyDecimalValue(hotelRoomsFull) * 100);
            
            if (double.IsNaN(percentage))
            {
                MessageBox.Show($"Warning! Rooms not defined properly!\nReturn to Room list to configure them.",
                                "Rooms undefined",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }

            OccupancyTextBlock.Text = $"Occupancy: {percentage}%"; 
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

        private void PopulateListWithGuests()
		{
			List<Guest> guests = DBMethods.GetGuests();
            GuestSummaryListView.ItemsSource = guests;
        }

        #region guest assignment
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Group> formData = Groups;

            if (FormValidator.ValidateGroupsFormData(ArrivalDate.HasValue,
                                                     DepartureDate.HasValue,
                                                     formData))
            {
                AssignDatesToGuests(formData);
                BuildSolutionAndStore(formData);

                MessageBox.Show($"Guest Assignment successful!",
                                "Success",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                Close();
            }
        }

        private void AssignDatesToGuests(ObservableCollection<Group> formData)
        {
            foreach (Group group in formData)
            {
                foreach (var guest in group.Guests)
                {
                    guest.ArrivalDate = ArrivalDate;
                    guest.DepartureDate = DepartureDate;
                }
            }
        }

        private void BuildSolutionAndStore(ObservableCollection<Group> formData)
        {
            SolutionInputBuilder builder = new SolutionInputBuilder();
            var rawRooms = hotel.Rooms;
            string query = builder.GenerateQuery(formData, rawRooms); //here, rooms passed might need to get checked for any disabled rooms!!

            List<Tuple<int, int>> solution = DLVHandler.GetSolutionFromSolver(query);

            MatchGuestsToRooms(solution, builder.GetGuestDict());

            SaveDataToDB();
        }

        private void SaveDataToDB()
        {
            DBMethods.StoreHotel(hotel);
            var rooms = hotel.Rooms;
            DBMethods.StoreRooms(rooms);
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
                if (MessageBox.Show($"Are you sure you want to remove the group '{groupToRemove.GroupName}'?",
                                    "Confirm Removal",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Groups.Remove(groupToRemove);
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

        private void AddNewGuestButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the button does not belong to the correct group ticket
            if (!ButtonBelongsToCorrectGroupTicket(sender))
            {
                MessageBox.Show("Please select the group before adding more guests to it.", "Group Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Group selected = GetSelectedGroup();
            if (selected == null)
            {
                MessageBox.Show("No group selected. \nSelect a group by double-clicking it!", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            bool groupAtCapacity = IsKeepTogetherGroupAtCapacity(selected);
            if (groupAtCapacity)
            {
                DisableAddButton(sender);
                return;
            }

            AddBlankGuestToGroupTicket();

            groupAtCapacity = IsKeepTogetherGroupAtCapacity(selected); //check again after adding a guest
            if (groupAtCapacity)
            {
                DisableAddButton(sender);
            }
            else 
            {
                if (!((Button)sender).IsEnabled)
                {
                    EnableAddButton(sender);
                }
            }
        }

        private bool IsKeepTogetherGroupAtCapacity(Group group) => group.WantGroupToStayTogether && group.Guests.Count >= GetBiggestRoomCapacity();

        private int GetBiggestRoomCapacity() => DBMethods.GetFullListOfRooms().Max(room => room.Capacity);

        private static void EnableAddButton(object sender)
        {
            ((Button)sender).IsEnabled = true;
            ((Button)sender).ToolTip = "Add Guest";
        }

        private static void DisableAddButton(object sender)
        {
            ToolTipService.SetShowOnDisabled((Button)sender, true);
            ((Button)sender).ToolTip = "Max group size reached! Turn off KeepTogether to add more guests to this group!";

            // Disable the button
            ((Button)sender).IsEnabled = false;
        }


        private void AddBlankGuestToGroupTicket()
        {
            Guest newGuest = new Guest { FirstName = "...", LastName = "..." };
            Group selectedGroup = GetSelectedGroup();
            
            selectedGroup.Guests.Add(newGuest);
        }

        private Group? GetSelectedGroup()
        {
            Group? selectedGroup = null;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GroupStayDurationWindow durationWindow = new GroupStayDurationWindow();

            durationWindow.PassDatesEvent += EventUpdate;

            durationWindow.ShowDialog();
        }

        private void EventUpdate(DateTime a, DateTime d)
        {
            ArrivalDate = a;
            DepartureDate = d;
        }
    }
}

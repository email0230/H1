using h1.Models;
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
			GetGuests();
            DataContext = this;
        }

		private void GetGuests()
		{
			var guests = DBMethods.GetGuests();
            GuestSummaryListView.ItemsSource = guests;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var formData = Groups;
            //validate, and send?
            DebugPrintGroupsVarStatus();
            throw new NotImplementedException();
        }
        private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
        {
            CreateGroupElement();
        }
        private void CreateGroupElement()
        {
            // Assuming you have a ViewModel with an ObservableCollection named "Groups"
            if (DataContext is RoomAssignmentWindow viewModel)
            {
                ObservableCollection<Guest> guests = new ObservableCollection<Guest>
                {
                    new Guest{LastName = "Nowakowski", FirstName = "Piotr" },
                };

                // Add a new object to the ObservableCollection
                Group groupAdded = new Group(guests, "Group name here!");
                viewModel.Groups.Add(groupAdded); //add a thing with a debug string and array of guests

                DebugPrintGroupsVarStatus();
            }
        }

        private void GroupSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Find the corresponding Group object to which the button belongs
            if (sender is FrameworkElement element && element.DataContext is Group group)
            {
                // Create a new instance of the settings window
                GroupSettingsWindow settingsWindow = new GroupSettingsWindow();

                // Set the DataContext of the settings window to the Group object
                settingsWindow.DataContext = group;

                // Show the settings window
                settingsWindow.ShowDialog();
            }
        }

        private void DebugPrintGroupsVarStatus()
        {
            Debug.WriteLine("========================debuggin=======================");
            Debug.WriteLine("ACTIVE GROUPS WITHIN GROUPS - OBSERVABLE COLLECTION");
            foreach (var item in Groups)
            {
                Debug.WriteLine($"Amount of guests in group *{item.DebugString}*:{item.Guests.Count}");
                foreach (var g in item.Guests)
                {
                    Debug.WriteLine($"Guest: {g.FirstName}, {g.LastName}");
                }
            }
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

        //private void AddNewGuestButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Debug.WriteLine("New guest added...");
        //    DebugPrintGroupsVarStatus();
        //    //DebugRestartContext();

        //    if (DataContext is RoomAssignmentWindow viewModel)
        //    {
        //        // Create a new guest
        //        Guest newGuest = new Guest { FirstName = "It", LastName = "Works!" };

        //        Group selectedGroup = GetSelectedGroup();

        //        // Check if the selected group and guest are not null
        //        if (selectedGroup != null && newGuest != null)
        //        {
        //            selectedGroup.Guests.Add(newGuest);
        //        }
        //        else
        //        {
        //            MessageBox.Show("Guest not added! Selected group or guest is null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //}

        private void AddNewGuestButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("New guest added...");
            DebugPrintGroupsVarStatus();

            if (DataContext is RoomAssignmentWindow viewModel)
            {
                // Check if there is a selected group
                if (GetSelectedGroup() != null)
                {
                    if (ButtonBelongsToCorrectGroupTicket(sender))
                    {
                        AddGuestToGroupTicket();
                    }
                    else
                    {
                        MessageBox.Show("Button does not belong to the selected group.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("No group selected. \nSelect a group by double-clicking it!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
        }
        private void AddGuestToGroupTicket()
        {
            // Create a new guest
            Guest newGuest = new Guest { FirstName = "It", LastName = "Works!" };

            Group selectedGroup = GetSelectedGroup();

            // Check if the selected group and guest are not null
            if (selectedGroup != null && newGuest != null)
            {
                selectedGroup.Guests.Add(newGuest);
            }
            else
            {
                MessageBox.Show("Guest not added! Selected group or guest is null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                    else
                    {
                        MessageBox.Show("DEBUG: Button does not belong to the correct group ticket.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            // Check if there is a selected item
            if (listViewElement.SelectedItem != null)
            {
                // Assuming your Group object is stored in the DataContext of the selected ListViewItem
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

        //todo:
        /* add numbering to groups added
         * handle guest addition
         * fix the looks
         * make the buttons less obtrusive
         * make the fields (in the ui) not readonly
         * get on reading the contents of the groups after you do that, to verify wether observablecollection works
         * possible dropdown, to hide guests?
         */
    }
}

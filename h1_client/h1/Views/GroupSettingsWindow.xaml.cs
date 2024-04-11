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

namespace h1.Views
{
    /// <summary>
    /// Interaction logic for GroupSettingsWindow.xaml
    /// </summary>
    public partial class GroupSettingsWindow : Window
    {
        public GroupSettingsWindow(Group a)
        {
            InitializeComponent();
            if (CheckIfGroupSizeAboveMax(a))
            {
                TurnOffTogetherCheckbox(a); //check if this instatiates properly
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

        private void TurnOffTogetherCheckbox(Group a)
        {
            a.WantGroupToStayTogether = false; //todo: verify whether "keep together" property is properly removed
            KeepTogetherCheckbox.IsEnabled = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle saving changes made in the settings window
            // You can access the Group object through the DataContext property
            if (DataContext is Group group)
            {
                // Perform any additional logic or validation if needed
                // Save changes, update database, etc.

                // Close the window
                Close();
            }
        }
    }
}

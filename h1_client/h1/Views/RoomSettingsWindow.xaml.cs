using h1.Models;
using System;
using System.CodeDom;
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
    public partial class RoomSettingsWindow : Window
    {
        public RoomSettingsWindow()
        {
            InitializeComponent();
            // Assuming you set the DataContext to an instance of RoomSettingsViewModel in the constructor or elsewhere.
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            // You should have a RoomSettingsViewModel as the DataContext. Cast DataContext accordingly.
            if (DataContext is RoomSettingsWindow roomSettingsViewModel)
            {
                // Perform save operation or update the corresponding Room model
                // Access roomSettingsViewModel properties to get the updated values

                // For example:
                //Room roomToUpdate = new Room
                //{
                //    Capacity = roomSettingsViewModel.Capacity,
                //    NoiseReduction = roomSettingsViewModel.NoiseReduction,
                //    SecurityFeatures = roomSettingsViewModel.SecurityFeatures,
                //    SmartLighting = roomSettingsViewModel.SmartLighting,
                //    Balcony = roomSettingsViewModel.Balcony,
                //    ModularFurniture = roomSettingsViewModel.ModularFurniture
                //};

                // Now you can save the updated Room model or perform any necessary operations.
                // You might want to use a service or repository to handle the actual save operation.

                // Close the window after saving
                Close();
            }
        }
    }
}

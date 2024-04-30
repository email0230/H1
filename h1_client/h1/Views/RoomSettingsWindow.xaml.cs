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
        }

     
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoomSettingsWindow roomSettingsViewModel)
            {
                Close();
            }

            Close(); //remove this one after validation has been implemented
        }
    }
}

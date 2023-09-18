using h1.Models;
using MongoDB.Bson.IO;
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
using Newtonsoft.Json;

namespace h1.Views
{
    /// <summary>
    /// Interaction logic for HotelDesignerWindow.xaml
    /// </summary>
    public partial class HotelDesignerWindow : Window
    {
		Hotel hotel = Hotel.GetInstance();

		public HotelDesignerWindow() //TODO: insert prior data into the textbox fields prior to pressing the submit button
		{
            InitializeComponent();
            HotelNameTextBox.Text = hotel.Name; //could be extracted to a separate function
        }

		private void AddRoomButton_Click(object sender, RoutedEventArgs e)
		{
            throw new NotImplementedException();
        }

		private void SubmitButton_Click(object sender, RoutedEventArgs e)
		{
			string name = HotelNameTextBox.Text;
            hotel.Name = name;
            hotel.LastModifiedDate = DateTime.Now;

            if (FormValidator.ValidateHotelForm(name))
            {
                // If validation passes, continue with the submission
                string jsonHotel = Newtonsoft.Json.JsonConvert.SerializeObject(hotel);
                DBMethods.StoreHotel(jsonHotel);
                MessageBox.Show("Hotel data logged", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }

            /* BIG THINGS TO DO:
             * - validation (ctnd)
             * - adding rooms in the ui
             * - add a "room list is null!" warning
             * 
             * 
             * 
             */
		}
	}
}

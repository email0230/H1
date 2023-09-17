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
        public HotelDesignerWindow()
        {
            InitializeComponent();
        }

		private void AddRoomButton_Click(object sender, RoutedEventArgs e)
		{
            throw new NotImplementedException();
        }

		private void SubmitButton_Click(object sender, RoutedEventArgs e)
		{
            Hotel hotel = Hotel.GetInstance();

			//TODO: insert prior data into the textbox fields prior to pressing the submit button

			string name = HotelNameTextBox.Text;
            hotel.Name = name;

            //mayhaps validation is in order????

			MessageBox.Show("Hotel data logged", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			string jsonHotel = Newtonsoft.Json.JsonConvert.SerializeObject(hotel);

            DBMethods.StoreHotel(jsonHotel);

			Close();

            /* BIG THINGS TO DO:
             * - validation
             * - adding rooms in the ui
             * - remove "location", who needs that
             * - i want a persistent hotel that can survive an application shutdown, perhaps a local file storing a json of the relevant information??
             * - add a "room list is null!" warning
             * 
             * 
             * 
             */
		}
	}
}

using MongoDB.Driver;
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
    /// Interaction logic for GroupStayDurationWindow.xaml
    /// </summary>
    /// 



    public class DateSelectedEventArgs : EventArgs //todo: extract this to its own file
    {
        public DateTime Arrival { get; }
        public DateTime Departure { get; }

        public DateSelectedEventArgs(DateTime arrival, DateTime departure)
        {
            Arrival = arrival;
            Departure = departure;
        }
    }

    public partial class GroupStayDurationWindow : Window
    {
        public DateTime arrival { get; set; }
        public DateTime departure { get; set; }

        public Action<DateTime, DateTime> PassDatesEvent;


        public GroupStayDurationWindow()
        {
            InitializeComponent();
            InitDatePickerDefaults();
            UpdateStayDuration();
        }

        private void InitDatePickerDefaults()
        {
            Arrival_picker.SelectedDate = DateTime.Now;
            Departure_picker.SelectedDate = DateTime.Now.AddDays(1);
        }

        private void Arrival_picker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => UpdateStayDuration();
        
        private void Departure_picker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => UpdateStayDuration();
        
        private void UpdateStayDuration()
        {
            DateTime arrivalDate = Arrival_picker.SelectedDate ?? DateTime.Now;
            DateTime departureDate = Departure_picker.SelectedDate ?? DateTime.Now.AddDays(1); 

            TimeSpan difference = departureDate - arrivalDate;

            Days_TextBlock.Text = $"This group will stay for {difference.TotalDays} days";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            arrival = Arrival_picker.SelectedDate ?? DateTime.Now; //todo: handle these defaults
            departure = Departure_picker.SelectedDate ?? DateTime.Now.AddDays(1);

            if (PassDatesEvent != null) 
            {
                PassDatesEvent(arrival, departure);
            }

            Close();
        }
    }
}

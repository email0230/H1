using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace h1.Models
{
    public class Group : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Guest> guests;
        public ObservableCollection<Guest> Guests
        {
            get { return guests; }
            set
            {
                if (value != guests)
                {
                    guests = value;
                    OnPropertyChanged(nameof(Guests));
                }
            }
        }

        private bool myProperty; //likely to get removed soon
        public bool MyProperty
        {
            get { return myProperty; }
            set
            {
                myProperty = value;
                OnPropertyChanged();
            }
        }

        private string? groupName;
        public string? GroupName
        {
            get { return groupName; }
            set
            {
                groupName = value;
                OnPropertyChanged();
            }
        }
        public bool WantGroupToStayTogether { get; set; }
        public bool WantNoiseReduction { get; set; }
        public bool WantSecurityFeatures { get; set; }
        public bool WantSmartLighting { get; set; }
        public bool WantBalcony { get; set; }
        public bool WantModularFurniture { get; set; }


        public Group(ObservableCollection<Guest> guestsInput, string? groupName)
        {
            Guests = guestsInput;
            GroupName = groupName;
        }

        //public bool CheckIfGuestExists(Guest input)
        //{
        //    foreach (Guest g in Guests)
        //        if (input == g)
        //            return true;
        //    return false;
        //}
        public bool CheckIfGuestExists(Guest input)
        {
            return Guests.Any(g => g == input);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
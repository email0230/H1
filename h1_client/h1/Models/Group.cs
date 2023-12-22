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

        private bool myProperty;
        public bool MyProperty
        {
            get { return myProperty; }
            set
            {
                myProperty = value;
                OnPropertyChanged();
            }
        }

        private string? debugString;
        public string? DebugString
        {
            get { return debugString; }
            set
            {
                debugString = value;
                OnPropertyChanged();
            }
        }

        public Group(ObservableCollection<Guest> guestsInput, string? debugStringInput)
        {
            Guests = guestsInput;
            DebugString = debugStringInput;
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
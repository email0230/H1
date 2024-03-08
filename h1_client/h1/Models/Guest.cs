using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace h1.Models
{
    public class Guest : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string lastName;
        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;
                OnPropertyChanged();
            }
        }

        private string firstName;
        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
                OnPropertyChanged();
            }
        }

        [BsonId]
        public ObjectId _id { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        //IMPORTANT: two datetime propertie above are absolutely neccessary, change it to requires instead of nullable after testing is done!
        public int? AssignedRoomNumber { get; set; }
        //other properties might also need to use onpropchanged, but only if they are meant to be displayed in a UI

        #region Less important properties
        public int? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        #endregion

        public void NotifyAssignedRoomNumberChanged()
        {
            OnPropertyChanged(nameof(AssignedRoomNumber));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
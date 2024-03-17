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
	[BsonIgnoreExtraElements] //my beloved
	public class Room : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; private set; }
		public int Capacity { get; set; } //max capacity of a room

        private int _occupancy;

        public int Occupancy
        {
            get => _occupancy;
            set
            {
                if (_occupancy != value)
                {
                    _occupancy = value;
                    OnPropertyChanged(nameof(Occupancy));
                }
            }
        }

        public List<Guest> Guests { get; set; }
        //public List<Guest> Guests = new List<Guest>();
		private static int INCREMENT = 1;

        public bool NoiseReduction { get; set; }
        public bool SecurityFeatures { get; set; }
        public bool SmartLighting { get; set; }
        public bool Balcony { get; set; }
        public bool ModularFurniture { get; set; }

        public Room() // Give each room a unique ID number
        {
            Id = INCREMENT;
            INCREMENT++;

            Guests = new List<Guest>();
            Occupancy = Guests.Count;
        }

        
        public bool AddGuest(Guest guest)
		{
			if (Guests.Count < Capacity)
			{
				Guests.Add(guest);
				return true; // Guest added successfully
			}
			else
			{
				return false; // Room is already at capacity
			}
		}

		public bool RemoveGuest(Guest guest)
		{
			if (Guests.Contains(guest))
			{
				Guests.Remove(guest);
				return true; // Guest removed successfully
			}
			else
			{
				return false; // Guest not found in the room
			}
		}

		public List<Guest> GetGuests()
		{
			return Guests;
		}

		public bool IsRoomOccupied()
		{
			return Guests.Count >= 1;
		}

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

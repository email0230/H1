using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1.Models
{
	[BsonIgnoreExtraElements] //my beloved
	public class Room
    {
        public int Id { get; private set; }
		public int Capacity { get; set; } //might need to change name to occupancy
		private List<Guest> Guests = new List<Guest>();
		private static int INCREMENT = 1;

        public bool NoiseReduction { get; set; }
        public bool SecurityFeatures { get; set; }
        public bool SmartLighting { get; set; }
        public bool Balcony { get; set; }
        public bool ModularFurniture { get; set; }

        public Room() //give each room a unique ID number
        {
            Id = INCREMENT;
            INCREMENT++;
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
	}
}

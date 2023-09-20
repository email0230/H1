using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1.Models
{
	[BsonIgnoreExtraElements]
	public class Room
    {
        public int Id { get; private set; }
		public int Capacity { get; set; } //might need to change name to occupancy
		public Guest[] Occupants { get; set; }
        //public int GuestCount => Occupants.Length;
		private static int INCREMENT = 1;

		public Room() //give each room a unique ID number
        {
            Id = INCREMENT;
            INCREMENT++;
        }
    }
}

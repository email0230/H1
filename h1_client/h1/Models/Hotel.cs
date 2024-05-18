using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace h1.Models
{
	[BsonIgnoreExtraElements] //to prevent id error from deserialization line in Fetch()
	public sealed class Hotel
	{
		public string Name { get; set; }
		public List<Room> Rooms { get; set; }
		public DateTime LastModifiedDate { get; set; }

		private Hotel() { }
		private static Hotel _instance;

		public static Hotel GetInstance()
		{
			if (_instance == null)
			{
				//_instance = new Hotel();
				_instance = GetHotelStateFromDB();
			}
			return _instance;
		}

        /*
		NOTES:
		-Hotel gets made at the very start of the cycle, before it gets the value from the text box
		-If there is a different object submitted in the form, h1 will STILL try to fetch the old one (10/03, probably outdated)
	    */

		private static Hotel GetHotelStateFromDB()
		{
			BsonDocument hotelString = DBMethods.GetHotel();
            Hotel a = hotelString != null ? BsonSerializer.Deserialize<Hotel>(hotelString) : new Hotel();

            return a;
		}

        public int[] GetHotelRoomCounts()
		{
			int[] counts = new int[3];
            try //handle first start with no hotel instance active
            {
                counts[0] = _instance.Rooms.Count(room => room.Capacity == 1);
                counts[1] = _instance.Rooms.Count(room => room.Capacity == 2);
                counts[2] = _instance.Rooms.Count(room => room.Capacity == 3);
            }
			catch (ArgumentNullException)
            {
                counts[0] = 1;
                counts[1] = 1;
                counts[2] = 1;
            }

			return counts;
		}

        public Room? FindRoomById(int id) => Rooms.FirstOrDefault(room => room.Id == id);
    }

}

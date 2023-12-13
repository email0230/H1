using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		-If there is a different object submitted in the form, h1 will STILL try to fetch the old one
	*/

		private static Hotel GetHotelStateFromDB()
		{
			BsonDocument hotelString = DBMethods.GetHotel(); //add async to db calls?
            return hotelString != null ? BsonSerializer.Deserialize<Hotel>(hotelString) : null;
		}

		public int[] GetHotelRoomCounts()
		{
			int[] counts = new int[3];

			counts[0] = _instance.Rooms.Count(room => room.Capacity == 1);
			counts[1] = _instance.Rooms.Count(room => room.Capacity == 2);
			counts[2] = _instance.Rooms.Count(room => room.Capacity == 3);

			return counts;
		}
		public void someBusinessLogic()
		{
			// ...
		}
	}

}

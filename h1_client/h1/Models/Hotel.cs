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
			BsonDocument hotelString = DBMethods.GetHotel(); //add async to db calls?
			var a = hotelString != null ? BsonSerializer.Deserialize<Hotel>(hotelString) : null;
			DebugCheckIfRoomsHaveProps(a);

            return a;
		}

        private static void DebugCheckIfRoomsHaveProps(Hotel? a)
        {
            Debug.WriteLine("Debug - displaying room properties");
            Debug.Indent();
            foreach (var item in a.Rooms)
            {
                Debug.WriteLine($"Room #{item.Id}");
                Debug.Indent();
                Debug.WriteLine($"NoiseReduction: {item.NoiseReduction}");
                Debug.WriteLine($"SecurityFeatures: {item.SecurityFeatures}");
                Debug.WriteLine($"SmartLighting: {item.SmartLighting}");
                Debug.WriteLine($"Balcony: {item.Balcony}");
                Debug.WriteLine($"ModularFurniture: {item.ModularFurniture}");
                Debug.Unindent();
            }
            Debug.Unindent();
        }

        public int[] GetHotelRoomCounts()
		{
			int[] counts = new int[3];

			counts[0] = _instance.Rooms.Count(room => room.Capacity == 1);
			counts[1] = _instance.Rooms.Count(room => room.Capacity == 2);
			counts[2] = _instance.Rooms.Count(room => room.Capacity == 3);

			return counts;
		}

        public Room FindRoomById(int roomId)
        {
            foreach (var room in Rooms)
            {
                if (room.Id == roomId)
                {
                    return room;
                }
            }
            return null; //todo: handle this
        }
    }

}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;
using h1.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace h1
{
    public static class DBMethods
    {
        #region Fields 
        static MongoClient client = new MongoClient();
        static IMongoDatabase database = client.GetDatabase("h1_db");
        static IMongoCollection<Guest> GuestCollection = database.GetCollection<Guest>("GuestCollection");
        static IMongoCollection<BsonDocument> HotelDataCollection = database.GetCollection<BsonDocument>("HotelDataCollection");
        static IMongoCollection<Room> HotelRoomCollection = database.GetCollection<Room>("HotelRoomCollection");
        #endregion

        public static void Insert(Guest guest)
        {
            IMongoCollection<Guest> collection = GuestCollection;
            collection.InsertOne(guest);
        }

        public static void StoreHotel(Hotel input)
        {
            string jsonHotel = Newtonsoft.Json.JsonConvert.SerializeObject(input);
            StoreHotel(jsonHotel); //easy to miss, this is the same method, different overload
        }

        public static void StoreHotel(string jsonInput)
        {
            HotelDataCollection.DeleteMany(FilterDefinition<BsonDocument>.Empty);

            List<Room> roomsCorrected = RemoveDepartedGuests();

            RemoveRooms();

            StoreRooms(roomsCorrected);
            
            BsonDocument bsonDocument = BsonDocument.Parse(jsonInput);
            HotelDataCollection.InsertOne(bsonDocument);
        }

        private static List<Room> RemoveDepartedGuests()
        {
            DateTime departureDate = DateTime.Now;

            var filter = Builders<Guest>.Filter.Lt("DepartureDate", departureDate);
            List<Guest> guests = GuestCollection.Find(filter).ToList();
            List<Room> allRooms = GetFullListOfRooms();

            if (guests.Count == 0)
            {
                return allRooms; //no valid guests found, continue as normal
            }
            
            for (int i = 0; i < guests.Count; i++)
            {
                Guest checkedGuest = guests[i];

                Room? roomWithGuest = allRooms.FirstOrDefault(room => room.Guests.Any(guest => guest._id == checkedGuest._id));
                if (roomWithGuest != null)
                {
                    Debug.WriteLine($"Guest found in room {roomWithGuest.Id}");
                    if (roomWithGuest.RemoveGuest(checkedGuest)) //remove from the granular room list
                    {
                        Debug.Write("Guest removed successfully.");
                    }
                    else
                    {
                        Debug.WriteLine("ERROR: Guest removal unsuccessful!");
                    }
                }
            }

            MessageBox.Show($"{guests.Count} guests have departed, and have been removed from the app.",
                            "Guests were removed.",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            GuestCollection.DeleteMany(filter); //remove from guest collection
            return allRooms;
        }

        public static BsonDocument GetHotel()
        {
            return HotelDataCollection.Find(new BsonDocument()).FirstOrDefault();
        }

        public static void StoreRooms(List<Room> rooms)
        {
            if (rooms == null)
            {
                throw new NullReferenceException();
            }

            RemoveRooms();

            foreach (var room in rooms)
            {
                HotelRoomCollection.InsertOne(room);
            }
        }

        public static void RemoveRooms() => HotelRoomCollection.DeleteMany(FilterDefinition<Room>.Empty);

        public static List<Guest> GetGuests()
        {
            var a = GuestCollection.Find(new BsonDocument()).ToList();
            StoreRooms(RemoveDepartedGuests());

            return a;
        }

        public static void StoreGuest(Guest guest) => GuestCollection.InsertOne(guest);

        public static void DeleteAllGuests() => GuestCollection.DeleteMany(FilterDefinition<Guest>.Empty);

        public static List<Room> GetFullListOfRooms()
        {
            List<Room> output;
            output = HotelRoomCollection.Find(new BsonDocument()).ToList();
            return output;
        }


        public static (int occupancy, int capacity) GetHotelOccupancyAndCapacity()
        {
            List<Room> rooms = Hotel.GetInstance().Rooms;
            int totalCapacity = 0, totalOccupancy = 0;

            foreach (var room in rooms)
            {
                totalCapacity += room.Capacity;
                totalOccupancy += room.Occupancy;
            }

            return (totalOccupancy, totalCapacity);
        }
    }
}

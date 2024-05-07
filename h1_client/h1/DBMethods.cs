﻿using System;
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
            RemoveRooms(); //placed here as an attempt to remove bleeding occupancy over resets
            StoreRooms(RemoveDepartedGuests());

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

            GuestCollection.DeleteMany(filter); //remove from guest collection

            MessageBox.Show($"{guests.Count} guests have departed, and have been removed from the app.",
                            "Guests were removed.",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
            
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

        private static void RemoveRooms() => HotelRoomCollection.DeleteMany(FilterDefinition<Room>.Empty);

        public static List<Guest> GetGuests()
        {
            var a = GuestCollection.Find(new BsonDocument()).ToList();
            StoreRooms(RemoveDepartedGuests());

            return a;
        }

        public static void StoreGuest(Guest guest) => GuestCollection.InsertOne(guest);

        //TODO: find a use for this one, or delete it
        public static void DeleteGuest(Guest guest)
        {
            // Check if the guest has a valid _id
            if (guest._id == ObjectId.Empty)
            {
                // Handle the case where the guest doesn't have a valid _id
                throw new InvalidOperationException("Guest _id is not valid for deletion.");
            }

            // Create a filter to find the guest by _id
            var filter = Builders<Guest>.Filter.Eq(g => g._id, guest._id);

            // Delete the guest from the guest collection
            DeleteResult result = GuestCollection.DeleteOne(filter);

            // Check if the deletion was successful
            if (result.DeletedCount == 0)
            {
                // Handle the case where the guest was not found for deletion
                throw new InvalidOperationException($"Guest with _id {guest._id} not found for deletion.");
            }
        }

        public static void DeleteAllGuests() => GuestCollection.DeleteMany(FilterDefinition<Guest>.Empty);

        public static List<Room> GetFullListOfRooms()
        {
            List<Room> output;
            output = HotelRoomCollection.Find(new BsonDocument()).ToList();
            return output;
        }


        public static (int occupancy, int capacity) GetHotelOccupancyAndCapacity()
        {
            List<Room> rooms = GetFullListOfRooms();
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

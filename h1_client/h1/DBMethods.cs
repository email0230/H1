using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        static IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("testCollection");
        static IMongoCollection<Guest> GuestCollection = database.GetCollection<Guest>("GuestCollection");
        static IMongoCollection<BsonDocument> HotelDataCollection = database.GetCollection<BsonDocument>("HotelDataCollection");
        static IMongoCollection<BsonDocument> HotelRoomCollection = database.GetCollection<BsonDocument>("HotelRoomCollection");
        #endregion

        public static void Insert(Guest guest)
        {
            IMongoCollection<Guest> collection = GuestCollection;
            collection.InsertOne(guest);
        }

        public static void StoreHotel(string jsonInput)
        {
            // Clear the collection
            HotelDataCollection.DeleteMany(FilterDefinition<BsonDocument>.Empty);

            BsonDocument bsonDocument = BsonDocument.Parse(jsonInput);
            HotelDataCollection.InsertOne(bsonDocument);
        }

        public static BsonDocument GetHotel()
        {
            return HotelDataCollection.Find(new BsonDocument()).FirstOrDefault();
        }

        public static List<Guest> GetGuests() => GuestCollection.Find(new BsonDocument()).ToList();

        public static void StoreRooms(List<Room> rooms)
        {
            // Convert the list of Room objects to a list of BsonDocument
            List<BsonDocument> bsonDocuments = rooms.Select(room => room.ToBsonDocument()).ToList();

            // Clear the collection
            HotelRoomCollection.DeleteMany(FilterDefinition<BsonDocument>.Empty);

            // Insert the list of BsonDocument into the collection
            HotelRoomCollection.InsertMany(bsonDocuments);
        }

        public static void StoreGuest(Guest guest)
        {
            GuestCollection.InsertOne(guest);
        }
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

        public static void DeleteAllGuests()
        {
            GuestCollection.DeleteMany(FilterDefinition<Guest>.Empty);
        }
    }
}

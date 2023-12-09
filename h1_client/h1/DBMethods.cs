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
		#endregion //add public access mods if problems arise

        public static void Insert(Guest guest)
        {
            IMongoCollection<Guest> collection = GuestCollection;
            collection.InsertOne(guest);
		}
        
        public static void StoreHotel(string jsonInput)
        {
			IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("HotelDataCollection");
			BsonDocument bsonDocument = BsonDocument.Parse(jsonInput);
			collection.InsertOne(bsonDocument);
		}

		public static BsonDocument GetHotel()
		{
			// Sort the data by date in descending order (most recent first)
			var sort = Builders<BsonDocument>.Sort.Descending("LastModifiedDate"); // Replace "DateField" with your actual date field name

			// Perform the query with sorting
			return HotelDataCollection.Find(new BsonDocument()).Sort(sort).FirstOrDefault();
		}
		public static List<Guest> GetGuests() => GuestCollection.Find(new BsonDocument()).ToList();
	}
}

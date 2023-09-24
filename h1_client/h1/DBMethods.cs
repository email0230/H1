﻿using System;
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

		static void InsertAGuy()
        {
            var date = new DateTime(2023, 4, 28, 14, 30, 0, DateTimeKind.Utc);
            var unixTimestamp = (long)(date - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            var arrivalBsonDate = new BsonDateTime(unixTimestamp);

            var date2 = new DateTime(2023, 4, 30, 14, 30, 0, DateTimeKind.Utc);
            var unixTimestamp2 = (long)(date2 - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            var departureBsonDate = new BsonDateTime(unixTimestamp2);
            var document = new BsonDocument
            {
                { "LastName", "John" },
                { "FirstName", "Doe" },
                { "ArrivalDate", arrivalBsonDate },
                { "DepartureDate", departureBsonDate },
                { "AssignedRoom", 10000 } //for testing :DD
            };

            collection.InsertOne(document);
        }
        public static void Insert(string name, DateTime? arrivalDate, DateTime? departureDate, string roomNumber)
        {
			var formData = new FormData
			{
				Name = name,
				ArrivalDate = arrivalDate.Value,
				DepartureDate = departureDate.Value,
				RoomNumber = roomNumber
			};

			var collection = database.GetCollection<FormData>("GuestCollection"); // Replace with your collection name
			collection.InsertOne(formData);
		}

        public static void DebugMethod()
        {
            //InsertAGuy();
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    //FilterDefinition<Guest> filter = Builders<Guest>.Filter.Eq("AssignedRoom", roomid);
                    //List<Guest> outputCollection = GuestCollection.Find(filter).ToList();
                    //foreach (var item in outputCollection)
                    //{
                    //    //do stuff haha
                    //}


                    //((MainWindow)window).test_textblock.ItemsSource = outputCollection;
                    //((MainWindow)window).test_textblock.Text = collection.ToString();
                }
            }
            
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

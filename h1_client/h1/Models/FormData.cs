using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1.Models
{
	public class FormData
	{
		public ObjectId Id { get; set; } // MongoDB document ID
		public string Name { get; set; }
		public DateTime ArrivalDate { get; set; }
		public DateTime DepartureDate { get; set; }
		public string RoomNumber { get; set; }
	}
}

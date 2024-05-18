using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1.Models
{
	public class FormData //todo: delete this, not used anywhere
	{
		public ObjectId Id { get; set; }
		public string Name { get; set; }
		public DateTime ArrivalDate { get; set; }
		public DateTime DepartureDate { get; set; }
		public string RoomNumber { get; set; }
	}
}

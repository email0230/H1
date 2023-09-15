using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1.Models
{
	public sealed class Hotel
	{
		public string Name { get; set; }
		public int TotalRooms { get; set; }
		public List<Room> Rooms { get; set; }

		private Hotel() { }
		private static Hotel _instance;

		public static Hotel GetInstance()
		{
			if (_instance == null)
			{
				_instance = new Hotel();
			}
			return _instance;
		}

		public void someBusinessLogic()
		{
			// ...
		}
	}

}

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
		//public int TotalRooms { get; set; }
		//public List<Room> Rooms { get; set; }

		// The Singleton's constructor should always be private to prevent
		// direct construction calls with the `new` operator.
		private Hotel() { }

		// The Singleton's instance is stored in a static field. There there are
		// multiple ways to initialize this field, all of them have various pros
		// and cons. In this example we'll show the simplest of these ways,
		// which, however, doesn't work really well in multithreaded program.
		private static Hotel _instance;

		// This is the static method that controls the access to the singleton
		// instance. On the first run, it creates a singleton object and places
		// it into the static field. On subsequent runs, it returns the client
		// existing object stored in the static field.
		public static Hotel GetInstance()
		{
			if (_instance == null)
			{
				_instance = new Hotel();
			}
			return _instance;
		}

		// Finally, any singleton should define some business logic, which can
		// be executed on its instance.
		public void someBusinessLogic()
		{
			// ...
		}
	}

}

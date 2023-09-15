using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1.Models
{
    public class Room
    {
        public int Id { get; private set; }
		public Guest[] Occupants { get; set; }
		public int Occupancy => Occupants.Length;
		private static int INCREMENT = 1;

		public Room() //give each room a unique ID number
        {
            Id = INCREMENT;
            INCREMENT++;
        }
    }
}

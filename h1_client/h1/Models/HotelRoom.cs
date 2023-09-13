using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1.Models
{
    class HotelRoom
    {
        private static int Id_increment = 1;
        public int Id { get; private set; }

        public HotelRoom() //give each room a unique ID number
        {
            Id = Id_increment;
            Id_increment++;
        }
    }
}

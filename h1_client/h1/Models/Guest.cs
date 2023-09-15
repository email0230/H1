using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1.Models
{
    public class Guest
    {
        //give guests ID numbers?
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public int AssignedRoom { get; set; }
        #region Less important properties
        public ObjectId _id { get; set; }
        public int? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        #endregion
    }
}

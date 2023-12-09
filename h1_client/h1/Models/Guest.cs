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
        public ObjectId _id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        //IMPORTANT: two datetime propertie above are absolutely neccessary, change it to requires instead of nullable after testing is done!
        public int? AssignedRoomNumber { get; set; }
        #region Less important properties
        public int? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        #endregion
    }
}

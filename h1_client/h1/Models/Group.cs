using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1.Models
{
    public class Group
    {
        public Guest[] Guests { get; set; }
        public bool MyProperty { get; set; }

        public bool CheckIfGuestExists(Guest input)
        {
            foreach (Guest g in Guests)
                if (input == g)
                    return true;
            return false;
        }
    }
}

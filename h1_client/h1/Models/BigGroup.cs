using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1.Models
{
    class BigGroup
    {
        public Group[] Groups { get; set; }

        //consider reservations for groups containted within a set as a big transaction, otherwise i cant really justify why sets/biggroups of guests exist right now
    }
}

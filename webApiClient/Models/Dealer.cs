using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webApiClient.Models
{
    class Dealer
    {
        
        public int dealerId { get; set; }
        public string name { get; set; }
        public List<Vehicle> vehicles { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webApiClient.Models
{
    class Answer
    {
        public List<Dealer> dealers { get; set; }

        /*public Dealer Dealer { get; set; }
        public List<Vehicle> Vehicles { get; set; }*/

        public Answer()
        {
            dealers = new List<Dealer>();
        }

    }
}

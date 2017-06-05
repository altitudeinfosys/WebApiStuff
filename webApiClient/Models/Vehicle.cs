using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace webApiClient.Models
{
    class Vehicle
    {
        public int vehicleId { get; set; }
        public int year { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public int dealerId { get; set; }

        public bool ShouldSerializedealerId()
        {
            return (dealerId !=0);
        }

    }

}

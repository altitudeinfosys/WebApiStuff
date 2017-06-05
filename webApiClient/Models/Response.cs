using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webApiClient.Models
{
    class Response
    {
        
            public bool success { get; set; }
            public string message { get; set; }
            public int totalMilliseconds { get; set; }
    }
}

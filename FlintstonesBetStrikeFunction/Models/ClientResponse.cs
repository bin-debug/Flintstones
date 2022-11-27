using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintstonesBetStrikeFunction.Models
{
    public class ClientResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public double Balance { get; set; }
    }
}

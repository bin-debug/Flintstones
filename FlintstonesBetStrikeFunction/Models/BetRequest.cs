using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintstonesBetStrikeFunction.Models
{
    public class BetRequest
    {
        public string ClientID { get; set; }
        public string Token { get; set; }
        public decimal StakeAmount { get; set; }
        public string Market { get; set; }
        public int Selection { get; set; }
        public decimal SelectionOdd { get; set; }
        public int Duration { get; set; }
    }
}

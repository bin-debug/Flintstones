using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintstonesModels
{
    public class BetDTO
    {
        public Guid BetID { get; set; }
        public int ClientID { get; set; }
        public string Token { get; set; }
        public decimal StakeAmount { get; set; }
        public string Market { get; set; }
        public int Selection { get; set; }
        public decimal SelectionOdd { get; set; }
        public int Duration { get; set; }
        public decimal CurrentMarketPrice { get; set; }
        public string Tag { get; set; }
        public decimal Payout { get; set; }
        public decimal AutoCashoutValue { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

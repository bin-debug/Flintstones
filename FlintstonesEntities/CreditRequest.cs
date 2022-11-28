using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintstonesEntities
{
    public class CreditRequest
    {
        public string ClientID { get; set; }
        public double WinAmount { get; set; }
        public decimal CashoutAmount { get; set; }
        public bool Cashout { get; set; }
        public double ResultMarketPrice { get; set; }
        public DateTime? CashoutCreatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

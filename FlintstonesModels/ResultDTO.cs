using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintstonesModels
{
    public class ResultDTO
    {
        public Guid ResultID { get; set; }
        public Guid BetID { get; set; }
        public string Market { get; set; }
        public decimal WinAmount { get; set; }
        public decimal CashoutAmount { get; set; }
        public int Cashout { get; set; }
        public decimal ResultMarketPrice { get; set; }
        public string Tag { get; set; }
        public DateTime? CashoutCreatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FlintstonesModels
{
    public class ResultDTO
    {
        [JsonProperty(PropertyName = "id")]
        public Guid BetID { get; set; }
        public Guid ResultID { get; set; }
        public decimal WinAmount { get; set; }
        public decimal CashoutAmount { get; set; }
        public bool Cashout { get; set; }
        public decimal ResultMarketPrice { get; set; }
        public string Tag { get; set; }
        public DateTime? CashoutCreatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

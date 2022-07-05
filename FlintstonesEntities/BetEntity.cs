using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintstonesEntities
{
    public class BetEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string Token { get; set; }
        public int StatusID { get; set; } // 1: Active, 2: Win, 3: Lose, 4: Refund
        public decimal StakeAmount { get; set; }
        public string Market { get; set; }
        public int Selection { get; set; }
        public decimal SelectionOdd { get; set; }
        public int Duration { get; set; }
        public double CurrentMarketPrice { get; set; }
        public string? Tag { get; set; }
        public decimal TotalPayout { get { return StakeAmount * SelectionOdd; } }
        public DateTime CreatedDate { get; set; }
    }
}

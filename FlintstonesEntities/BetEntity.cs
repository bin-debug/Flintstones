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
        //public BetEntity()
        //{
        //    Result = new ResultDTO();
        //}

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string Token { get; set; }
        public decimal StakeAmount { get; set; }
        public string Market { get; set; }
        public int Selection { get; set; }
        public decimal SelectionOdd { get; set; }
        public int Duration { get; set; }
        public double CurrentMarketPrice { get; set; }
        public string? Tag { get; set; }
        public decimal TotalPayout { get { return StakeAmount * SelectionOdd; } }
        //public ResultDTO Result { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

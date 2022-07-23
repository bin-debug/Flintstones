using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintstonesEntities
{
    public class MarketEntity : ITableEntity
    {
        public string PartitionKey { get; set; } //symbol
        public string RowKey { get; set; } //duration - direction
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string ID { get; set; }
        public double BaseOdds { get; set; }
        public int Duration { get; set; }
        public int Direction { get; set; }

    }
}

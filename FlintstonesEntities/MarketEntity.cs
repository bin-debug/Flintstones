﻿using Azure;
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
        public string PartitionKey { get; set; } //MARKETS
        public string RowKey { get; set; } //symbol-duration-direction
        public string MarketName { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string ID { get; set; }
        public double BaseOdds { get; set; }
        public int Duration { get; set; }
        public int Direction { get; set; }
        public bool Active { get; set; }
    }
}

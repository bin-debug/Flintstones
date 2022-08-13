using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintstonesEntities
{
    public class BOSummaryEntity : ITableEntity
    {
        public string Activity { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public int NumberOfBets { get; set; }
        public double TotalStake { get; set; }
        public double TotalPayout { get; set; }
        public int BrokenHeartbeats { get; set; }
    }
}

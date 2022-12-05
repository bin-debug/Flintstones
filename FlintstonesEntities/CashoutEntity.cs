using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintstonesEntities
{
    public class CashoutEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public double CashoutValue { get; set; }
        public DateTime? CashoutDate { get; set; }
        public int CashoutStage { get; set; }
        public bool IsCashedOut { get; set; }
        public bool IsExpired { get; set; }
    }
}

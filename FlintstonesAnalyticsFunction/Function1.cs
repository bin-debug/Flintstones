using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using FlintstonesEntities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FlintstonesAnalyticsFunction
{
    public class Function1
    {
        [FunctionName("Function1")]
        public async Task RunAsync([ServiceBusTrigger("rm-misc", Connection = "ConnectionName")]string myQueueItem, ILogger log)
        {
            var serviceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=rmzasa;AccountKey=rOeD6L2O33PrIZOHZMRMA7vmSapOKC9xBQcr20mHrTWe7aewe6N9sXs/tx4uHX4nd+LpfsMBY2jm+ASt7s8zAA==;EndpointSuffix=core.windows.net");
            var tableClient = serviceClient.GetTableClient("BACKOFFICE");

            var summary = JsonConvert.DeserializeObject<BOSummaryEntity>(myQueueItem);

            var queryResultsFilter = tableClient.Query<BOSummaryEntity>(filter: $"PartitionKey eq '{summary.PartitionKey}'");
            var currentRecord = queryResultsFilter.FirstOrDefault();

            var updatedSummaryInfo = new BOSummaryEntity();
            updatedSummaryInfo.PartitionKey = summary.PartitionKey;
            updatedSummaryInfo.RowKey = summary.RowKey;

            if (summary.Activity == "strike")
            {
                updatedSummaryInfo.NumberOfBets = currentRecord.NumberOfBets + 1;
                updatedSummaryInfo.TotalStake = currentRecord.TotalStake + summary.TotalStake;
            }

            if (summary.Activity == "result")
            {
                updatedSummaryInfo.TotalPayout = currentRecord.TotalPayout + summary.TotalPayout;
            }

            await tableClient.UpsertEntityAsync(updatedSummaryInfo);

            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}

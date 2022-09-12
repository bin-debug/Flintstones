using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FlintstonesEntities;
using Azure.Data.Tables;
using System.Linq;

namespace FlintstonesMarketApi
{
    public static class MarketFunction
    {
        [FunctionName("GetMarkets")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var serviceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=rmzasa;AccountKey=rOeD6L2O33PrIZOHZMRMA7vmSapOKC9xBQcr20mHrTWe7aewe6N9sXs/tx4uHX4nd+LpfsMBY2jm+ASt7s8zAA==;EndpointSuffix=core.windows.net");
            var tableClient = serviceClient.GetTableClient("BACKOFFICE");

            string market = req.Query["market"];

            //PartitionKey eq 'MARKETS' and MarketName eq 'BTCUSDT'
            var queryResultsFilter = tableClient.Query<MarketEntity>(filter: $"PartitionKey eq 'MARKETS' and MarketName eq '{market}' ");
            var records = queryResultsFilter.OrderBy(r => r.MarketName);

            log.LogInformation(JsonConvert.SerializeObject(records));

            return new OkObjectResult(JsonConvert.SerializeObject(records));
        }
    }
}

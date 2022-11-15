using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using FlintstonesEntities;
using System.Linq;

namespace FlintstonesMarketApi
{
    public static class LobbyFunction
    {
        [FunctionName("LobbyFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var serviceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=nivs;AccountKey=mHilsEON7rSB84YCK6noL0sWbs8nxX+UjihWeeSawKPPyu0H1yKh40JtMQ/iHDJXS+RE414LM2Th+AStT1MWJg==;EndpointSuffix=core.windows.net");
            var tableClient = serviceClient.GetTableClient("BACKOFFICE");

            //PartitionKey eq 'MARKETS' and MarketName eq 'BTCUSDT'
            var queryResultsFilter = tableClient.Query<LobbyEntity>(filter: $"PartitionKey eq 'LOBBY' and IsMarketActive eq true");
            var records = queryResultsFilter.OrderBy(r => r.MarketName);

            log.LogInformation(JsonConvert.SerializeObject(records));

            return new OkObjectResult(JsonConvert.SerializeObject(records));
        }
    }
}

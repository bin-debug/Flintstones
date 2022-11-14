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

namespace FlintstonesCTFunction
{
    public static class TransactionsFunction
    {
        [FunctionName("GetClientTransactions")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string clientid = req.Query["clientid"];
            string pageSize = req.Query["pagesize"];

            var serviceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=nivs;AccountKey=mHilsEON7rSB84YCK6noL0sWbs8nxX+UjihWeeSawKPPyu0H1yKh40JtMQ/iHDJXS+RE414LM2Th+AStT1MWJg==;EndpointSuffix=core.windows.net");
            var tableClient = serviceClient.GetTableClient("BETS");

            var queryResultsFilter = tableClient.Query<BetEntity>(filter: $"PartitionKey eq '{clientid}'");
            var records = queryResultsFilter.OrderByDescending(r => r.CreatedDate).Take(Convert.ToInt32(pageSize)).ToList();

            log.LogInformation(JsonConvert.SerializeObject(records));

            return new OkObjectResult(JsonConvert.SerializeObject(records));
        }
    }
}

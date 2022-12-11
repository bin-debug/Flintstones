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
using System.Collections.Generic;

namespace FlintstonesMarketApi
{
    public static class SettingsFunction
    {
        [FunctionName("SettingsFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var serviceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=nivs;AccountKey=mHilsEON7rSB84YCK6noL0sWbs8nxX+UjihWeeSawKPPyu0H1yKh40JtMQ/iHDJXS+RE414LM2Th+AStT1MWJg==;EndpointSuffix=core.windows.net");
            var tableClient = serviceClient.GetTableClient("BACKOFFICE");

            var queryResultsFilter = tableClient.Query<SettingEntity>(filter: $"PartitionKey eq 'SETTINGS' and Available eq true");
            var records = queryResultsFilter.ToList();
            log.LogInformation(JsonConvert.SerializeObject(records));

            return new OkObjectResult(JsonConvert.SerializeObject(records));
        }
    }
}

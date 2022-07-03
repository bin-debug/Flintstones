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
using Azure;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace FlintstonesFeedFunctionApi
{
    public static class GetFeedFunction
    {
        [FunctionName("GetFeedFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string feed = req.Query["feed"];
            var conn = Environment.GetEnvironmentVariable("MyConnection");

            log.LogInformation(feed);

            var serviceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=rmzasa;AccountKey=plxf+fIqm/TYanA0vvgPDUBZYS5j3HGoZtcXPP5RByI7t+wfzbtd5v6rNvOgCHfswBt0wGJAtOSP+AStDTNuOw==;EndpointSuffix=core.windows.net");
            var tableClient = serviceClient.GetTableClient(feed);

            string partitionKey = DateTime.Now.ToString("ddMMyyyy"); //"03072022";
            string rowkey = DateTime.Now.AddSeconds(-5).ToString("yyyy-MM-ddTHH:mm:ss"); //2022-07-01T02:00:00
            Pageable<FeedModel> queryResultsFilter = tableClient.Query<FeedModel>(filter: $"PartitionKey eq '{partitionKey}' and RowKey gt '{rowkey}'");

            var model = queryResultsFilter.LastOrDefault();
            var newModel = new { latest_price = model.LastPrice };

            log.LogInformation(JsonConvert.SerializeObject(newModel));

            return new OkObjectResult(newModel);
        }
    }
}
//https://rm-feed.azurewebsites.net/api/GetFeedFunction?feed=BTCUSDT
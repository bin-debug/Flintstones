using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FlintstonesBetStrikeFunction.Models;
using FlintstonesEntities;
using Azure.Data.Tables;
using Azure;
using System.Linq;

namespace FlintstonesBetStrikeFunction
{
    public static class BetStrikeFunction
    {
        [FunctionName("BetStrike")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var serviceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=rmzasa;AccountKey=plxf+fIqm/TYanA0vvgPDUBZYS5j3HGoZtcXPP5RByI7t+wfzbtd5v6rNvOgCHfswBt0wGJAtOSP+AStDTNuOw==;EndpointSuffix=core.windows.net");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<BetRequest>(requestBody);

            // send to client api 

            await Submit(serviceClient, request);

            // send to queue on a schedule

            log.LogInformation("Bet successfully submitted.");
            return new OkObjectResult(request);
        }

        public static async Task<bool> Submit(TableServiceClient serviceClient, BetRequest request)
        {
            var latestPrice = GetPrice(serviceClient, request.Market);

            var bet = new BetEntity();
            bet.PartitionKey = request.ClientID.ToString();
            bet.RowKey = Guid.NewGuid().ToString();
            bet.Token = "token";
            bet.CurrentMarketPrice = latestPrice;
            bet.Duration = request.Duration;
            bet.CreatedDate = DateTime.UtcNow.AddHours(2);
            bet.Market = request.Market;
            bet.Selection = request.Selection;
            bet.SelectionOdd = request.SelectionOdd;
            bet.StakeAmount = request.StakeAmount;
            bet.Tag = "";

            var tableClient = serviceClient.GetTableClient("BETS");
            var result = await tableClient.UpsertEntityAsync(bet);
            if (result.Status == 204)
                return true;
            else
                return false;
        }

        public static double GetPrice(TableServiceClient serviceClient, string market)
        {
            var tableClient = serviceClient.GetTableClient(market);

            string partitionKey = DateTime.Now.ToString("ddMMyyyy"); //"03072022";
            string rowkey = DateTime.Now.AddSeconds(-5).ToString("yyyy-MM-ddTHH:mm:ss"); //2022-07-01T02:00:00
            Pageable<FeedEntity> queryResultsFilter = tableClient.Query<FeedEntity>(filter: $"PartitionKey eq '{partitionKey}' and RowKey gt '{rowkey}'");

            var model = queryResultsFilter.LastOrDefault();
            return model.LastPrice;
        }
    }
}

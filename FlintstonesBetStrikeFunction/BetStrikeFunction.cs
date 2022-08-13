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
using Azure.Messaging.ServiceBus;
using System.Text;

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

            var serviceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=rmzasa;AccountKey=rOeD6L2O33PrIZOHZMRMA7vmSapOKC9xBQcr20mHrTWe7aewe6N9sXs/tx4uHX4nd+LpfsMBY2jm+ASt7s8zAA==;EndpointSuffix=core.windows.net");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<BetRequest>(requestBody);

            // send to client api 

            var insertedBet = await Submit(serviceClient, request);

            if (insertedBet != null)
            {
                // send to queue on a schedule
                await PublishBet(insertedBet);
                await PublishMisc(insertedBet);
            }

            log.LogInformation("Bet successfully submitted.");
            insertedBet.Token = String.Empty;
            insertedBet.Tag = String.Empty;
            return new OkObjectResult(request);
        }

        public static async Task<BetEntity> Submit(TableServiceClient serviceClient, BetRequest request)
        {
            var latestPrice = GetPrice(serviceClient, request.Market);

            var bet = new BetEntity();
            bet.PartitionKey = request.ClientID.ToString();
            bet.RowKey = Guid.NewGuid().ToString();
            bet.Token = "token";
            bet.StatusID = 1;
            bet.CurrentMarketPrice = latestPrice;
            bet.Duration = request.Duration;
            bet.CreatedDate = DateTime.UtcNow.AddHours(2);
            bet.Market = request.Market;
            bet.Selection = request.Selection;
            bet.SelectionOdd = request.SelectionOdd;
            bet.StakeAmount = request.StakeAmount;
            bet.Tag = "tag";

            var tableClient = serviceClient.GetTableClient("BETS");
            var result = await tableClient.UpsertEntityAsync(bet);
            if (result.Status == 204)
                return bet;
            else
                return null;
        }

        public async static Task PublishBet(BetEntity bet)
        {
            string connectionString = "Endpoint=sb://dev-test-rm.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fVb2VD5fJ/RFENSCD44aPYp0Eb9LhFV7/+iFDEK6Hxc=";
            string queueName = "rm-bets";
            var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(queueName);

            var dateTime = DateTime.UtcNow.AddSeconds(bet.Duration);

            var serializedMessage = JsonConvert.SerializeObject(bet);
            var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(serializedMessage));

            await sender.ScheduleMessageAsync(serviceBusMessage,dateTime);
        }

        public async static Task PublishMisc(BetEntity bet)
        {
            string connectionString = "Endpoint=sb://dev-test-rm.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fVb2VD5fJ/RFENSCD44aPYp0Eb9LhFV7/+iFDEK6Hxc=";
            string queueName = "rm-misc";
            var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(queueName);

            var summary = new BOSummaryEntity()
            {
                PartitionKey = DateTime.Now.ToString("ddMMyyyy"),
                RowKey = DateTime.Now.ToString("ddMMyyyy"),
                Activity = "strike",
                TotalStake = bet.StakeAmount,
            };

            var serializedMessage = JsonConvert.SerializeObject(summary);
            var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(serializedMessage));

            await sender.SendMessageAsync(serviceBusMessage);
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

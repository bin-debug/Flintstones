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
using Polly;
using System.Net.Http;
using System.Net.Http.Headers;

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

            var serviceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=nivs;AccountKey=mHilsEON7rSB84YCK6noL0sWbs8nxX+UjihWeeSawKPPyu0H1yKh40JtMQ/iHDJXS+RE414LM2Th+AStT1MWJg==;EndpointSuffix=core.windows.net");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<BetRequest>(requestBody);

            // send to client api 
            var debitURL = GetDebitURL(serviceClient);
            var clientResult = await SendBetToClient(request, debitURL);
            //var clientResult = await SendBetToClient(request, "https://localhost:7184/debit");
            if (clientResult != null && clientResult.StatusCode == 200)
            {
                var insertedBet = await Submit(serviceClient, request);

                if (insertedBet != null)
                {
                    // send to queue on a schedule
                    await PublishBet(insertedBet);
                }

                log.LogInformation("Bet successfully submitted.");
                return new OkObjectResult(clientResult);
            }
            else
                return new BadRequestObjectResult(clientResult);
        }

        public static async Task<ClientResponse> SendBetToClient(BetRequest betRequest, string debitURL)
        {
            var result = new ClientResponse();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", betRequest.Token);
            var content = new StringContent(JsonConvert.SerializeObject(betRequest), Encoding.UTF8, "application/json");

            var maxRetryAttempts = 3;
            var pauseBetweenFailures = TimeSpan.FromSeconds(5);

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures);

            await retryPolicy.ExecuteAsync(async () =>
            {
                var response = await httpClient.PostAsync(debitURL,content);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<ClientResponse>(data);
                }

                response.EnsureSuccessStatusCode();
            });

            return result;
        }

        public static async Task<BetEntity> Submit(TableServiceClient serviceClient, BetRequest request)
        {
            var latestPrice = GetPrice(serviceClient, request.Market);

            var bet = new BetEntity();
            bet.PartitionKey = request.ClientID.ToString();
            bet.RowKey = Guid.NewGuid().ToString();
            bet.Token = request.Token;
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
            string connectionString = "Endpoint=sb://nivash.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=JBWiJC6pKtyMrBr0kdgB9K6NrxDMtoLdJ1PPVIODJsE=";
            string queueName = "bets";
            var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(queueName);

            var dateTime = DateTime.UtcNow.AddSeconds(bet.Duration);

            var serializedMessage = JsonConvert.SerializeObject(bet);
            var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(serializedMessage));

            await sender.ScheduleMessageAsync(serviceBusMessage,dateTime);
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

        public static string GetDebitURL(TableServiceClient serviceClient)
        {
            var tableClient = serviceClient.GetTableClient("BACKOFFICE");
            var queryResult = tableClient.Query<SettingEntity>("PartitionKey eq 'SETTINGS' and RowKey eq 'DebitURL'");
            var model = queryResult.FirstOrDefault();
            return model.Value;
        }
    }
}

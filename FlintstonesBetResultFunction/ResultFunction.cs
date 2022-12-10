using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Azure.Messaging.ServiceBus;
using FlintstonesEntities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;

namespace FlintstonesBetResultFunction
{
    public class ResultFunction
    {
        [FunctionName("ResultFunction")]
        public async Task Run([ServiceBusTrigger("bets", Connection = "ServiceBus")]string myQueueItem, ILogger log)
        {
            var serviceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=nivs;AccountKey=mHilsEON7rSB84YCK6noL0sWbs8nxX+UjihWeeSawKPPyu0H1yKh40JtMQ/iHDJXS+RE414LM2Th+AStT1MWJg==;EndpointSuffix=core.windows.net");
            var betTableClient = serviceClient.GetTableClient("BETS");
            var resultTableClient = serviceClient.GetTableClient("RESULTS");

            var creditURL = GetCreditURL(serviceClient);
            //var creditURL = "https://localhost:7184/credit";

            var bet = JsonConvert.DeserializeObject<BetEntity>(myQueueItem);
            var latestPrice = GetPrice(serviceClient, bet.Market);

            var resultEntity = new ResultEntity();
            resultEntity.PartitionKey = bet.PartitionKey;
            resultEntity.RowKey = bet.RowKey;
            resultEntity.ResultMarketPrice = latestPrice;
            resultEntity.Timestamp = DateTime.UtcNow;
            resultEntity.CreatedDate = DateTime.UtcNow.AddHours(2);
            resultEntity.Cashout = false;
            resultEntity.WinAmount = 0;
            resultEntity.ClientID = bet.PartitionKey.ToString();

            // client predicted price will go up
            if (bet.Selection == 1)
            {
                if (latestPrice > bet.CurrentMarketPrice)
                {
                    // pay the client
                    resultEntity.WinAmount = bet.TotalPayout;
                    bet.StatusID = 2;
                }
                else
                {
                    bet.StatusID = 3;
                }

                if (latestPrice == bet.CurrentMarketPrice)
                {
                    // give the client back the stake
                    resultEntity.WinAmount = bet.StakeAmount;
                    bet.StatusID = 4;
                }
            }
            else
            {
                if (bet.CurrentMarketPrice > latestPrice)
                {
                    // pay the client
                    resultEntity.WinAmount = bet.TotalPayout;
                    bet.StatusID = 2;
                }
                else
                {
                    bet.StatusID = 3;
                }

                if (latestPrice == bet.CurrentMarketPrice)
                {
                    // give the client back the stake
                    resultEntity.WinAmount = bet.StakeAmount;
                    bet.StatusID = 4;
                }
            }

            await betTableClient.UpsertEntityAsync(bet);
            await resultTableClient.UpsertEntityAsync(resultEntity);

            await SendResultToClient(resultEntity, bet, creditURL);

            //log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            log.LogInformation("bet resulted");
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

        public static async Task<ClientResponse> SendResultToClient(ResultEntity resultEntity, BetEntity betEntity, string creditURL)
        {
            var result = new ClientResponse();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", betEntity.Token);

            var objToSend = new CreditRequest() 
            { 
                Cashout = false,
                CashoutAmount = 0,
                CashoutCreatedDate = null,
                ClientID = resultEntity.ClientID, 
                CreatedDate = DateTime.Now,
                ResultMarketPrice = resultEntity.ResultMarketPrice,
                WinAmount = resultEntity.WinAmount
            };

            var content = new StringContent(JsonConvert.SerializeObject(objToSend), Encoding.UTF8, "application/json");

            var maxRetryAttempts = 3;
            var pauseBetweenFailures = TimeSpan.FromSeconds(5);

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures);

            await retryPolicy.ExecuteAsync(async () =>
            {
                var response = await httpClient.PostAsync(creditURL, content);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<ClientResponse>(data);
                }

                response.EnsureSuccessStatusCode();
            });

            return result;
        }

        public static string GetCreditURL(TableServiceClient serviceClient)
        {
            var tableClient = serviceClient.GetTableClient("BACKOFFICE");
            var queryResult = tableClient.Query<SettingEntity>("PartitionKey eq 'SETTINGS' and RowKey eq 'CreditURL'");
            var model = queryResult.FirstOrDefault();
            return model.Value;
        }
    }
}

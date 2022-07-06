using System;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using FlintstonesEntities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FlintstonesBetResultFunction
{
    public class Function1
    {
        [FunctionName("ResultFunction")]
        public async Task Run([ServiceBusTrigger("rm-bets", Connection = "ServiceBus")]string myQueueItem, ILogger log)
        {
            var serviceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=rmzasa;AccountKey=plxf+fIqm/TYanA0vvgPDUBZYS5j3HGoZtcXPP5RByI7t+wfzbtd5v6rNvOgCHfswBt0wGJAtOSP+AStDTNuOw==;EndpointSuffix=core.windows.net");
            var betTableClient = serviceClient.GetTableClient("BETS");
            var resultTableClient = serviceClient.GetTableClient("RESULTS");

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
    }
}

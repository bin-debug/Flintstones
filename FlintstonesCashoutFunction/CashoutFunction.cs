using System;
using System.Linq;
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

namespace FlintstonesCashoutFunction
{
    public class CashoutFunction
    {
        [FunctionName("CashoutFunction")]
        public void Run([ServiceBusTrigger("cashout", Connection = "ServiceBus")]string myQueueItem, ILogger log)
        {
            var bet = JsonConvert.DeserializeObject<BetEntity>(myQueueItem);

            var dateExpiry = bet.CreatedDate.AddSeconds(bet.Duration);
            if (DateTime.Now >= dateExpiry)
                return;

            var serviceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=nivs;AccountKey=mHilsEON7rSB84YCK6noL0sWbs8nxX+UjihWeeSawKPPyu0H1yKh40JtMQ/iHDJXS+RE414LM2Th+AStT1MWJg==;EndpointSuffix=core.windows.net");
            var cashoutTableClient = serviceClient.GetTableClient("CASHOUT");

            var latestPrice = GetPrice(serviceClient, bet.Market);
            var cashoutEntity = GetCashoutEntity(serviceClient, bet);
            //CalculateCashoutValue(serviceClient, bet, cashoutEntity, latestPrice);

            log.LogInformation($"C# ServiceBus queue trigger function processed cashout message: {myQueueItem}");
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

        public static CashoutEntity GetCashoutEntity(TableServiceClient serviceClient, BetEntity bet)
        {
            var tableClient = serviceClient.GetTableClient("CASHOUT");
            var queryResult = tableClient.Query<CashoutEntity>($"PartitionKey eq '{bet.PartitionKey}' and RowKey eq '{bet.RowKey}'");
            var model = queryResult.FirstOrDefault();
            return model;
        }

        public async static Task PublishCashoutBet(BetEntity bet)
        {
            if (bet.Duration < 60)
                return;

            string connectionString = "Endpoint=sb://nivash.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=JBWiJC6pKtyMrBr0kdgB9K6NrxDMtoLdJ1PPVIODJsE=";
            string queueName = "cashout";
            var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(queueName);

            var dateTime = DateTime.UtcNow.AddSeconds(15);

            var serializedMessage = JsonConvert.SerializeObject(bet);
            var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(serializedMessage));

            await sender.ScheduleMessageAsync(serviceBusMessage, dateTime);
        }

        //public static async Task UpdateCashout(TableServiceClient serviceClient)
        //{
        //    var tableClient = serviceClient.GetTableClient("CASHOUT");
        //    var result = await tableClient.UpsertEntityAsync(bet);
        //}

        //private static void CalculateCashoutValue(TableServiceClient serviceClient, BetEntity bet, CashoutEntity cashoutEntity, double price)
        //{
        //    var newPrice = price;
        //    // check to if the price is gone in the opposite direction to the selection
        //    // if bet is gone in the opposite direction then take away half of the money left
        //    // if the bet is gone in the direction of the selection then add 5% to the cashout

        //    if (bet.Selection == 1)
        //    {
        //        decimal percentage;
        //        if (newPrice > bet.CurrentMarketPrice) // this bet is winning
        //        {
        //            if (_cashoutMethodCalled == 2)
        //            {
        //                decimal percentageNumber = (decimal)40 / 100;
        //                percentage = percentageNumber * bet.Payout;
        //            }
        //            else
        //            {
        //                decimal percentageNumber = (decimal)75 / 100;
        //                percentage = percentageNumber * bet.Payout;
        //            }

        //            var newPayout = bet.Payout - percentage;
        //            await _bettingRepo.UpsertCashoutValues(bet, newPayout);
        //        }

        //        if (bet.CurrentMarketPrice > newPrice) // this bet is losing
        //        {
        //            if (_cashoutMethodCalled == 2)
        //            {
        //                decimal percentageNumber = (decimal)35 / 100;
        //                percentage = percentageNumber * bet.Payout;
        //            }
        //            else
        //            {
        //                decimal percentageNumber = (decimal)75 / 100;
        //                percentage = percentageNumber * bet.Payout;
        //            }

        //            var newPayout = bet.Payout - percentage;
        //            await _bettingRepo.UpsertCashoutValues(bet, newPayout);
        //        }
        //    }
        //    else
        //    {
        //        decimal percentage;
        //        if (newPrice < bet.CurrentMarketPrice) // this bet is winning
        //        {
        //            if (_cashoutMethodCalled == 2)
        //            {
        //                decimal percentageNumber = (decimal)40 / 100;
        //                percentage = percentageNumber * bet.Payout;
        //            }
        //            else
        //            {
        //                decimal percentageNumber = (decimal)75 / 100;
        //                percentage = percentageNumber * bet.Payout;
        //            }

        //            var newPayout = bet.Payout - percentage;
        //            await _bettingRepo.UpsertCashoutValues(bet, newPayout);
        //        }

        //        if (newPrice > bet.CurrentMarketPrice) // this bet is losing
        //        {
        //            if (_cashoutMethodCalled == 2)
        //            {
        //                decimal percentageNumber = (decimal)35 / 100;
        //                percentage = percentageNumber * bet.Payout;
        //            }
        //            else
        //            {
        //                decimal percentageNumber = (decimal)75 / 100;
        //                percentage = percentageNumber * bet.Payout;
        //            }

        //            var newPayout = bet.Payout - percentage;
        //            await _bettingRepo.UpsertCashoutValues(bet, newPayout);
        //        }
        //    }
        //}
    }
}

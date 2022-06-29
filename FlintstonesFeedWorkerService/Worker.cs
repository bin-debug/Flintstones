using Binance.Net.Clients;
using StackExchange.Redis;
using System.Data;
using System.Text;
using FlintstonesModels;

namespace FlintstonesFeedWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        //string symbol = Environment.GetEnvironmentVariable("SYMBOL");
        string symbol = "BTCUSDT";
        CosmosDbService _cosmos;

        public Worker(ILogger<Worker> logger, CosmosDbService cosmos)
        {
            _logger = logger;
            _cosmos = cosmos;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var socketClient = new BinanceSocketClient();

            await socketClient.SpotStreams.SubscribeToMiniTickerUpdatesAsync(symbol, async data =>
            {
                try
                {
                    string date = DateTime.Today.Date.ToString("ddMMyyyy");//29062022
                    var model = new FeedModel();
                    model.Date = date;
                    model.ID = Guid.NewGuid();
                    model.LastPrice = data.Data.LastPrice;
                    model.TimeStamp = data.Timestamp.AddHours(2).ToString("yyyy-MM-ddTHH:mm:ss");

                    await _cosmos.AddAsync(model);
                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });
        }
    }
}

//--SELECT * FROM c
//-- where c.TimeStamp = '2022-06-29T21:30:37'
//-- and c.partitionKey = '29062022'

//-- SELECT TOP 1 * FROM c
//-- where c.partitionKey = '29062022'
//-- order by c.TimeStamp desc

//SELECT c.LastPrice FROM c
//where c.TimeStamp = '2022-06-29T19:17:01'
//and c.clientid = '29062022'
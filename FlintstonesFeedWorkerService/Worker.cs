using Binance.Net.Clients;
using System.Data;
using System.Text;

namespace FlintstonesFeedWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly TableStorageService _tableStorageService;

        //string symbol = Environment.GetEnvironmentVariable("SYMBOL");
        string symbol = "BTCUSDT";

        public Worker(ILogger<Worker> logger, TableStorageService tableStorageService)
        {
            _logger = logger;
            _tableStorageService = tableStorageService;
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
                    model.PartitionKey = date;
                    model.RowKey = data.Timestamp.AddHours(2).ToString("yyyy-MM-ddTHH:mm:ss");
                    model.Timestamp = data.Timestamp;
                    model.LastPrice =  data.Data.LastPrice;

                    await _tableStorageService.UpsertEntityAsync(model);
                    _logger.LogInformation($"Price added {model.LastPrice}");
                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _logger.LogError(ex.ToString());
                }
            });
        }
    }
}
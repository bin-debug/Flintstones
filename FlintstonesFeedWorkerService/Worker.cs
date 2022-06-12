using Binance.Net.Clients;
using Dapper;
using Npgsql;
using StackExchange.Redis;
using System.Data;
using System.Text;

namespace FlintstonesFeedWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        string symbol = Environment.GetEnvironmentVariable("SYMBOL");
        //string symbol = "BTCUSDT";

        ConnectionMultiplexer redis; 
        IDatabase db;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            redis = ConnectionMultiplexer.Connect("redis:6379");
            db = redis.GetDatabase();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var socketClient = new BinanceSocketClient();

            await socketClient.SpotStreams.SubscribeToMiniTickerUpdatesAsync(symbol, async data =>
            {
                try
                {
                    var val = new RedisValue($"{data.Data.LastPrice}-{data.Timestamp}");
                    await db.ListLeftPushAsync(symbol, val);
                    Console.WriteLine(val);
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
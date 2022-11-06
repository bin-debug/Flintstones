using FlintstonesEntities;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Timers;

namespace FlintstonesWeb.Service
{
    public class MarketService : IDisposable
    {

        private readonly System.Timers.Timer timer;
        private int INTERVAL_MS = 1000;
        private IHttpClientFactory _clientFactory;
        private readonly IMemoryCache _memoryCache;
        public string MarketName { get; set; } = "BTCUSDT";

        public MarketService(IHttpClientFactory clientFactory, IMemoryCache memoryCache)
        {
            _clientFactory = clientFactory;
            _memoryCache = memoryCache;

            Populate();

            timer = new System.Timers.Timer(INTERVAL_MS);
            timer.Elapsed += TimerTick;
            timer.Enabled = true;
        }

        public void Populate()
        {
            var client = _clientFactory.CreateClient();

            //var response = client.GetStringAsync($"https://rm-mk-api.azurewebsites.net/api/GetMarkets?code=XDPcsRnZV_mBDYkTAV-5TyGdMi24o25V_Ci1Sjd0jJhKAzFuxt7GrQ==").Result;
            var response =  client.GetStringAsync($"https://rm-mk-api.azurewebsites.net/api/GetMarkets?code=XDPcsRnZV_mBDYkTAV-5TyGdMi24o25V_Ci1Sjd0jJhKAzFuxt7GrQ==&market={MarketName}").Result;

            if (response != null)
            {
                _memoryCache.Set<string>("markets", response);
            }
        }

        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            var client = _clientFactory.CreateClient();

            //var response = client.GetStringAsync($"https://rm-mk-api.azurewebsites.net/api/GetMarkets?code=XDPcsRnZV_mBDYkTAV-5TyGdMi24o25V_Ci1Sjd0jJhKAzFuxt7GrQ==").Result;
            var response = client.GetStringAsync($"https://rm-mk-api.azurewebsites.net/api/GetMarkets?code=XDPcsRnZV_mBDYkTAV-5TyGdMi24o25V_Ci1Sjd0jJhKAzFuxt7GrQ==&market={MarketName}").Result;

            if (response != null)
            {
                //var rawData = JsonConvert.DeserializeObject<List<MarketEntity>>(response);

                //var rawData = JsonConvert.SerializeObject(response);
                //_memoryCache.Set<string>("markets", rawData);

                _memoryCache.Set<string>("markets", response);
            }

            INTERVAL_MS = 5000;
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}

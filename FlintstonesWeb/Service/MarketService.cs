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
        public event Action OnMarketChanged;
        public string MarketName;

        public MarketService(IHttpClientFactory clientFactory, IMemoryCache memoryCache)
        {
            _clientFactory = clientFactory;
            _memoryCache = memoryCache;

            timer = new System.Timers.Timer(INTERVAL_MS);
            timer.Elapsed += TimerTick;
            timer.Enabled = true;
        }

        public void Populate()
        {
            var client = _clientFactory.CreateClient();

            //var response = client.GetStringAsync($"https://rm-mk-api.azurewebsites.net/api/GetMarkets?code=XDPcsRnZV_mBDYkTAV-5TyGdMi24o25V_Ci1Sjd0jJhKAzFuxt7GrQ==").Result;
            var response =  client.GetStringAsync($"https://markets-api.azurewebsites.net/api/GetMarkets?code=H80qnTQEd3eKraOWUM1gEPyTUqbSJgzvnn89E-YmfX44AzFucYca5A==&market={MarketName}").Result;

            if (!string.IsNullOrEmpty(response))
            {
                _memoryCache.Set<string>("markets", response);
                //timer.Enabled = true;
                OnMarketChanged?.Invoke();
            }
        }

        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            var client = _clientFactory.CreateClient();

            //var response = client.GetStringAsync($"https://rm-mk-api.azurewebsites.net/api/GetMarkets?code=XDPcsRnZV_mBDYkTAV-5TyGdMi24o25V_Ci1Sjd0jJhKAzFuxt7GrQ==").Result;
            var response = client.GetStringAsync($"https://markets-api.azurewebsites.net/api/GetMarkets?code=H80qnTQEd3eKraOWUM1gEPyTUqbSJgzvnn89E-YmfX44AzFucYca5A==&market={MarketName}").Result;

            if (response != null)
            {
                _memoryCache.Set<string>("markets", response);
                OnMarketChanged?.Invoke();
            }

            INTERVAL_MS = 2000;
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}

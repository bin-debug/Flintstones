using FlintstonesEntities;
using Microsoft.Extensions.Caching.Memory;

namespace FlintstonesWeb.Service
{
    public class SettingsService
    {
        private IHttpClientFactory _clientFactory;
        private readonly IMemoryCache _memoryCache;

        public SettingsService(IHttpClientFactory clientFactory, IMemoryCache memoryCache)
        {
            _clientFactory = clientFactory;
            _memoryCache = memoryCache;
        }

        public async Task Populate()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync("https://markets-api.azurewebsites.net/api/SettingsFunction?code=C3pEHDVDa8DGEYHfqyrNRjSGV7QuFG0Nktac5_yhcM83AzFuoijVBA==");
            if (!string.IsNullOrEmpty(response))
            {
                _memoryCache.Set<string>("settings", response);
            }
        }
    }
}

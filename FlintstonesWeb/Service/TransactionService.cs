using FlintstonesEntities;
using Newtonsoft.Json;
using System;
using System.Timers;

namespace FlintstonesWeb.Service
{
    public class TransactionService : IDisposable
    {
        private readonly System.Timers.Timer timer;
        const int INTERVAL_MS = 3000;
        private IHttpClientFactory _clientFactory;

        public event Action<List<BetEntity>> OnTransactionsChanged;
        public int clientid;
        public int pagesize;

        public TransactionService(IHttpClientFactory clientFactory)
        {
            timer = new System.Timers.Timer(INTERVAL_MS);
            timer.Elapsed += TimerTick;
            timer.Enabled = true;
            _clientFactory = clientFactory;
        }

        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            //var clientid = 123;
            //var pagesize = 8;

            var client = _clientFactory.CreateClient();
            var response = client.GetStringAsync($"https://rm-ct.azurewebsites.net/api/GetClientTransactions?clientid={clientid}&pagesize={pagesize}").Result;
            if (response != null)
            {
                var rawData = JsonConvert.DeserializeObject<List<BetEntity>>(response);
                OnTransactionsChanged?.Invoke(rawData);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

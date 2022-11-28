using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using FlintstonesWeb;
using FlintstonesWeb.Shared;
using MudBlazor;
using System.Text.Json;
using System.Net.Http.Headers;
using FlintstonesEntities;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using FlintstonesWeb.Service;
using System.Timers;

namespace FlintstonesWeb.Pages
{
    public partial class Index
    {
        public bool price_direction { get; set; } = true;
        public int stake = 1;
        public int duration = 30;
        public double payout = 0;
        public double odds;
        private bool _processing = false;
        public bool showOddsChangedMessage = false;
        public string OddsChangedMessage = "Odds have changed, recalculating now...";
        public MudBlazor.Severity OddsChangedMessageSeverity = Severity.Error;
        

        public List<MarketEntity> MarketOdds;
        public int activeTransactions = 0;
        public List<Transactions> transactions;

        private System.Timers.Timer timer;
        private int INTERVAL_MS = 1000;

        [Parameter]
        public string symbol { get; set; }

        [Parameter]
        public string client { get; set; }

        [Parameter]
        public string key { get; set; }

        [Parameter]
        public string token { get; set; }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender == true)
            {
                MarketService.MarketName = symbol;
                //Js.InvokeAsync<object>("buildChart", "BINANCE:BTCUSDT");
                Js.InvokeAsync<object>("buildChart", $"BINANCE:{symbol}");
            }

            return base.OnAfterRenderAsync(firstRender);
        }

        protected override Task OnInitializedAsync()
        {
            MarketService.MarketName = symbol;
            MarketService.Populate();
            MarketService.OnMarketChanged += MarketService_OnMarketChanged;

            MarketOdds = new List<MarketEntity>();
            transactions = new List<Transactions>();

            RefreshOdds();

            timer = new System.Timers.Timer(INTERVAL_MS);
            timer.Elapsed += RefreshOddsEvent;
            timer.Enabled = true;

            //PopulateTransactions();
            TransactionService.clientid = client;
            TransactionService.pagesize = 8;
            TransactionService.OnTransactionsChanged += HandleTransactionsChange;
            CalculatePayout();
            return base.OnInitializedAsync();
        }

        private void RefreshOddsEvent(object? sender, ElapsedEventArgs e)
        {
            RefreshOdds();
        }

        public void CalculatePayout()
        {
            if (MarketOdds.Count > 0)
            {
                var direction = price_direction == true ? 1 : 2;
                var selectedDuration = MarketOdds.FirstOrDefault(r => r.Duration == duration && r.Direction == direction);
                odds = selectedDuration.BaseOdds;
                payout = stake * odds;
                InvokeAsync(StateHasChanged);
            }    
        }

        private void MarketService_OnMarketChanged()
        {
            RefreshOdds();
        }

        public void RefreshOdds()
        {
            var response = MemoryCache.Get<string>("markets");

            if (!string.IsNullOrEmpty(response))
            {
                MarketOdds = JsonConvert.DeserializeObject<List<MarketEntity>>(response);
            }

            InvokeAsync(StateHasChanged);
        }

        async Task ProcessSomething()
        {
            // do some sort of validation
            var direction = price_direction == true ? 1 : 2;
            var selectedDuration = MarketOdds.FirstOrDefault(r => r.Duration == duration && r.Direction == direction);
            if (odds != selectedDuration.BaseOdds)
            {
                showOddsChangedMessage = true;
                RefreshOdds();
                CalculatePayout();
                OddsChangedMessage = "New Payout calculated, click on submit bet to confirm.";
                OddsChangedMessageSeverity = Severity.Warning;
                return;
            }

            _processing = true;
            showOddsChangedMessage = false;
            OddsChangedMessageSeverity = Severity.Error;
            OddsChangedMessage = "Odds have changed, recalculating now...";

            var model = new
            {
                clientID = client, // go
                token = token, // go
                stakeAmount = stake, // go
                market = symbol, // go
                //send marketid and not the below
                selection = price_direction == true ? 1 : 0, 
                selectionOdd = odds,
                duration = duration
            };

            var json = JsonConvert.SerializeObject(model);
            var httpClient = ClientFactory.CreateClient();
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //var response = await httpClient.PostAsync("http://localhost:7085/api/BetStrike", content);
            var response = await httpClient.PostAsync("https://bet-strike.azurewebsites.net/api/BetStrike?code=snNnLqHmDdrq8mpWRbKohzla9aBusZ8g7x1OB4fq5s-1AzFukj7L9Q==", content);

            var data = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<ClientResponse>(data);
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add(obj.Message, Severity.Success);

            activeTransactions++;
            _processing = false;
            TransactionService.OnTransactionsChanged += HandleTransactionsChange;
            await InvokeAsync(StateHasChanged);
        }

        private void HandleTransactionsChange(List<BetEntity> bets)
        {
            transactions = new List<Transactions>();
            foreach (var item in bets)
            {
                string direction = item.Selection == 1 ? "UP" : "DOWN";
                transactions.Add(new Transactions { ID = item.RowKey, Payout = item.TotalPayout, MarketName = item.Market.ToUpper(), StatusID = item.StatusID, DateTime = item.CreatedDate, Duration = item.Duration, Description = $"R{item.StakeAmount} for Price ({item.CurrentMarketPrice}) to go {direction} in {item.Duration} secs | odds {item.SelectionOdd.ToString("N2")}" });
            }

            activeTransactions = bets.Count(r => r.StatusID == 1);
            InvokeAsync(StateHasChanged);
            if (activeTransactions == 0)
                TransactionService.OnTransactionsChanged -= HandleTransactionsChange;
        }

        public void BackToLobby()
        {
            NavigationManager.NavigateTo($"/lobby/{client}/{key}/{token}",true);
        }

        public class Transactions
        {
            public string ID { get; set; }

            public string Description { get; set; }

            public double Payout { get; set; }

            public int StatusID { get; set; }

            public int Duration { get; set; }

            public string MarketName { get; set; }

            public DateTime DateTime { get; set; }
        }
    }
}
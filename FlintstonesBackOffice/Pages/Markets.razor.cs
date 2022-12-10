using FlintstonesBackOffice.Services;
using FlintstonesEntities;
using MudBlazor;

namespace FlintstonesBackOffice.Pages
{
    public partial class Markets
    {
        private bool _processing = false;
        public string SelectedMarket { get; set; }
        public string SelectedDirection { get; set; }
        public string StrippedSelectedMarket { get; set; }
        public double UpdatedOdd { get; set; }
        public bool UpdateActive { get; set; }
        public int duration = 30;

        public MarketEntity SelectedMarketToUpdate { get; set; }

        public List<MarketEntity> _marketOdds = new List<MarketEntity>();
        public List<LobbyEntity> _lobby = new List<LobbyEntity>();

        string[] headings = {"", "Duration (secs)", "Odds", "Type", ""};

        protected override async Task OnInitializedAsync()
        {
            TableStorageService.TableName = "BACKOFFICE";
            SelectedMarket = "BTCUSDT - (Bitcoin/Tether)";
            SelectedDirection = "UP";
            await PopulateLobby();
            await PopulateMarketOdds("BTCUSDT", 1);
        }

        async Task PopulateLobby()
        {
            _processing = true;
            var tableClient = await TableStorageService.GetTableClient("BACKOFFICE");
            string query = $"PartitionKey eq 'LOBBY' and IsMarketActive eq true";
            var results = tableClient.Query<LobbyEntity>(query).ToList();
            _lobby = results;
            _processing = false;
        }

        async Task PopulateMarketOdds(string symbol, int direction)
        {
            _processing = true;
            TableStorageService.TableName = "BACKOFFICE";
            var tableClient = await TableStorageService.GetTableClient();
            string query = $"PartitionKey eq 'MARKETS' and MarketName eq '{symbol}' and Direction eq {direction}";
            var results = tableClient.Query<MarketEntity>(query).OrderBy(r => r.Duration).ToList();
            _marketOdds = results;
            _processing = false;
        }

        Task OpenOddsDialog(MarketEntity marketEntity)
        {
            SelectedMarketToUpdate = new MarketEntity();
            SelectedMarketToUpdate = marketEntity;
            visible = true;
            UpdatedOdd = marketEntity.BaseOdds;
            UpdateActive = marketEntity.Active;
            return Task.CompletedTask;
        }

        async Task SaveOdds()
        {
            _processing = true;
            var tableClient = await TableStorageService.GetTableClient("BACKOFFICE");
            SelectedMarketToUpdate.BaseOdds = UpdatedOdd;
            SelectedMarketToUpdate.Active = UpdateActive;
            var response = await tableClient.UpsertEntityAsync(SelectedMarketToUpdate);
            if (response.Status == 204)
            {
                _processing = false;
                visible = false;
            }
        }

        async Task OnSelectedValuesChanged(string val)
        {
            SelectedMarket = val;
            await PopulateMarketOdds(GetStrippedSelectedMarketName(), GetSelectedDirectio());
        }

        async Task OnSelectedDirectionValuesChanged(string val)
        {
            SelectedDirection = val;
            await  PopulateMarketOdds(GetStrippedSelectedMarketName(), GetSelectedDirectio());  
        }

        private string GetStrippedSelectedMarketName()
        {
            var fullMarketName = SelectedMarket.Trim().Split("-");
            return fullMarketName[0].TrimEnd();
        }

        public int GetSelectedDirectio()
        {
            int direction = 1;

            if (SelectedDirection.ToLower() == "down")
                direction = 2;
            else if (SelectedDirection.ToLower() == "odd")
                direction = 3;
            else if (SelectedDirection.ToLower() == "even")
                direction = 4;
            else
                direction = 1;

            return direction;
        }

        private bool visible;
        private void OpenDialog() => visible = true;
        void Submit() => visible = false;
        bool open;
        private DialogOptions dialogOptions = new()
        {};
    }
}
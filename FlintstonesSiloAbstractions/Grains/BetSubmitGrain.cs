namespace FlintstonesSiloAbstractions.Grains
{
    public class BetSubmitGrain : Grain, IBetSubmit
    {
        private readonly ICosmosDbService<BetDTO> _cosmosDbService;
        private readonly ServiceBusSender _serviceBusSender;
        private readonly IHttpClientFactory _httpClientFactory = null!;

        private string _feedURL = "http://localhost:31670/feed";

        public BetSubmitGrain(ICosmosDbService<BetDTO> cosmosDbService, ServiceBusSender serviceBusSender,
            IHttpClientFactory httpClientFactory)
        {
            _cosmosDbService = cosmosDbService;
            _serviceBusSender = serviceBusSender;
            _httpClientFactory = httpClientFactory;

            //_feedURL = configurationSection.GetValue<string>("FeedURL");
        }

        public async Task<BetDTO> SubmitBet(BetDTO bet)
        {
            try
            {
                bet.BetID = Guid.NewGuid();
                var price = await GetPrice(bet.Market);
                if (price == 0)
                    return bet;

                bet.CurrentMarketPrice = price; // get the latest price
                bet.CreatedDate = DateTime.Now;
                bet.Tag = String.Empty;
                var result = await _cosmosDbService.AddAsync(bet);

                //// SEND TO OPERATOR (need to add polly also)

                if (result != null)
                {
                    this.RegisterTimer(PublishMessage, bet, TimeSpan.FromMinutes(bet.Duration), TimeSpan.FromMilliseconds(-1));
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private async Task PublishMessage(object arg)
        {
            var bet = arg as BetDTO;
            var body = JsonSerializer.Serialize(bet);
            await _serviceBusSender.SendMessageAsync(new ServiceBusMessage(body));
            Console.WriteLine($"bet sent to queue");
        }

        private async Task<decimal> GetPrice(string symbol)
        {
            try
            {
                decimal price = 0;
                string url = $"{_feedURL}/{symbol}";
                HttpClient client = _httpClientFactory.CreateClient();
                var response = await client.GetStringAsync(url);
                if (!string.IsNullOrEmpty(response))
                {
                    response = response.Replace("\"", "");
                    var data = response.Split("-");
                    price = Convert.ToDecimal(data[0]);
                }
                return price;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }
    }
}

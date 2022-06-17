namespace FlintstonesSiloAbstractions.Grains
{
    public class BetSubmitGrain : Grain, IBetSubmit
    {
        private readonly ICosmosDbService<BetDTO> _cosmosDbService;
        private readonly ServiceBusSender _serviceBusSender;

        public BetSubmitGrain(ICosmosDbService<BetDTO> cosmosDbService, ServiceBusSender serviceBusSender)
        {
            _cosmosDbService = cosmosDbService;
            _serviceBusSender = serviceBusSender;
        }

        public async Task<BetDTO> SubmitBet(BetDTO bet)
        {
            bet.BetID = Guid.NewGuid();
            bet.CurrentMarketPrice = 0;  // get the latest price
            bet.CreatedDate = DateTime.Now;

            // SEND TO OPERATOR FIRST



            var result = await _cosmosDbService.AddAsync(bet);

            if (result != null)
            {
                this.RegisterTimer(PublishMessage, bet, TimeSpan.FromMinutes(bet.Duration), TimeSpan.FromMilliseconds(-1));
            }

            return result;
        }

        private async Task PublishMessage(object arg)
        {
            var bet = arg as BetDTO;
            var body = JsonSerializer.Serialize(bet);
            await _serviceBusSender.SendMessageAsync(new ServiceBusMessage(body));
            Console.WriteLine($"bet sent to queue");
        }
    }
}

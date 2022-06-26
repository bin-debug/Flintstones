using FlintstonesResultsConsumer;

Console.WriteLine("Hello, Servicebus!");

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:30580");
IDatabase db = redis.GetDatabase();

string symbol = "BTCUSDT";
string date = DateTime.Today.Date.ToString("ddMMyyyy");
string key = $"{symbol}";

string connectionString = "Endpoint=sb://dev-test-rm.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=HO3U4nLJoeLfX8+kwHahMKVG38qRosW7auRoFJpA/a8=";
string queueName = "bets";
ServiceBusClient client;
ServiceBusProcessor processor;

string databaseName = "flintstones";
string containerName = "barney";
string account = "https://dev-test-rm.documents.azure.com:443/";
string cosmokey = "HE9dQvkko2ci8eLMg5W4MxX7g1OcPMaf0SJZfrxtwihqZGHz9WL7uDVpRefdl2IPrRNg1Jd3aU38dlGiqzP8lQ==";
CosmosClient cosmosClient = new Microsoft.Azure.Cosmos.CosmosClient(account, cosmokey);
CosmosDbService cosmosDbService = new CosmosDbService(cosmosClient, databaseName, containerName);

client = new ServiceBusClient(connectionString);

// create a processor that we can use to process the messages
processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

try
{
    // add handler to process messages
    processor.ProcessMessageAsync += MessageHandler;
    // add handler to process any errors
    processor.ProcessErrorAsync += ErrorHandler;
    // start processing 
    await processor.StartProcessingAsync();

    Console.WriteLine("Wait for a minute and then press any key to end the processing");
    Console.ReadKey();

    // stop processing 
    Console.WriteLine("\nStopping the receiver...");
    await processor.StopProcessingAsync();
    Console.WriteLine("Stopped receiving messages");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await processor.DisposeAsync();
    await client.DisposeAsync();
}

// handle received messages
async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Received: {body}");

    var bet = JsonSerializer.Deserialize<BetDTO>(body);
    var resultedBet = await LetsResult(bet);

    //upsert resuledBet to cosmosdb
    await cosmosDbService.UpdateAsync(bet.BetID.ToString(), bet);

    // complete the message. messages is deleted from the queue. 
    await args.CompleteMessageAsync(args.Message);
}

// handle any errors when receiving messages
static Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}

Console.ReadKey();

async Task<BetDTO> LetsResult(BetDTO bet)
{
    var resultDate = bet.CreatedDate.AddHours(-2).AddMinutes(bet.Duration).ToString("MM/dd/yyyy HH:mm:ss");
    var price = await GetResultPrice(resultDate);

    if (bet.Selection == 1) // this means the punter wanted the price to go up
    {
        if (price >= bet.CurrentMarketPrice)
            return MarkBet(bet, price, true);
        else
            return MarkBet(bet, price, false);
    }
    else
    {
        if (price <= bet.CurrentMarketPrice)
            return MarkBet(bet, price, true);
        else
            return MarkBet(bet, price, false);
    }
}

static BetDTO MarkBet(BetDTO bet, decimal price, bool win)
{
    bet.Result.Cashout = false;
    bet.Result.CashoutAmount = 0;
    bet.Result.CashoutCreatedDate = null;
    bet.Result.ResultMarketPrice = price;
    bet.Result.CreatedDate = DateTime.Now;

    if(win == true)
        bet.Result.WinAmount = bet.TotalPayout;
    else
        bet.Result.WinAmount = 0;

    return bet;
}

async Task<decimal> GetResultPrice(string resultTime)
{
    resultTime = DateTime.Now.AddHours(-2).AddMinutes(-1).ToString("MM/dd/yyyy HH:mm:ss");

    var values = await db.ListRangeAsync(key, stop: 1200);

    var Prices = new List<PriceModel>();

    foreach (var item in values)
    {
        var data = item.ToString().Split("-");
        var obj = new PriceModel();
        obj.Price = Convert.ToDecimal(data[0]);
        obj.DateTime = Convert.ToDateTime(data[1]);
        Prices.Add(obj);
    }

    var currentPrice = Prices.FirstOrDefault(r => r.DateTime == Convert.ToDateTime(resultTime));
    return currentPrice.Price;
}

class PriceModel
{
    public decimal Price { get; set; }
    public DateTime DateTime { get; set; }
}
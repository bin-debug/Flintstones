using StackExchange.Redis;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    EndPoints = { "redis:6379" },
                    //EndPoints = { "localhost:30580" },
                    AbortOnConnectFail = false,
                    DefaultDatabase = 0
                }));


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/feed/{symbol}", async (IConnectionMultiplexer redis, string symbol) =>
{
    var db = redis.GetDatabase();
    symbol = symbol.ToUpper();
    var price = await db.ListGetByIndexAsync(symbol, 0);
    return Results.Ok(price.ToString());
});

app.Run();

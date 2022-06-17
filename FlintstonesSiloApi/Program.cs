var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
builder.Services.AddSingleton<ICosmosDbService<BetDTO>>(ExtensionHelper.InitializeCosmosClientInstanceAsync(configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
builder.Services.AddSingleton<ServiceBusSender>(ExtensionHelper.InitializeServiceBusInstanceAsync(configuration.GetSection("AzureServiceBus")).GetAwaiter().GetResult());

var redisConnectionString = $"redis:6379";

builder.Host.UseOrleans((ctx,siloBuilder) =>
{
    if (ctx.HostingEnvironment.IsDevelopment())
    {
        siloBuilder.UseLocalhostClustering().AddMemoryGrainStorage("flintstones-test")
                   .ConfigureLogging(options => { options.AddConsole(); });
    }
    else
    {
        siloBuilder.UseKubernetesHosting()
                   .UseRedisClustering(options => options.ConnectionString = redisConnectionString)
                   .AddRedisGrainStorage("flintstones-test", options => options.ConnectionString = redisConnectionString)
                   .ConfigureLogging(options => { options.AddConsole(); });
    }
    siloBuilder.UseDashboard(options => { });
});

var app = builder.Build();

app.MapGet("/", async (IClusterClient orleansClient) =>
{
    var grain = orleansClient.GetGrain<IHelloWorld>(Guid.Empty);
    return await grain.SayHelloWorld();
});

app.MapPost("/betting", async (IClusterClient orleansClient, [FromBody] BetDTO betDTO) =>
{
    var grain = orleansClient.GetGrain<IBetSubmit>(Guid.NewGuid());
    var result = await grain.SubmitBet(betDTO);
});

app.Run();
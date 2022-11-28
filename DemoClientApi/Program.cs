using DemoClientApi.Models;
using FlintstonesEntities;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", async () => 
{
    return Results.Ok("Atomic-Labs Demo API.");
});

app.MapPost("/debit", async (BetRequest request) =>
{
    var response = new BetResponse();

    response.Balance = new Random().NextDouble();
    response.StatusCode = 200;
    response.Message = "Bet successfully placed.";

    return Results.Ok(response);
});

app.MapPost("/credit", async (CreditRequest creditRequest) =>
{
    var response = new BetResponse();
    response.Balance = new Random().NextDouble();
    response.StatusCode = 200;
    response.Message = "Success.";

    return Results.Ok(response);
});

app.MapPost("/health", async () => { return Results.Ok(); });

app.Run();

using DemoClientApi.DataAccess;
using DemoClientApi.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cnn = new SqliteConnection("Filename=:memory:");
cnn.Open();
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(cnn));

var app = builder.Build();

app.MapGet("/", async (AppDbContext db) => 
{
    await db.Database.EnsureCreatedAsync();

    db.Clients.AddRange(new Client() { Id = 1, Name = "Bruce Wayne", Balance = 500 },
        new Client() { Id = 2, Name = "Alfred Pennyworth", Balance = 400 },
        new Client() { Id = 3, Name = "Clark Kent", Balance = 300 });

    var result = await db.SaveChangesAsync();
    if (result > 0)
        return Results.Ok("Atomic-Labs Demo API, clients have been created.");
    else
        return Results.Conflict();
});

app.MapGet("/delete", async (AppDbContext db) =>
{
    var clients = await db.Clients.ToListAsync();
    foreach (var item in clients)
    {
        db.Clients.Remove(item);
        await db.SaveChangesAsync();
    }
    return Results.Ok("Atomic-Labs Demo API, clients have been deleted.");
});

app.MapGet("/test", async (AppDbContext db) =>
{
    var result = await db.Clients.ToListAsync();
    return Results.Ok(result);
});

app.MapPost("/debit", async (BetRequest request, AppDbContext db) =>
{
    var response = new BetResponse();

    var client = db.Clients.FirstOrDefault(r => r.Id == Convert.ToInt32(request.ClientID));

    if (request.StakeAmount > client.Balance)
    {
        response.Balance = client.Balance;
        response.StatusCode = 300;
        response.Message = "Insufficient funds.";
        return Results.Ok(response);
    }

    client.Balance = client.Balance - request.StakeAmount;
    response.Balance = client.Balance;
    response.StatusCode = 200;
    response.Message = "Successfully placed bet.";
    await db.SaveChangesAsync();

    return Results.Ok(response);
});

app.MapPost("/credit", async (BetRequest request, AppDbContext db) =>
{
    var response = new BetResponse();

    var client = db.Clients.FirstOrDefault(r => r.Id == Convert.ToInt32(request.ClientID));

    client.Balance = client.Balance - request.StakeAmount;
    response.Balance = client.Balance;
    response.StatusCode = 200;
    response.Message = "Successfully placed bet.";
    await db.SaveChangesAsync();

    return Results.Ok(response);
});

app.MapPost("/health", async () => { return Results.Ok(); });

app.Run();

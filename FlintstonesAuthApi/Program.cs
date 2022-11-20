using FlintstonesAuthApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServer().AddDeveloperSigningCredential()
    .AddInMemoryApiScopes(Config.ApiScopes).AddInMemoryClients(Config.Clients);

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.Run();

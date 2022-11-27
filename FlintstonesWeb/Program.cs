using FlintstonesWeb.Data;
using FlintstonesWeb.Service;
using Microsoft.AspNetCore.Rewrite;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<TransactionService>();
builder.Services.AddSingleton<MarketService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

var options = new RewriteOptions();

app.UseRewriter(options);


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/lobby/{Client}/{Balance}/{Key}/{Token}", "/_Host");
app.MapFallbackToPage("/_Host");

app.Run();

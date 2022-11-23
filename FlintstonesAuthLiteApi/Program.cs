using FlintstonesAuthLiteApi.Models;
using FlintstonesAuthLiteApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
//var settings = builder.Configuration.GetSection("Settings").Get<Settings>();
builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/authenticate", (AuthenticateRequest model, IUserService userService) =>
{
    var response = userService.Authenticate(model);

    if (response == null)
        return Results.BadRequest("Username or password is incorrect");

    return Results.Ok(response);
});

app.Run();

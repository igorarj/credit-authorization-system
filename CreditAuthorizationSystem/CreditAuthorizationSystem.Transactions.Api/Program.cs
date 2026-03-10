using CreditAuthorizationSystem.Transactions.Application.Interfaces;
using CreditAuthorizationSystem.Transactions.Application.Messaging;
using CreditAuthorizationSystem.Transactions.Application.Services;
using CreditAuthorizationSystem.Transactions.Domain.Repositories;
using CreditAuthorizationSystem.Transactions.Infrastructure.HttpClients;
using CreditAuthorizationSystem.Transactions.Infrastructure.Messaging;
using CreditAuthorizationSystem.Transactions.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddSingleton<ITransactionRepository, InMemoryTransactionRepository>();

builder.WebHost.UseUrls("http://0.0.0.0:8080");

builder.Services.AddHttpClient<ICustomerApiClient, CustomerApiClient>(client =>
{
    client.BaseAddress = new Uri("http://customer-api:8080");
});

var authConfig = builder.Configuration.GetSection("AuthApi");
var baseUrl = authConfig.GetValue<string>("BaseUrl");
var user = authConfig.GetValue<string>("UserName");
var pass = authConfig.GetValue<string>("Password");

builder.Services.AddSingleton<IAuthApiClient>(sp =>
{
    var http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    return new AuthApiClient(http, user, pass);
});

var secret = builder.Configuration["Jwt:Secret"];
var key = Encoding.ASCII.GetBytes(secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddSingleton<ITransactionEventPublisher>(sp =>
{
    var config = builder.Configuration.GetSection("RabbitMQ");
    var hostName = config.GetValue<string>("HostName");
    var userName = config.GetValue<string>("UserName");
    var password = config.GetValue<string>("Password");

    return new RabbitMqTransactionEventPublisher(hostName, userName, password);
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
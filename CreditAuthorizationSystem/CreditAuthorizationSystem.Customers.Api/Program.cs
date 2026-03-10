using CreditAuthorizationSystem.Customers.Application.Interfaces;
using CreditAuthorizationSystem.Customers.Application.Services;
using CreditAuthorizationSystem.Customers.Domain.Repositories;
using CreditAuthorizationSystem.Customers.Infrastructure.Messaging;
using CreditAuthorizationSystem.Customers.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();

builder.WebHost.UseUrls("http://0.0.0.0:8080");

builder.Services.AddMemoryCache();

builder.Services.AddHostedService(sp =>
{
    var repository = sp.GetRequiredService<ICustomerRepository>();
    var config = builder.Configuration.GetSection("RabbitMQ");
    var hostName = config.GetValue<string>("HostName");
    var userName = config.GetValue<string>("UserName");
    var password = config.GetValue<string>("Password");
    return new RabbitMqTransactionConsumer(repository, hostName, userName, password);
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

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
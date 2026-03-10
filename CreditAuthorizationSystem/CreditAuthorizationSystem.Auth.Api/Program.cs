using CreditAuthorizationSystem.Auth.Application.Interfaces;
using CreditAuthorizationSystem.Auth.Application.Services;
using CreditAuthorizationSystem.Auth.Domain.Repositories;
using CreditAuthorizationSystem.Auth.Infrastructure.Repositories;
using CreditAuthorizationSystem.Auth.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddScoped<IRegisterUserService, RegisterUserService>();
builder.Services.AddScoped<ILoginService, LoginService>();

builder.Services.AddSingleton<ITokenService>(
    new JwtTokenService(builder.Configuration["Jwt:Secret"])
);

builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
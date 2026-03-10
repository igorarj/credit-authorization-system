FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CreditAuthorizationSystem.Auth.Api/*.csproj CreditAuthorizationSystem.Auth.Api/
COPY CreditAuthorizationSystem.Auth.Application/*.csproj CreditAuthorizationSystem.Auth.Application/
COPY CreditAuthorizationSystem.Auth.Domain/*.csproj CreditAuthorizationSystem.Auth.Domain/
COPY CreditAuthorizationSystem.Auth.Infrastructure/*.csproj CreditAuthorizationSystem.Auth.Infrastructure/

RUN dotnet restore CreditAuthorizationSystem.Auth.Api/CreditAuthorizationSystem.Auth.Api.csproj

COPY . .

WORKDIR /src/CreditAuthorizationSystem.Auth.Api
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /publish .

ENTRYPOINT ["dotnet", "CreditAuthorizationSystem.Auth.Api.dll"]
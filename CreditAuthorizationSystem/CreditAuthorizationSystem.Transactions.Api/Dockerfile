FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CreditAuthorizationSystem.Transactions.Api/*.csproj CreditAuthorizationSystem.Transactions.Api/
COPY CreditAuthorizationSystem.Transactions.Application/*.csproj CreditAuthorizationSystem.Transactions.Application/
COPY CreditAuthorizationSystem.Transactions.Domain/*.csproj CreditAuthorizationSystem.Transactions.Domain/
COPY CreditAuthorizationSystem.Transactions.Infrastructure/*.csproj CreditAuthorizationSystem.Transactions.Infrastructure/

RUN dotnet restore CreditAuthorizationSystem.Transactions.Api/CreditAuthorizationSystem.Transactions.Api.csproj

COPY . .

WORKDIR /src/CreditAuthorizationSystem.Transactions.Api
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /publish .

ENTRYPOINT ["dotnet", "CreditAuthorizationSystem.Transactions.Api.dll"]
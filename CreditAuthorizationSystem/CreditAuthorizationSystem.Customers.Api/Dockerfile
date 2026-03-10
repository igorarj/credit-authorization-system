FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CreditAuthorizationSystem.Customers.Api/*.csproj CreditAuthorizationSystem.Customers.Api/
COPY CreditAuthorizationSystem.Customers.Application/*.csproj CreditAuthorizationSystem.Customers.Application/
COPY CreditAuthorizationSystem.Customers.Domain/*.csproj CreditAuthorizationSystem.Customers.Domain/
COPY CreditAuthorizationSystem.Customers.Infrastructure/*.csproj CreditAuthorizationSystem.Customers.Infrastructure/

RUN dotnet restore CreditAuthorizationSystem.Customers.Api/CreditAuthorizationSystem.Customers.Api.csproj

COPY . .

WORKDIR /src/CreditAuthorizationSystem.Customers.Api
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /publish .

ENTRYPOINT ["dotnet", "CreditAuthorizationSystem.Customers.Api.dll"]
namespace CreditAuthorizationSystem.Transactions.Application.Interfaces
{
    public interface IAuthApiClient
    {
        public Task<string> GetTokenAsync();
    }
}

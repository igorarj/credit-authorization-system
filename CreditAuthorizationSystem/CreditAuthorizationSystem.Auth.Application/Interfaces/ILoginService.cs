namespace CreditAuthorizationSystem.Auth.Application.Interfaces
{
    public interface ILoginService
    {
        Task<string?> LoginAsync(string email, string password);
    }
}

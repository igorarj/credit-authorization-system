namespace CreditAuthorizationSystem.Auth.Application.Interfaces
{
    public interface IRegisterUserService
    {
        Task RegisterAsync(string email, string password);
    }
}

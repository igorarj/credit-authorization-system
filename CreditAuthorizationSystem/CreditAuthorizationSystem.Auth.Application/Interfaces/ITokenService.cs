namespace CreditAuthorizationSystem.Auth.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(Guid userId, string email);
    }
}

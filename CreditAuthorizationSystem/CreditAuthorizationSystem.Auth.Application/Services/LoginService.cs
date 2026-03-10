using CreditAuthorizationSystem.Auth.Application.Interfaces;
using CreditAuthorizationSystem.Auth.Domain.Repositories;

namespace CreditAuthorizationSystem.Auth.Application.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public LoginService(
            IUserRepository userRepository,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                return null;

            var valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (!valid)
                return null;

            return await _tokenService.GenerateToken(user.Id, user.Email);
        }
    }
}

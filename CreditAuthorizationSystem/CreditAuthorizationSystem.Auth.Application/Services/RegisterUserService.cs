using CreditAuthorizationSystem.Auth.Application.Interfaces;
using CreditAuthorizationSystem.Auth.Domain.Models;
using CreditAuthorizationSystem.Auth.Domain.Repositories;

namespace CreditAuthorizationSystem.Auth.Application.Services
{
    public class RegisterUserService : IRegisterUserService
    {
        private readonly IUserRepository _userRepository;

        public RegisterUserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task RegisterAsync(string email, string password)
        {
            var existing = await _userRepository.GetByEmailAsync(email);

            if (existing != null)
                throw new Exception("Usuário já cadastrado");

            var hash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User(email, hash);

            await _userRepository.AddAsync(user);
        }
    }
}

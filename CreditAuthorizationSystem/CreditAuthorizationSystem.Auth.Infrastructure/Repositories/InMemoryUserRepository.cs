using CreditAuthorizationSystem.Auth.Domain.Models;
using CreditAuthorizationSystem.Auth.Domain.Repositories;

namespace CreditAuthorizationSystem.Auth.Infrastructure.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private static readonly List<User> _users = new();

        public Task AddAsync(User user)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            var user = _users.FirstOrDefault(x => x.Email == email);

            return Task.FromResult(user);
        }
    }
}

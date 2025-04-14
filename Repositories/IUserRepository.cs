using vm_api_backend_appservice.Models;

namespace vm_api_backend_appservice.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
    }
} 
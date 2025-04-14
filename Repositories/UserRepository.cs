using Microsoft.EntityFrameworkCore;
using vm_api_backend_appservice.Data;
using vm_api_backend_appservice.Exceptions;
using vm_api_backend_appservice.Models;

namespace vm_api_backend_appservice.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly VmDbContext _dbContext;

        public UserRepository(VmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                return user ?? throw new NotFoundException($"User with email {email} not found");
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Error retrieving user: {ex.Message}");
            }
        }
    }
} 
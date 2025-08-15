// Repositories/IUserRepository.cs
using FindTradie.Services.UserManagement.Entities;

namespace FindTradie.Services.UserManagement.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdWithDetailsAsync(Guid id);
    Task<List<User>> GetAllAsync(int pageNumber = 1, int pageSize = 20);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    Task<bool> PhoneExistsAsync(string phone, Guid? excludeId = null);
}

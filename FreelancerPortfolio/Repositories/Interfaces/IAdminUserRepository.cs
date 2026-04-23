using FreelancerPortfolio.Entities;

namespace FreelancerPortfolio.Repositories.Interfaces;

public interface IAdminUserRepository
{
    Task<AdminUser?> GetByUsernameAsync(string username);
    Task<AdminUser?> GetByIdAsync(int id);
    Task<AdminUser> CreateAsync(AdminUser user);
}

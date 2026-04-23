using FreelancerPortfolio.Data;
using FreelancerPortfolio.Entities;
using FreelancerPortfolio.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerPortfolio.Repositories.Implementations;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly AppDbContext _context;

    public AdminUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdminUser?> GetByUsernameAsync(string username)
        => await _context.AdminUsers
            .FirstOrDefaultAsync(u => u.Username == username);

    public async Task<AdminUser?> GetByIdAsync(int id)
        => await _context.AdminUsers.FindAsync(id);

    public async Task<AdminUser> CreateAsync(AdminUser user)
    {
        _context.AdminUsers.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}

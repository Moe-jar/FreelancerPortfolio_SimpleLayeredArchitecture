using FreelancerPortfolio.Data;
using FreelancerPortfolio.Entities;
using FreelancerPortfolio.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerPortfolio.Repositories.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
        => await _context.Categories.OrderBy(c => c.Name).ToListAsync();

    public async Task<Category?> GetByIdAsync(int id)
        => await _context.Categories.FindAsync(id);

    public async Task<Category?> GetBySlugAsync(string slug)
        => await _context.Categories.FirstOrDefaultAsync(c => c.Slug == slug);

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        => await _context.Categories
            .AnyAsync(c => c.Slug == slug && (excludeId == null || c.Id != excludeId));

    public async Task<Category> CreateAsync(Category entity)
    {
        _context.Categories.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Category> UpdateAsync(Category entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Categories.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Categories.FindAsync(id);
        if (entity is null) return false;
        _context.Categories.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}

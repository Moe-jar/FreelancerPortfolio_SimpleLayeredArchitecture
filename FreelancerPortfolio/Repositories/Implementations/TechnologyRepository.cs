using FreelancerPortfolio.Data;
using FreelancerPortfolio.Entities;
using FreelancerPortfolio.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerPortfolio.Repositories.Implementations;

public class TechnologyRepository : ITechnologyRepository
{
    private readonly AppDbContext _context;

    public TechnologyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Technology>> GetAllAsync()
        => await _context.Technologies.OrderBy(t => t.Name).ToListAsync();

    public async Task<Technology?> GetByIdAsync(int id)
        => await _context.Technologies.FindAsync(id);

    public async Task<Technology?> GetBySlugAsync(string slug)
        => await _context.Technologies.FirstOrDefaultAsync(t => t.Slug == slug);

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        => await _context.Technologies
            .AnyAsync(t => t.Slug == slug && (excludeId == null || t.Id != excludeId));

    public async Task<Technology> CreateAsync(Technology entity)
    {
        _context.Technologies.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Technology> UpdateAsync(Technology entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Technologies.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Technologies.FindAsync(id);
        if (entity is null) return false;
        _context.Technologies.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}

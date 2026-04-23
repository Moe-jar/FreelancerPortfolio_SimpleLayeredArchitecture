using FreelancerPortfolio.Data;
using FreelancerPortfolio.Entities;
using FreelancerPortfolio.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerPortfolio.Repositories.Implementations;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
        => await _context.Projects
            .Include(p => p.Category)
            .Include(p => p.ProjectTechnologies)
                .ThenInclude(pt => pt.Technology)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();

    public async Task<Project?> GetByIdAsync(int id)
        => await _context.Projects
            .Include(p => p.Category)
            .Include(p => p.ProjectTechnologies)
                .ThenInclude(pt => pt.Technology)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Project?> GetByIdWithDetailsAsync(int id)
        => await GetByIdAsync(id);

    public async Task<Project?> GetBySlugAsync(string slug)
        => await _context.Projects
            .Include(p => p.Category)
            .Include(p => p.ProjectTechnologies)
                .ThenInclude(pt => pt.Technology)
            .FirstOrDefaultAsync(p => p.Slug == slug);

    public async Task<IEnumerable<Project>> GetPublishedAsync()
        => await _context.Projects
            .Include(p => p.Category)
            .Include(p => p.ProjectTechnologies)
                .ThenInclude(pt => pt.Technology)
            .Where(p => p.IsPublished)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();

    public async Task<IEnumerable<Project>> GetByCategoryAsync(int categoryId)
        => await _context.Projects
            .Include(p => p.Category)
            .Include(p => p.ProjectTechnologies)
                .ThenInclude(pt => pt.Technology)
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();

    public async Task<IEnumerable<Project>> GetByTechnologyAsync(int technologyId)
        => await _context.Projects
            .Include(p => p.Category)
            .Include(p => p.ProjectTechnologies)
                .ThenInclude(pt => pt.Technology)
            .Where(p => p.ProjectTechnologies.Any(pt => pt.TechnologyId == technologyId))
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        => await _context.Projects
            .AnyAsync(p => p.Slug == slug && (excludeId == null || p.Id != excludeId));

    public async Task<Project> CreateAsync(Project entity)
    {
        _context.Projects.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Project> UpdateAsync(Project entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Projects.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Projects.FindAsync(id);
        if (entity is null) return false;
        _context.Projects.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}

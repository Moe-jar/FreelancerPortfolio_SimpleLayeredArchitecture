using FreelancerPortfolio.Entities;

namespace FreelancerPortfolio.Repositories.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetBySlugAsync(string slug);
    Task<IEnumerable<Project>> GetPublishedAsync();
    Task<IEnumerable<Project>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<Project>> GetByTechnologyAsync(int technologyId);
    Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
    Task<Project?> GetByIdWithDetailsAsync(int id);
}

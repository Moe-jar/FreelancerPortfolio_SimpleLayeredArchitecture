using FreelancerPortfolio.Entities;

namespace FreelancerPortfolio.Repositories.Interfaces;

public interface ITechnologyRepository : IRepository<Technology>
{
    Task<Technology?> GetBySlugAsync(string slug);
    Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
}

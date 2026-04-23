using FreelancerPortfolio.DTOs;

namespace FreelancerPortfolio.Services.Interfaces;

public interface ITechnologyService
{
    Task<IEnumerable<TechnologyDto>> GetAllAsync();
    Task<TechnologyDto> GetByIdAsync(int id);
    Task<TechnologyDto> GetBySlugAsync(string slug);
    Task<TechnologyDto> CreateAsync(CreateTechnologyDto dto);
    Task<TechnologyDto> UpdateAsync(int id, UpdateTechnologyDto dto);
    Task DeleteAsync(int id);
}

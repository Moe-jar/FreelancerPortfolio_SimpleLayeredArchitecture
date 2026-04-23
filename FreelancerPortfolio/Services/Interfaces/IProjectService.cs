using FreelancerPortfolio.DTOs;

namespace FreelancerPortfolio.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllAsync();
    Task<ProjectDto> GetByIdAsync(int id);
    Task<ProjectDto> GetBySlugAsync(string slug);
    Task<IEnumerable<ProjectDto>> GetPublishedAsync();
    Task<IEnumerable<ProjectDto>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<ProjectDto>> GetByTechnologyAsync(int technologyId);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto);
    Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto dto);
    Task DeleteAsync(int id);
}

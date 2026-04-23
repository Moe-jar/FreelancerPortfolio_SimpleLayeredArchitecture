using AutoMapper;
using FreelancerPortfolio.Data;
using FreelancerPortfolio.DTOs;
using FreelancerPortfolio.Entities;
using FreelancerPortfolio.Repositories.Interfaces;
using FreelancerPortfolio.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelancerPortfolio.Services.Implementations;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repo;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public ProjectService(IProjectRepository repo, IMapper mapper, AppDbContext context)
    {
        _repo = repo;
        _mapper = mapper;
        _context = context;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllAsync()
    {
        var projects = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<ProjectDto>>(projects);
    }

    public async Task<ProjectDto> GetByIdAsync(int id)
    {
        var project = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Project with id {id} not found.");
        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto> GetBySlugAsync(string slug)
    {
        var project = await _repo.GetBySlugAsync(slug)
            ?? throw new KeyNotFoundException($"Project with slug '{slug}' not found.");
        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<IEnumerable<ProjectDto>> GetPublishedAsync()
    {
        var projects = await _repo.GetPublishedAsync();
        return _mapper.Map<IEnumerable<ProjectDto>>(projects);
    }

    public async Task<IEnumerable<ProjectDto>> GetByCategoryAsync(int categoryId)
    {
        var projects = await _repo.GetByCategoryAsync(categoryId);
        return _mapper.Map<IEnumerable<ProjectDto>>(projects);
    }

    public async Task<IEnumerable<ProjectDto>> GetByTechnologyAsync(int technologyId)
    {
        var projects = await _repo.GetByTechnologyAsync(technologyId);
        return _mapper.Map<IEnumerable<ProjectDto>>(projects);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
    {
        if (await _repo.SlugExistsAsync(dto.Slug))
            throw new InvalidOperationException($"A project with slug '{dto.Slug}' already exists.");

        var entity = _mapper.Map<Project>(dto);

        // Set technologies
        if (dto.TechnologyIds.Any())
        {
            entity.ProjectTechnologies = dto.TechnologyIds
                .Select(tid => new ProjectTechnology { TechnologyId = tid })
                .ToList();
        }

        var created = await _repo.CreateAsync(entity);
        var withDetails = await _repo.GetByIdAsync(created.Id);
        return _mapper.Map<ProjectDto>(withDetails!);
    }

    public async Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto dto)
    {
        var existing = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Project with id {id} not found.");

        if (await _repo.SlugExistsAsync(dto.Slug, id))
            throw new InvalidOperationException($"A project with slug '{dto.Slug}' already exists.");

        _mapper.Map(dto, existing);

        // Update technologies
        var existingTechs = await _context.ProjectTechnologies
            .Where(pt => pt.ProjectId == id)
            .ToListAsync();
        _context.ProjectTechnologies.RemoveRange(existingTechs);

        existing.ProjectTechnologies = dto.TechnologyIds
            .Select(tid => new ProjectTechnology { ProjectId = id, TechnologyId = tid })
            .ToList();

        var updated = await _repo.UpdateAsync(existing);
        var withDetails = await _repo.GetByIdAsync(updated.Id);
        return _mapper.Map<ProjectDto>(withDetails!);
    }

    public async Task DeleteAsync(int id)
    {
        var deleted = await _repo.DeleteAsync(id);
        if (!deleted)
            throw new KeyNotFoundException($"Project with id {id} not found.");
    }
}

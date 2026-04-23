using AutoMapper;
using FreelancerPortfolio.DTOs;
using FreelancerPortfolio.Entities;
using FreelancerPortfolio.Repositories.Interfaces;
using FreelancerPortfolio.Services.Interfaces;

namespace FreelancerPortfolio.Services.Implementations;

public class TechnologyService : ITechnologyService
{
    private readonly ITechnologyRepository _repo;
    private readonly IMapper _mapper;

    public TechnologyService(ITechnologyRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TechnologyDto>> GetAllAsync()
    {
        var techs = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<TechnologyDto>>(techs);
    }

    public async Task<TechnologyDto> GetByIdAsync(int id)
    {
        var tech = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Technology with id {id} not found.");
        return _mapper.Map<TechnologyDto>(tech);
    }

    public async Task<TechnologyDto> GetBySlugAsync(string slug)
    {
        var tech = await _repo.GetBySlugAsync(slug)
            ?? throw new KeyNotFoundException($"Technology with slug '{slug}' not found.");
        return _mapper.Map<TechnologyDto>(tech);
    }

    public async Task<TechnologyDto> CreateAsync(CreateTechnologyDto dto)
    {
        if (await _repo.SlugExistsAsync(dto.Slug))
            throw new InvalidOperationException($"A technology with slug '{dto.Slug}' already exists.");

        var entity = _mapper.Map<Technology>(dto);
        var created = await _repo.CreateAsync(entity);
        return _mapper.Map<TechnologyDto>(created);
    }

    public async Task<TechnologyDto> UpdateAsync(int id, UpdateTechnologyDto dto)
    {
        var existing = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Technology with id {id} not found.");

        if (await _repo.SlugExistsAsync(dto.Slug, id))
            throw new InvalidOperationException($"A technology with slug '{dto.Slug}' already exists.");

        _mapper.Map(dto, existing);
        var updated = await _repo.UpdateAsync(existing);
        return _mapper.Map<TechnologyDto>(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var deleted = await _repo.DeleteAsync(id);
        if (!deleted)
            throw new KeyNotFoundException($"Technology with id {id} not found.");
    }
}

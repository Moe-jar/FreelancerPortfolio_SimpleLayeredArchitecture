using AutoMapper;
using FreelancerPortfolio.DTOs;
using FreelancerPortfolio.Entities;
using FreelancerPortfolio.Repositories.Interfaces;
using FreelancerPortfolio.Services.Interfaces;

namespace FreelancerPortfolio.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        var category = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Category with id {id} not found.");
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> GetBySlugAsync(string slug)
    {
        var category = await _repo.GetBySlugAsync(slug)
            ?? throw new KeyNotFoundException($"Category with slug '{slug}' not found.");
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        if (await _repo.SlugExistsAsync(dto.Slug))
            throw new InvalidOperationException($"A category with slug '{dto.Slug}' already exists.");

        var entity = _mapper.Map<Category>(dto);
        var created = await _repo.CreateAsync(entity);
        return _mapper.Map<CategoryDto>(created);
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var existing = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Category with id {id} not found.");

        if (await _repo.SlugExistsAsync(dto.Slug, id))
            throw new InvalidOperationException($"A category with slug '{dto.Slug}' already exists.");

        _mapper.Map(dto, existing);
        var updated = await _repo.UpdateAsync(existing);
        return _mapper.Map<CategoryDto>(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var deleted = await _repo.DeleteAsync(id);
        if (!deleted)
            throw new KeyNotFoundException($"Category with id {id} not found.");
    }
}

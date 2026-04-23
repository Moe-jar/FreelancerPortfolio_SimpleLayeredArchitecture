using FreelancerPortfolio.DTOs;
using FreelancerPortfolio.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerPortfolio.Controllers;

[ApiController]
[Route("api/project")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectController(IProjectService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(new { success = true, data = result });
    }

    [HttpGet("published")]
    public async Task<IActionResult> GetPublished()
    {
        var result = await _service.GetPublishedAsync();
        return Ok(new { success = true, data = result });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(new { success = true, data = result });
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _service.GetBySlugAsync(slug);
        return Ok(new { success = true, data = result });
    }

    [HttpGet("category/{categoryId:int}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var result = await _service.GetByCategoryAsync(categoryId);
        return Ok(new { success = true, data = result });
    }

    [HttpGet("technology/{technologyId:int}")]
    public async Task<IActionResult> GetByTechnology(int technologyId)
    {
        var result = await _service.GetByTechnologyAsync(technologyId);
        return Ok(new { success = true, data = result });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { success = true, data = result });
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return Ok(new { success = true, data = result });
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok(new { success = true, message = "Project deleted successfully." });
    }
}

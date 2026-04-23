namespace FreelancerPortfolio.DTOs;

public record CategoryDto(int Id, string Name, string Slug, string? Description, DateTime CreatedAt);

public record CreateCategoryDto(string Name, string Slug, string? Description);

public record UpdateCategoryDto(string Name, string Slug, string? Description);

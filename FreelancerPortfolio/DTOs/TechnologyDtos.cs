namespace FreelancerPortfolio.DTOs;

public record TechnologyDto(int Id, string Name, string Slug, string? IconUrl, DateTime CreatedAt);

public record CreateTechnologyDto(string Name, string Slug, string? IconUrl);

public record UpdateTechnologyDto(string Name, string Slug, string? IconUrl);

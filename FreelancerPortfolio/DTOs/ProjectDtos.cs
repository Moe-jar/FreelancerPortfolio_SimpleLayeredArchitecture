namespace FreelancerPortfolio.DTOs;

public record ProjectDto(
    int Id,
    string Title,
    string Slug,
    string? ShortDescription,
    string? LongDescription,
    string? ThumbnailUrl,
    string? LiveUrl,
    string? RepoUrl,
    bool IsPublished,
    int DisplayOrder,
    DateTime CreatedAt,
    CategoryDto Category,
    IEnumerable<TechnologyDto> Technologies
);

public record CreateProjectDto(
    string Title,
    string Slug,
    string? ShortDescription,
    string? LongDescription,
    string? ThumbnailUrl,
    string? LiveUrl,
    string? RepoUrl,
    bool IsPublished,
    int DisplayOrder,
    int CategoryId,
    IEnumerable<int> TechnologyIds
);

public record UpdateProjectDto(
    string Title,
    string Slug,
    string? ShortDescription,
    string? LongDescription,
    string? ThumbnailUrl,
    string? LiveUrl,
    string? RepoUrl,
    bool IsPublished,
    int DisplayOrder,
    int CategoryId,
    IEnumerable<int> TechnologyIds
);

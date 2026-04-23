using AutoMapper;
using FreelancerPortfolio.DTOs;
using FreelancerPortfolio.Entities;

namespace FreelancerPortfolio.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Category
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Technology
        CreateMap<Technology, TechnologyDto>();
        CreateMap<CreateTechnologyDto, Technology>();
        CreateMap<UpdateTechnologyDto, Technology>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Project
        CreateMap<Project, ProjectDto>()
            .ForCtorParam("Technologies",
                opt => opt.MapFrom(src => src.ProjectTechnologies.Select(pt => pt.Technology)));

        CreateMap<CreateProjectDto, Project>()
            .ForMember(dest => dest.ProjectTechnologies, opt => opt.Ignore());

        CreateMap<UpdateProjectDto, Project>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ProjectTechnologies, opt => opt.Ignore());
    }
}

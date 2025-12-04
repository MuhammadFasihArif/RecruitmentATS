using AutoMapper;
using RecruitmentATS.Application.DTOs;
using RecruitmentATS.Domain.Entities;

namespace RecruitmentATS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Job mappings
        CreateMap<Job, JobDto>();
        CreateMap<CreateJobRequest, Job>();
        CreateMap<UpdateJobRequest, Job>();

        // Application mappings
        CreateMap<Domain.Entities.Application, ApplicationDto>()
            .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
            .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => $"{src.Candidate.FirstName} {src.Candidate.LastName}"))
            .ForMember(dest => dest.CandidateEmail, opt => opt.MapFrom(src => src.Candidate.Email))
            .ForMember(dest => dest.ResumeUrl, opt => opt.MapFrom(src => src.Candidate.ResumeUrl))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

        // Candidate mappings
        CreateMap<ApplyToJobRequest, Candidate>();
    }
}

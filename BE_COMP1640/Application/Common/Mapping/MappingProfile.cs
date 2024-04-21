using Application.Features.Auth.Commands.CreateContributorAccount;
using Application.Features.Auth.Commands.CreateCoordinatorAccount;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.UpdateProfile;
using Application.Features.Auth.Queries.GetSelfProfile;
using Application.Features.Auth.Queries.ListRole;
using Application.Features.Auth.Queries.ListUser;
using Application.Features.Comments.Commands.CreateComment;
using Application.Features.Comments.Queries.ListComment;
using Application.Features.Contributions.Commands.CreateContribution;
using Application.Features.Contributions.Commands.UpdateContribution;
using Application.Features.Contributions.Queries.GetContribution;
using Application.Features.Contributions.Queries.ListContribution;
using Application.Features.Faculties.Commands.CreateFaculty;
using Application.Features.Faculties.Queries.ListFaculty;
using Application.Features.Feedbacks.Commands.CreateFeedback;
using Application.Features.Feedbacks.Queries.ListFeedback;
using Application.Features.Periods.Commands.CreatePeriod;
using Application.Features.Periods.Commands.UpdatePeriod;
using Application.Features.Periods.Queries.ListPeriod;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Mapping of contributions 
        CreateMap<Contribution, ListContributionDto>()
            .ForMember(dest => dest.CreatedByEmail, opt => opt.MapFrom(src => src.CreatedBy.Email))
            .ForMember(dest => dest.CreatedByFullName,
                opt => opt.MapFrom(src => src.CreatedBy.FirstName + " " + src.CreatedBy.LastName))
            .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.Image.UrlFilePath))
            .ForMember(dest => dest.FacultyName,
                opt => opt.MapFrom(src => src.CreatedBy.Faculty != null ? src.CreatedBy.Faculty.Name : null))
            .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.CreatedBy.FacultyId))
            .ForMember(dest => dest.AcademicYear, opt => opt.MapFrom(src => src.Period.AcademicYear))
            .ForMember(dest => dest.FirstSubmissionDeadline,
                opt => opt.MapFrom(src => src.Period.FirstSubmissionDeadline))
            .ForMember(dest => dest.SecondSubmissionDeadline,
                opt => opt.MapFrom(src => src.Period.SecondSubmissionDeadline));

        CreateMap<Contribution, GetContributionDto>()
            .ForMember(dest => dest.CreatedByEmail, opt => opt.MapFrom(src => src.CreatedBy.Email))
            .ForMember(dest => dest.CreatedByFullName,
                opt => opt.MapFrom(src => src.CreatedBy.FirstName + " " + src.CreatedBy.LastName))
            .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.Image.UrlFilePath))
            .ForMember(dest => dest.DocumentUrl, opt => opt.MapFrom(src => src.Document.UrlFilePath))
            .ForMember(dest => dest.FacultyName,
                opt => opt.MapFrom(src => src.CreatedBy.Faculty != null ? src.CreatedBy.Faculty.Name : null))
            .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.CreatedBy.FacultyId))
            .ForMember(dest => dest.LoveCount, opt => opt.MapFrom(src => src.Ratings.Count));

        CreateMap<UpdateContributionCommand, Contribution>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Image, opt => opt.Ignore())
            .ForMember(dest => dest.Document, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Title)))
            .ForMember(dest => dest.Description, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Description)));

        CreateMap<CreateContributionCommand, Contribution>()
            .ForMember(dest => dest.Image, opt => opt.Ignore())
            .ForMember(dest => dest.Document, opt => opt.Ignore());




        //Mapping of auth
        CreateMap<LoginCommand, ApplicationUser>().ReverseMap();

        CreateMap<RegisterCommand, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom((src => src.Email)));


        CreateMap<CreateContributorAccountCommand, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

        CreateMap<CreateCoordinatorAccountCommand, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.FacultyId));

        CreateMap<UpdateProfileCommand, ApplicationUser>()
            .ForMember(dest => dest.Avatar, opt => opt.Ignore())
            .ForMember(dest => dest.LastName, opt => opt.Condition(src => !string.IsNullOrEmpty(src.LastName)))
            .ForMember(dest => dest.FirstName, opt => opt.Condition(src => !string.IsNullOrEmpty(src.FirstName)));

        CreateMap<ApplicationUser, ListUserDto>()
            .ForMember(dest => dest.FacultyName,
                opt => opt.MapFrom(src => src.Faculty != null ? src.Faculty.Name : null))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Roles.FirstOrDefault().Name))
            //.ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Roles.Select(r => r.Name).Aggregate((current, next) => current + ", " + next)))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Avatar.UrlFilePath));

        CreateMap<ApplicationRole, ListRoleDto>();

        CreateMap<ApplicationUser, GetSelfProfileDto>()
            .ForMember(dest => dest.FacultyName,
                opt => opt.MapFrom(src => src.Faculty != null ? src.Faculty.Name : null))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Roles.FirstOrDefault().Name))
            .ForMember(dest => dest.NumberOfContributions, opt => opt.MapFrom(src => src.Contributions.Count))
            //.ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Roles.Select(r => r.Name).Aggregate((current, next) => current + ", " + next)))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Avatar.UrlFilePath));






        //Mapping of Period
        CreateMap<CreatePeriodCommand, Period>();
        CreateMap<UpdatePeriodCommand, Period>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());


        CreateMap<Period, ListPeriodDto>()
            .ForMember(dest => dest.CreatedByEmail, opt => opt.MapFrom(src => src.CreatedBy.Email))
            .ForMember(dest => dest.CreatedByFullName,
                opt => opt.MapFrom(src => src.CreatedBy.FirstName + " " + src.CreatedBy.LastName));




        //Mapping of Faculty
        CreateMap<CreateFacultyCommand, Faculty>();
        CreateMap<Faculty, ListFacultyDto>().ReverseMap();




        //Mapping of Feedback
        CreateMap<CreateFeedbackCommand, Feedback>();

        CreateMap<Feedback, ListFeedbackDto>()
            .ForMember(dest => dest.CreatedByEmail, opt => opt.MapFrom(src => src.CreatedBy.Email))
            .ForMember(dest => dest.CreatedByFullName, opt => opt.MapFrom(src => src.CreatedBy.FirstName + " " + src.CreatedBy.LastName));


        //Mapping of Comment
        CreateMap<CreateCommentCommand, Comment>();

        CreateMap<Comment, ListCommentDto>()
            .ForMember(dest => dest.CreatedByEmail, opt => opt.MapFrom(src => src.CreatedBy.Email))
            .ForMember(dest => dest.CreatedByFullName,
                opt => opt.MapFrom(src => src.CreatedBy.FirstName + " " + src.CreatedBy.LastName));
    }


}
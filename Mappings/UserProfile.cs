using AutoMapper;
using HRMS.API.Models.Response;
using HRMS.API.Models;

namespace HRMS.API.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.Emp_Code, opt => opt.MapFrom(src => src.Emp_Code))
                .ForMember(dest => dest.Emp_Emailaddress, opt => opt.MapFrom(src => src.Emp_Emailaddress))
                .ForMember(dest => dest.emp_status, opt => opt.MapFrom(src => src.emp_status))
                .ForMember(dest => dest.Emp_Name, opt => opt.MapFrom(src => src.Emp_Name))
                .ForMember(dest => dest.ActualHrReleaseDate, opt => opt.MapFrom(src => src.ActualHrReleaseDate));
        }
    }
}
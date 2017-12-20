using AutoMapper;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Api.DependencyResolution
{
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<User, UserSummaryViewModel>().ForMember(d => d.Href, o => o.Ignore());
            CreateMap<User, UserViewModel>();
        }
    }
}
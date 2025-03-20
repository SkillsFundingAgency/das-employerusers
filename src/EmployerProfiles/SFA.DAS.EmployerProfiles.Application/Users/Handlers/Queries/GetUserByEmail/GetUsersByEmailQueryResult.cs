using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByEmail;

public class GetUsersByEmailQueryResult
{
    public List<UserProfile> UserProfiles { get; set; }
}
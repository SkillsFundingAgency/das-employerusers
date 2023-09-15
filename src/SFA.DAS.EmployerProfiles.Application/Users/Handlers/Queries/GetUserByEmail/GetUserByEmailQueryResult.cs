using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByEmail;

public class GetUserByEmailQueryResult
{
    public UserProfile? UserProfile { get; set; }
}
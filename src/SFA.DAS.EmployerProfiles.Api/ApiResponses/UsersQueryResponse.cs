using System.Collections.Generic;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Api.ApiResponses;

public class UsersQueryResponse
{
    public List<UserProfile> Users { get; set; } = new ();
}

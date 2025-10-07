using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserById;

public class GetUserByIdQueryResult
{
    public UserProfile? UserProfile { get; set; }
}
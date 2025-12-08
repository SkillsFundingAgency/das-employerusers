
namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByEmail;

public class GetUsersByEmailQueryHandler(IUserProfileRepository repository)
    : IRequestHandler<GetUsersByEmailQuery, GetUsersByEmailQueryResult>
{
    public async Task<GetUsersByEmailQueryResult> Handle(GetUsersByEmailQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetAllProfilesForEmailAddress(request.Email);

        return new GetUsersByEmailQueryResult
        {
            UserProfiles = result.Select(x=>(UserProfile)x!).ToList()
        };
    }
}

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByEmail;

public class GetUsersByEmailQueryHandler :IRequestHandler<GetUsersByEmailQuery, GetUsersByEmailQueryResult>
{
    private readonly IUserProfileRepository _repository;

    public GetUsersByEmailQueryHandler(IUserProfileRepository repository)
    {
        _repository = repository;
    }
    public async Task<GetUsersByEmailQueryResult> Handle(GetUsersByEmailQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllProfilesForEmailAddress(request.Email);

        return new GetUsersByEmailQueryResult
        {
            UserProfiles = result.Select(x=>(UserProfile)x!).ToList()
        };
    }
}
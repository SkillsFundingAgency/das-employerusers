namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByGovIdentifier;

public class GetUserByGovIdentifierQueryHandler(IUserProfileRepository repository)
    : IRequestHandler<GetUserByGovIdentifierQuery, GetUserByGovIdentifierQueryResult>
{
    public async Task<GetUserByGovIdentifierQueryResult> Handle(GetUserByGovIdentifierQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetByGovIdentifier(request.GovIdentifier);

        return new GetUserByGovIdentifierQueryResult
        {
            UserProfile = result
        };
    }
}
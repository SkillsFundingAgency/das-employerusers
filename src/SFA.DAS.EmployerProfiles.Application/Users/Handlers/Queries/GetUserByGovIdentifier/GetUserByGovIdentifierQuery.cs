namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByGovIdentifier;

public class GetUserByGovIdentifierQuery : IRequest<GetUserByGovIdentifierQueryResult>
{
    public string GovIdentifier { get; init; }
}
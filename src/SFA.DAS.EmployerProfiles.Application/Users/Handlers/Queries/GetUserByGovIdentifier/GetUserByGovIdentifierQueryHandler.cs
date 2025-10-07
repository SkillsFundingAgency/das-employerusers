using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByGovIdentifier;

public class GetUserByGovIdentifierQueryHandler : IRequestHandler<GetUserByGovIdentifierQuery, GetUserByGovIdentifierQueryResult>
{
    private readonly IUserProfileRepository _repository;

    public GetUserByGovIdentifierQueryHandler(IUserProfileRepository repository)
    {
        _repository = repository;
    }
    public async Task<GetUserByGovIdentifierQueryResult> Handle(GetUserByGovIdentifierQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByGovIdentifier(request.GovIdentifier);

        return new GetUserByGovIdentifierQueryResult
        {
            UserProfile = result
        };
    }
}
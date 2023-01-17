using MediatR;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByEmail;

public class GetUserByEmailQueryHandler :IRequestHandler<GetUserByEmailQuery, GetUserByEmailQueryResult>
{
    private readonly IUserProfileRepository _repository;

    public GetUserByEmailQueryHandler(IUserProfileRepository repository)
    {
        _repository = repository;
    }
    public async Task<GetUserByEmailQueryResult> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByEmail(request.Email);

        return new GetUserByEmailQueryResult
        {
            UserProfile = result
        };
    }
}
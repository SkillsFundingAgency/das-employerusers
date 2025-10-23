using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, GetUsersQueryResult>
{
    private readonly IUserProfileRepository _repository;

    public GetUsersQueryHandler(IUserProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetUsersQueryResult> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllUsers(request.PageSize, request.PageNumber);

        return new GetUsersQueryResult
        {
            UserProfiles = result.UserProfiles.Select(x => (UserProfile)x!).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

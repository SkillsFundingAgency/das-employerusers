namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUsers;

public class GetUsersQueryHandler(IUserProfileRepository repository)
    : IRequestHandler<GetUsersQuery, GetUsersQueryResult>
{
    public async Task<GetUsersQueryResult> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetAllUsers(request.PageSize, request.PageNumber);

        return new GetUsersQueryResult
        {
            UserProfiles = result.UserProfiles.Select(x => (UserProfile)x!).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUsers;

public class GetUsersQuery : IRequest<GetUsersQueryResult>
{
    public int PageSize { get; init; } = 1000;
    public int PageNumber { get; init; } = 1;
}

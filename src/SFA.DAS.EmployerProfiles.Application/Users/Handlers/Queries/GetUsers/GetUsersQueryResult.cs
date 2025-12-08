namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUsers;

public class GetUsersQueryResult
{
    public List<UserProfile> UserProfiles { get; init; } = [];
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

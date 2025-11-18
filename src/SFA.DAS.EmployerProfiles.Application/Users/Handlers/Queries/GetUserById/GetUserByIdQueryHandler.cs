namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserById;

public class GetUserByIdQueryHandler(IUserProfileRepository repository)
    : IRequestHandler<GetUserByIdQuery, GetUserByIdQueryResult>
{
    public async Task<GetUserByIdQueryResult> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetById(request.Id);

        return new GetUserByIdQueryResult
        {
            UserProfile = result
        };
    }
}
using MediatR;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpsertUser;

public class UpsertUserRequestHandler : IRequestHandler<UpsertUserRequest, UpsertUserResult>
{
    private readonly IUserProfileRepository _userProfileRepository;

    public UpsertUserRequestHandler(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }
    public async Task<UpsertUserResult> Handle(UpsertUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _userProfileRepository.Upsert(new UserProfileEntity
        {
            Id = request.Id.ToString(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            GovUkIdentifier = request.GovUkIdentifier
        });

        return new UpsertUserResult
        {
            UserProfile = result!
        };
    }
}
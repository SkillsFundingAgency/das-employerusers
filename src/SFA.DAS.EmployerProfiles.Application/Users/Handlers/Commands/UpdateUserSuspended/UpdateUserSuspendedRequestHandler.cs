using MediatR;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpdateUserSuspended;

public class UpdateUserSuspendedRequestHandler : IRequestHandler<UpdateUserSuspendedRequest, UpdateUserSuspendedResult>
{
    private readonly IUserProfileRepository _userProfileRepository;

    public UpdateUserSuspendedRequestHandler(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }
    
    public async Task<UpdateUserSuspendedResult> Handle(UpdateUserSuspendedRequest request, CancellationToken cancellationToken)
    {
        var result = await _userProfileRepository.UpdateUserSuspendedFlag(request.Id, request.UserSuspended);

        return new UpdateUserSuspendedResult
        {
            Updated = result
        };
    }
}
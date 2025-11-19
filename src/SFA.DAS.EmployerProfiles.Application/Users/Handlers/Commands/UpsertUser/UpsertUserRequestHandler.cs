using System.ComponentModel.DataAnnotations;
using SFA.DAS.EmployerProfiles.Domain.RequestHandlers;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpsertUser;

public class UpsertUserRequestHandler(
    IUserProfileRepository userProfileRepository,
    IValidator<UpsertUserRequest> validator)
    : IRequestHandler<UpsertUserRequest, UpsertUserResult>
{
    public async Task<UpsertUserResult> Handle(UpsertUserRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid())
        {
            throw new ValidationException(validationResult.DataAnnotationResult,null, null);
        }
        
        var result = await userProfileRepository.Upsert(new UserProfileEntity
        {
            Id = request.Id.ToString(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            GovUkIdentifier = request.GovUkIdentifier
        });

        return new UpsertUserResult
        {
            UserProfile = result.Item1!,
            IsCreated = result.Item2
        };
    }
}
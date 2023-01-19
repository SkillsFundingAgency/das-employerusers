using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EmployerProfiles.Domain.RequestHandlers;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpsertUser;

public class UpsertUserRequestHandler : IRequestHandler<UpsertUserRequest, UpsertUserResult>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IValidator<UpsertUserRequest> _validator;

    public UpsertUserRequestHandler(IUserProfileRepository userProfileRepository, IValidator<UpsertUserRequest> validator)
    {
        _userProfileRepository = userProfileRepository;
        _validator = validator;
    }
    public async Task<UpsertUserResult> Handle(UpsertUserRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid())
        {
            throw new ValidationException(validationResult.DataAnnotationResult,null, null);
        }
        
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
            UserProfile = result.Item1!,
            IsCreated = result.Item2
        };
    }
}
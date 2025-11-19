
namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpsertUser;

public class UpsertUserRequest : IRequest<UpsertUserResult>
{
    public Guid Id { get; set; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string GovUkIdentifier { get; init; }
    public string Email { get; set; }
}
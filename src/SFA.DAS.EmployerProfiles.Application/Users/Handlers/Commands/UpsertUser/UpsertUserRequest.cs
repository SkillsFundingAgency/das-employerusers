
namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpsertUser;

public class UpsertUserRequest : IRequest<UpsertUserResult>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string GovUkIdentifier { get; set; }
    public string Email { get; set; }
}
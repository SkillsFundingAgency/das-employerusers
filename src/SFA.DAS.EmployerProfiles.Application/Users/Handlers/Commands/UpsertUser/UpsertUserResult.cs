namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpsertUser;

public class UpsertUserResult
{
    public UserProfile UserProfile { get; init; }
    public bool IsCreated { get; set; }
}
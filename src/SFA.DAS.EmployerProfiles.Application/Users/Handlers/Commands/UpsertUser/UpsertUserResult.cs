namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpsertUser;

public class UpsertUserResult
{
    public UserProfile UserProfile { get; set; }
    public bool IsCreated { get; set; }
}
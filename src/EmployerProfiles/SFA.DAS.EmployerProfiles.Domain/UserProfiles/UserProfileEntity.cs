namespace SFA.DAS.EmployerProfiles.Domain.UserProfiles;

public class UserProfileEntity
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? GovUkIdentifier { get; set; }
    public bool IsSuspended { get; set; }
}
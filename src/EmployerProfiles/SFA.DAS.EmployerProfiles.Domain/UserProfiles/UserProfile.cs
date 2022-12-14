namespace SFA.DAS.EmployerProfiles.Domain.UserProfiles;

public class UserProfile
{
    public static implicit operator UserProfile?(UserProfileEntity? source)
    {
        if (source == null)
        {
            return null;
        }
        return new UserProfile
        {
            Email = source.Email,
            Id = Guid.Parse(source.Id),
            DisplayName = $"{source.FirstName} {source.LastName}",
            FirstName = source.FirstName,
            LastName = source.LastName,
            GovUkIdentifier = source.GovUkIdentifier,
            IsSuspended = source.IsSuspended
        };
    }

    public bool IsSuspended { get; set; }

    public Guid Id { get; set; }
    public string DisplayName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string GovUkIdentifier { get; set; }
}
using System.ComponentModel.DataAnnotations;

    namespace SFA.DAS.EmployerProfiles.Api.ApiRequests;

public class UserProfileRequest
{
    [Required]
    public string Email { get; }
    public string? FirstName { get; }
    public string? LastName { get; }
    [Required]
    public string GovIdentifier { get; }
}
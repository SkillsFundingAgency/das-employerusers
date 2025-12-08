using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerProfiles.Api.ApiRequests;

public class ChangeUserStatusRequest
{
    [Required]
    public string ChangedByUserId { get; set; }

    [Required]
    [EmailAddress]
    public string ChangedByEmail { get; set; }
}


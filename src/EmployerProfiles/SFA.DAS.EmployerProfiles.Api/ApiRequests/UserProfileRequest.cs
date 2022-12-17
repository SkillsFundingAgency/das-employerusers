using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EmployerProfiles.Api.ApiRequests;

public class UserProfileRequest
{
    [FromQuery]
    [Required]
    public string Email { get; set; }
    [FromQuery]
    public string FirstName { get; set; }
    [FromQuery]
    public string LastName { get; set; }
    [FromQuery]
    [Required]
    public string GovIdentifier { get; set; }
}
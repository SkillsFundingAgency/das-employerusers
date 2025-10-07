using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

    namespace SFA.DAS.EmployerProfiles.Api.ApiRequests;

public class UserProfileRequest
{
    [Required]
    public string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [Required]
    public string GovIdentifier { get; set; }
}
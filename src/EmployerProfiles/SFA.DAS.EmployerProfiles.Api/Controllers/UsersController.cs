using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByGovIdentifier;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserById;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/[controller]/")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    [HttpGet]
    [Route("id/{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        try
        {
            UserProfile? userProfile;
            if (Guid.TryParse(id, out var userId))
            {
                var result = await _mediator.Send(new GetUserByIdQuery()
                {
                    Id = userId
                });
                userProfile = result.UserProfile;
            }
            else
            {
                var result = await _mediator.Send(new GetUserByGovIdentifierQuery
                {
                    GovIdentifier = id
                });
                userProfile = result.UserProfile;
            }

            if (userProfile == null)
            {
                return NotFound();
            }

            return Ok(userProfile);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred");
            return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
        }
        
    }
}
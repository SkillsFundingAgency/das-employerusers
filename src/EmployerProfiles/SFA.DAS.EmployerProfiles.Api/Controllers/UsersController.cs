using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EmployerProfiles.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/[controller]/")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    [Route("email/{email}")]
    public async Task<IActionResult> GetUserByEmail([FromRoute] string email)
    {
        return Ok();
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerProfiles.Api.ApiRequests;
using SFA.DAS.EmployerProfiles.Api.ApiResponses;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpsertUser;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByEmail;
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
    [Route("{id}")]
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
            _logger.LogError(e, "GetUserById : An error occurred");
            return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet]
    [Route("query")]
    public async Task<IActionResult> GetUsersByQuery(string email)
    {
        try
        {
            List<UserProfile> users = new List<UserProfile>();
            var result = await _mediator.Send(new GetUsersByEmailQuery()
            {
                Email = email
            });

            return Ok(new UsersQueryResponse { Users = result.UserProfiles });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetUsersByQuery : An error occurred");
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> PutUser([FromRoute]Guid id, UserProfileRequest userProfileRequest)
    {
        try
        {
            var result = await _mediator.Send(new UpsertUserRequest
            {
                Email = userProfileRequest.Email,
                Id = id,
                FirstName = userProfileRequest.FirstName,
                LastName = userProfileRequest.LastName,
                GovUkIdentifier = userProfileRequest.GovIdentifier
            });

            if (result.IsCreated)
            {
                return Created($"{id}",result.UserProfile);
            }
            return Ok(result.UserProfile);
        }
        catch (ValidationException e)
        {
            return BadRequest(e.ValidationResult.ErrorMessage);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "UpsertUser : An error occurred");
            return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
        }
    }
}
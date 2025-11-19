using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerProfiles.Api.ApiRequests;
using SFA.DAS.EmployerProfiles.Api.ApiResponses;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpdateUserSuspended;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpsertUser;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByEmail;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByGovIdentifier;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserById;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUsers;

namespace SFA.DAS.EmployerProfiles.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/[controller]/")]
public class UsersController(IMediator mediator, ILogger<UsersController> logger) : ControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        try
        {
            UserProfile? userProfile;
            if (Guid.TryParse(id, out var userId))
            {
                var result = await mediator.Send(new GetUserByIdQuery
                {
                    Id = userId
                });
                userProfile = result.UserProfile;
            }
            else
            {
                var result = await mediator.Send(new GetUserByGovIdentifierQuery
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
            logger.LogError(e, "GetUserById : An error occurred");
            return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet]
    [Route("query")]
    public async Task<IActionResult> GetUsersByQuery(string email)
    {
        try
        {
            var result = await mediator.Send(new GetUsersByEmailQuery()
            {
                Email = email
            });

            return Ok(new UsersQueryResponse { Users = result.UserProfiles });
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetUsersByQuery : An error occurred");
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers(int pageSize = 1000, int pageNumber = 1)
    {
        try
        {
            var result = await mediator.Send(new GetUsersQuery
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            });

            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetUsers : An error occurred");
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> PutUser([FromRoute]Guid id, UserProfileRequest userProfileRequest)
    {
        try
        {
            var result = await mediator.Send(new UpsertUserRequest
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
            logger.LogError(e, "UpsertUser : An error occurred");
            return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
        }
    }

    [HttpPost]
    [Route("{id}/suspend")]
    public async Task<IActionResult> SuspendUser(string id, [FromBody] ChangeUserStatusRequest request)
    {
        return await ChangeUserSuspensionState(id, request, true);
    }

    [HttpPost]
    [Route("{id}/resume")]
    public async Task<IActionResult> ResumeUser(string id, [FromBody] ChangeUserStatusRequest request)
    {
        return await ChangeUserSuspensionState(id, request, false);
    }

    private async Task<IActionResult> ChangeUserSuspensionState(string identifier, ChangeUserStatusRequest request, bool suspend)
    {
        if (request == null)
        {
            return BadRequest("Request body is required.");
        }

        try
        {
            var userProfile = await GetUserProfile(identifier);

            if (userProfile == null)
            {
                logger.LogInformation("User {Identifier} not found while attempting to update suspension", identifier);
                return NotFound();
            }

            if (suspend && userProfile.IsSuspended)
            {
                return BadRequest(ChangeUserStatusResponse.Failure(userProfile.Id, "Suspended - only active user accounts can be suspended"));
            }

            if (!suspend && !userProfile.IsSuspended)
            {
                return BadRequest(ChangeUserStatusResponse.Failure(userProfile.Id, "Active - only suspended accounts can be reinstated"));
            }

            logger.LogInformation("User {UserId} suspension status set to {Suspended} by {ChangedByUserId}",
                userProfile.Id, suspend, request.ChangedByUserId);

            var result = await mediator.Send(new UpdateUserSuspendedRequest
            {
                Id = userProfile.Id,
                UserSuspended = suspend
            });

            if (!result.Updated)
            {
                return NotFound();
            }

            return Ok(ChangeUserStatusResponse.Success(userProfile.Id));
        }
        catch (Exception e)
        {
            logger.LogError(e, "ChangeUserSuspensionState : An error occurred for identifier {Identifier}", identifier);
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    private async Task<UserProfile?> GetUserProfile(string identifier)
    {
        if (Guid.TryParse(identifier, out var userId))
        {
            var result = await mediator.Send(new GetUserByIdQuery
            {
                Id = userId
            });

            return result.UserProfile;
        }

        var govResult = await mediator.Send(new GetUserByGovIdentifierQuery
        {
            GovIdentifier = identifier
        });

        return govResult.UserProfile;
    }
}
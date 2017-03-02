using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.ForcePasswordReset;
using SFA.DAS.EmployerUsers.Application.Commands.UpdateUser;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Application.Queries.GetUsers;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Api.Orchestrators
{
    public class UserOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public UserOrchestrator(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        protected UserOrchestrator()
        {
            // For testing
        }

        public async Task<OrchestratorResponse<PagedApiResponseViewModel<UserSummaryViewModel>>> UsersIndex(int pageSize, int pageNumber)
        {
            _logger.Info("Getting all user accounts.");
            var response = await _mediator.SendAsync(new GetUsersQuery { PageSize = pageSize, PageNumber = pageNumber });

            return new OrchestratorResponse<PagedApiResponseViewModel<UserSummaryViewModel>>
            {
                Data = new PagedApiResponseViewModel<UserSummaryViewModel>()
                {
                    Data = response.Users.Select(ConvertUserToUserSummaryViewModel).ToList(),
                    Page = pageNumber,
                    TotalPages = (response.RecordCount / pageSize) + 1
                }
            };
        }

        public async Task<OrchestratorResponse<UserViewModel>> UserShow(string id)
        {
            _logger.Info($"Getting user account {id}.");
            var user = await _mediator.SendAsync(new GetUserByIdQuery() { UserId = id });
            return new OrchestratorResponse<UserViewModel>()
            {
                Data = ConvertUserToUserViewModel(user)
            };
        }

        public virtual async Task<OrchestratorResponse> UpdateUser(string id, PatchUserViewModel patch)
        {
            try
            {
                _logger.Info($"Updating user account {id}");

                if (patch.RequiresPasswordReset.HasValue && patch.RequiresPasswordReset.Value)
                {
                    await _mediator.SendAsync(new ForcePasswordResetCommand { UserId = id });
                }
                else // When more properties are patchable then this should change
                {
                    var user = await _mediator.SendAsync(new GetUserByIdQuery { UserId = id });
                    if (patch.RequiresPasswordReset.HasValue && !patch.RequiresPasswordReset.Value)
                    {
                        user.RequiresPasswordReset = patch.RequiresPasswordReset.Value;
                    }

                    await _mediator.SendAsync(new UpdateUserCommand { User = user });
                }
                return new OrchestratorResponse { Status = HttpStatusCode.Accepted };
            }
            catch (InvalidRequestException ex)
            {
                return new OrchestratorResponse { Status = HttpStatusCode.BadRequest, Exception = ex };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error updating user account {id}: {ex.Message}");
                return new OrchestratorResponse { Status = HttpStatusCode.InternalServerError, Exception = ex };
            }
        }



        private UserViewModel ConvertUserToUserViewModel(User user)
        {
            if (user == null)
            {
                return null;
            }

            return new UserViewModel
            {
                LastName = user.LastName,
                FirstName = user.FirstName,
                Id = user.Id,
                Email = user.Email,
                IsActive = user.IsActive,
                IsLocked = user.IsLocked,
                FailedLoginAttempts = user.FailedLoginAttempts
            };
        }

        private UserSummaryViewModel ConvertUserToUserSummaryViewModel(User user)
        {
            return new UserSummaryViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}
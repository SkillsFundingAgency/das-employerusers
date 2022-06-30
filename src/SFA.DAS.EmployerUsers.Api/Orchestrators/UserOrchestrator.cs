using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using NLog;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application.Commands.ResumeUser;
using SFA.DAS.EmployerUsers.Application.Commands.SuspendUser;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserByEmailAddress;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Application.Queries.GetUsers;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Api.Orchestrators
{
    public class UserOrchestrator
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public UserOrchestrator(IMapper mapper, IMediator mediator, ILogger logger)
        {
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }
        
        public async Task<OrchestratorResponse<PagedApiResponseViewModel<UserSummaryViewModel>>> UsersIndex(int pageSize, int pageNumber)
        {
            _logger.Info("Getting all user accounts.");
            var response = await _mediator.SendAsync(new GetUsersQuery { PageSize = pageSize, PageNumber = pageNumber });

            return new OrchestratorResponse<PagedApiResponseViewModel<UserSummaryViewModel>>
            {
                Data = new PagedApiResponseViewModel<UserSummaryViewModel>()
                {
                    Data = response.Users.Select(_mapper.Map<UserSummaryViewModel>).ToList(),
                    Page = pageNumber,
                    TotalPages = (response.RecordCount / pageSize) + 1
                }
            };
        }

        public async Task<OrchestratorResponse<UserViewModel>> UserShow(string id)
        {
            _logger.Info($"Getting user account {id}.");
            var user = await _mediator.SendAsync(new GetUserByIdQuery { UserId = id });
            return new OrchestratorResponse<UserViewModel>
            {
                Data = _mapper.Map<UserViewModel>(user)
            }; 
        }

        public async Task<OrchestratorResponse<UserViewModel>> UserByEmail(string emailAddress)
        {
            _logger.Info($"Getting user account for email address {emailAddress}.");
            var user = await _mediator.SendAsync(new GetUserByEmailAddressQuery { EmailAddress = emailAddress });
            return new OrchestratorResponse<UserViewModel>
            {
                Data = _mapper.Map<UserViewModel>(user)
            };
        }

        public async Task<SuspendUserResponse> Suspend(string id, ChangedByUserInfo changedByUserInfo)
        {
            var user = await _mediator.SendAsync(new GetUserByIdQuery { UserId = id });

            if (user == null)
            {
                return new SuspendUserResponse();
            }

            _logger.Info($"Suspending user account with Id {id}.");

            await _mediator.SendAsync(new SuspendUserCommand(new User { Id = id, Email = user.Email }, changedByUserInfo));

            return new SuspendUserResponse
            {
                Id = id
            };
        }

        public async Task<ResumeUserResponse> Resume(string id, ChangedByUserInfo changedByUserInfo)
        {
            var user = await _mediator.SendAsync(new GetUserByIdQuery { UserId = id });

            if (user == null)
            {
                return new ResumeUserResponse();
            }

            _logger.Info($"Resuming user account with Id {id}.");

            await _mediator.SendAsync(new ResumeUserCommand(new User { Id = id, Email = user.Email }, changedByUserInfo));

            return new ResumeUserResponse
            {
                Id = id
            };
        }
    }
}
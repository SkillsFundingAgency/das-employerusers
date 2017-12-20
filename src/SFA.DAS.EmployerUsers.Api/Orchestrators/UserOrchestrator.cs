using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using NLog;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Application.Queries.GetUsers;

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
            var user = await _mediator.SendAsync(new GetUserByIdQuery() {UserId = id});
            return new OrchestratorResponse<UserViewModel>()
            {
                Data = _mapper.Map<UserViewModel>(user)
            }; 
        }
    }
}
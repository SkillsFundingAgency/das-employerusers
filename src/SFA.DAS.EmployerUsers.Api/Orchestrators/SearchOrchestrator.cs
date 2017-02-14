using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application.Queries.SearchUsers;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Api.Orchestrators
{
    public class SearchOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public SearchOrchestrator(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        public async Task<OrchestratorResponse<PagedApiResponseViewModel<UserSummaryViewModel>>> UserSearch(string criteria, int pageSize, int pageNumber)
        {
            _logger.Info($"Searching for user accounts using criteria {criteria}.");
            var response = await _mediator.SendAsync(new SearchUsersQuery { Criteria = criteria, PageSize = pageSize, PageNumber = pageNumber });

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
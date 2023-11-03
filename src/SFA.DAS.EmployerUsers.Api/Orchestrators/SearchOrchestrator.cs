using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using NLog;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Application.Queries.SearchUsers;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Api.Orchestrators
{
    public class SearchOrchestrator
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public SearchOrchestrator(IMapper mapper, IMediator mediator, ILogger logger)
        {
            _mapper = mapper;
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
                    Data = response.Users.Select(_mapper.Map<UserSummaryViewModel>).ToList(),
                    Page = pageNumber,
                    TotalPages = GetNoOfTotalPages(pageSize, response)
                }
            };
        }

        private static int GetNoOfTotalPages(int pageSize, SearchUsersQueryResponse response)
        {
            if (pageSize >= response.RecordCount)
            {
                return 1;
            }
            
            var remainingResultCountForFinalPage = response.RecordCount % pageSize;
            var totalPages = response.RecordCount / pageSize;
            return remainingResultCountForFinalPage > 0 ? totalPages + 1 : totalPages;
        }
    }
}
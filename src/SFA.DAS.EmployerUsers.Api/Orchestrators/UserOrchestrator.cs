using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MediatR;
using NLog;
using SFA.DAS.EmployerUsers.Api.Types;
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

   

        public async Task<OrchestratorResponse<PagedApiResponseViewModel<User>>> UsersIndex(int pageSize, int pageNumber)
        {
            _logger.Info("Getting all user accounts.");
            var response = await _mediator.SendAsync(new GetUsersQuery { PageSize = pageSize, PageNumber = pageNumber });
      
            return new OrchestratorResponse<PagedApiResponseViewModel<User>>() {Data = new PagedApiResponseViewModel<User>()
            {
                Data = response.Users.ToList(), Page = pageNumber, TotalPages = (response.RecordCount / pageSize) + 1
            } };
        }

        public async Task<OrchestratorResponse<User>> UserShow(string id)
        {
            _logger.Info($"Getting user account {id}.");
            var user = await _mediator.SendAsync(new GetUserByIdQuery() {UserId = id});
            return new OrchestratorResponse<User>()
            {
                Data = user
            }; 
        }
    }
}
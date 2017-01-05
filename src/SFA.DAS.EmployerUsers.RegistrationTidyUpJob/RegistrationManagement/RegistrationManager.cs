using System.Threading.Tasks;
using MediatR;
using NLog;

namespace SFA.DAS.EmployerUsers.RegistrationTidyUpJob.RegistrationManagement
{
    public class RegistrationManager
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public RegistrationManager(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public Task RemoveExpiredRegistrations()
        {
            // Get Users that have expired registrations
            // Delete each user in above set
            return Task.FromResult<object>(null);
        }
    }
}

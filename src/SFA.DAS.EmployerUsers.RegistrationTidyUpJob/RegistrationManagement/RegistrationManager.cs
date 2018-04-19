using MediatR;
using Microsoft.Azure;
using NLog;
using SFA.DAS.EmployerUsers.Application.Commands.DeleteUser;
using SFA.DAS.EmployerUsers.Application.Queries.GetUsersWithExpiredRegistrations;
using System;
using System.Threading.Tasks;

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

        public async Task RemoveExpiredRegistrations()
        {
            _logger.Info("Starting deletion of expired registrations");

            _logger.Info($"AuditApiBaseUrl: {CloudConfigurationManager.GetSetting("AuditApiBaseUrl")}");

            try
            {
                var users = await _mediator.SendAsync(new GetUsersWithExpiredRegistrationsQuery());
                _logger.Info($"Found {users.Length} users with expired registrations");

                foreach (var user in users)
                {
                    _logger.Info($"Deleting user with id: '{user.Id}')");
                    await _mediator.SendAsync(new DeleteUserCommand
                    {
                        User = user
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }

            _logger.Info("Finished deletion of expired registrations");
        }
    }
}

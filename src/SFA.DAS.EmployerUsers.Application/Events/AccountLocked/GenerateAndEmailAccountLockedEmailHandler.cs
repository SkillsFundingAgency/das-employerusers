using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.CodeGenerator;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.Events.AccountLocked
{
    public class GenerateAndEmailAccountLockedEmailHandler : IAsyncNotificationHandler<AccountLockedEvent>
    {
        private readonly IConfigurationService _configurationService;
        private readonly IUserRepository _userRepository;
        private readonly ICodeGenerator _codeGenerator;
        private readonly ICommunicationService _communicationService;

        public GenerateAndEmailAccountLockedEmailHandler(
            IConfigurationService configurationService,
            IUserRepository userRepository,
            ICodeGenerator codeGenerator,
            ICommunicationService communicationService)
        {
            _configurationService = configurationService;
            _userRepository = userRepository;
            _codeGenerator = codeGenerator;
            _communicationService = communicationService;
        }

        public async Task Handle(AccountLockedEvent notification)
        {
            var user = await _userRepository.GetById(notification.User.Id);
            if ((user.UnlockCodeExpiry < DateTime.UtcNow && !string.IsNullOrEmpty(user.UnlockCode)) || string.IsNullOrEmpty(user.UnlockCode))
            {
                user.UnlockCode = await GenerateCode();
                user.UnlockCodeExpiry = DateTime.Now.AddDays(1);
                await _userRepository.Update(user);

                await _communicationService.SendAccountLockedMessage(user, Guid.NewGuid().ToString());
            }
        }

        private async Task<string> GenerateCode()
        {
            var codeLength = (await GetConfig())?.UnlockCodeLength ?? 6;
            return _codeGenerator.GenerateAlphaNumeric(codeLength);
        }
        private async Task<AccountConfiguration> GetConfig()
        {
            return (await _configurationService.GetAsync<EmployerUsersConfiguration>())?.Account;
        }
    }
}

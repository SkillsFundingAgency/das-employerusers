﻿using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Registration;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.Events.AccountLocked
{
    public class GenerateAndEmailAccountLockedEmailHandler : IAsyncNotificationHandler<AccountLockedEvent>
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly ICodeGenerator _codeGenerator;
        private readonly ICommunicationService _communicationService;
        private readonly IAuditService _auditService;
        private readonly EmployerUsersConfiguration _employerUsersConfiguration;

        public GenerateAndEmailAccountLockedEmailHandler(
            IUserRepository userRepository, 
            ICodeGenerator codeGenerator, 
            ICommunicationService communicationService, 
            IAuditService auditService,
            EmployerUsersConfiguration employerUsersConfiguration,
            ILogger logger)
        {
            _userRepository = userRepository;
            _codeGenerator = codeGenerator;
            _communicationService = communicationService;
            _auditService = auditService;
            _employerUsersConfiguration = employerUsersConfiguration;
            _logger = logger;
        }

        public async Task Handle(AccountLockedEvent notification)
        {
            if (notification.User == null)
            {
                _logger.Warn($"AccountLockedEvent: User was not set");

                return;
            }

            _logger.Debug($"Handling AccountLockedEvent for user (id: {notification.User?.Id})");

            var user = !string.IsNullOrEmpty(notification.User.Id)
                            ? await _userRepository.GetById(notification.User.Id)
                            : await _userRepository.GetByEmailAddress(notification.User.Email);

            if (user == null)
            {
                _logger.Debug($"Handling AccountLockedEvent for user '{notification.User?.Email}' (id: {notification.User?.Id})");

                return;
            }

            var sendNotification = false;

            var unlockCode = user.SecurityCodes?.OrderByDescending(sc => sc.ExpiryTime)
                                                .FirstOrDefault(sc => sc.CodeType == Domain.SecurityCodeType.UnlockCode);

            var useStaticCodeGenerator = ConfigurationManager.AppSettings["UseStaticCodeGenerator"].Equals("false", StringComparison.CurrentCultureIgnoreCase);


            if (unlockCode == null)
            {
                _logger.Warn($"Could not generate new unlock code for null unlock code");
            }

            if (unlockCode != null && unlockCode.ExpiryTime >= DateTime.UtcNow)
            {
                _logger.Warn($"Could not generate new unlock code for un-expired code");
            }

            if (unlockCode != null && unlockCode.ExpiryTime < DateTime.UtcNow && useStaticCodeGenerator)
            {
                _logger.Warn($"Could not generate new unlock code: UseStaticCodeGenerator not equal to False");
            }

            if (unlockCode == null || (unlockCode.ExpiryTime < DateTime.UtcNow)
                && useStaticCodeGenerator)
                {
                unlockCode = new Domain.SecurityCode
                {
                    Code = GenerateCode(),
                    CodeType = Domain.SecurityCodeType.UnlockCode,
                    ExpiryTime = DateTime.UtcNow.AddDays(1),
                    ReturnUrl = notification?.ReturnUrl ?? unlockCode?.ReturnUrl ?? _employerUsersConfiguration?.EmployerAccountsBaseUrl
                };
                user.AddSecurityCode(unlockCode);
                await _userRepository.Update(user);

                _logger.Debug($"Generated new unlock code of '{unlockCode.Code}' for user '{user.Id}'");
                sendNotification = true;
            }

            if (notification.ResendUnlockCode || sendNotification)
            {
                await _communicationService.SendAccountLockedMessage(user, Guid.NewGuid().ToString());

                await _auditService.WriteAudit(new SendUnlockCodeAuditMessage(user, unlockCode));
            }
        }      

        private string GenerateCode()
        {
            var codeLength = _employerUsersConfiguration?.Account?.UnlockCodeLength ?? 6;
            return _codeGenerator.GenerateAlphaNumeric(codeLength);
        }      
    }
}

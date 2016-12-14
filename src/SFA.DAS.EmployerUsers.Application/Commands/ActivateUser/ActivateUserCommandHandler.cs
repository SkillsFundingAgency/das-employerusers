using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.ActivateUser
{
    public class ActivateUserCommandHandler : IAsyncRequestHandler<ActivateUserCommand, ActivateUserCommandResult>
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IValidator<ActivateUserCommand> _activateUserCommandValidator;
        private readonly IUserRepository _userRepository;
        private readonly ICommunicationService _communicationService;


        public ActivateUserCommandHandler(IValidator<ActivateUserCommand> activateUserCommandValidator, IUserRepository userRepository, ICommunicationService communicationService)
        {
            _activateUserCommandValidator = activateUserCommandValidator;
            _userRepository = userRepository;
            _communicationService = communicationService;
        }

        public async Task<ActivateUserCommandResult> Handle(ActivateUserCommand message)
        {
            Logger.Debug($"Received ActivateUserCommand for userId '{message.UserId}', Email Address '{message.Email}' with access code '{message.AccessCode}'");

            var user = (!string.IsNullOrEmpty(message.Email) && string.IsNullOrEmpty(message.UserId) && string.IsNullOrEmpty(message.AccessCode))
                            ? await _userRepository.GetByEmailAddress(message.Email)
                            : await _userRepository.GetById(message.UserId);

            message.User = user;
            var validationResult = await _activateUserCommandValidator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }


            var securityCode = message.User.SecurityCodes?.SingleOrDefault(sc => sc.Code == message.AccessCode
                                                                    && sc.CodeType == Domain.SecurityCodeType.AccessCode);
            var result = new ActivateUserCommandResult
            {
                ReturnUrl = securityCode?.ReturnUrl
            };


            if (user.IsActive)
            {
                return result;
            }

            user.IsActive = true;
            user.ExpireSecurityCodesOfType(Domain.SecurityCodeType.AccessCode);
            await _userRepository.Update(user);
            
            return result;
        }
    }
}

using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Login;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Domain.Links;
using SFA.DAS.HashingService;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerUsers.Application.Commands.RequestPasswordResetCode
{
    public class RequestPasswordResetCodeCommandHandler : AsyncRequestHandler<RequestPasswordResetCodeCommand>
    {
        private readonly ILogger _logger;
        private readonly IAuditService _auditService;
        private readonly IHashingService _hashingService;
        private readonly IValidator<RequestPasswordResetCodeCommand> _validator;
        private readonly IUserRepository _userRepository;
        private readonly ICommunicationService _communicationService;
        private readonly ICodeGenerator _codeGenerator;
        private readonly ILinkBuilder _linkBuilder;


        public RequestPasswordResetCodeCommandHandler(IValidator<RequestPasswordResetCodeCommand> validator, IUserRepository userRepository, ICommunicationService communicationService, ICodeGenerator codeGenerator, ILinkBuilder linkBuilder, ILogger logger, IAuditService auditService, IHashingService hashingService)
        {
            _validator = validator;
            _userRepository = userRepository;
            _communicationService = communicationService;
            _codeGenerator = codeGenerator;
            _linkBuilder = linkBuilder;
            _logger = logger;
            _auditService = auditService;
            _hashingService = hashingService;
        }

        protected override async Task HandleCore(RequestPasswordResetCodeCommand message)
        {
            _logger.Debug($"Received RequestPasswordResetCodeCommand for user '{message.Email}'");

            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var existingUser = await _userRepository.GetByEmailAddress(message.Email);

            if (existingUser == null)
            {
                _logger.Info($"Request to reset email for unknown email address : '{message.Email}'");
                return;
            }

            if (RequiresPasswordResetCode(existingUser))
            {
                if (ConfigurationManager.AppSettings["UseStaticCodeGenerator"].Equals("false", StringComparison.CurrentCultureIgnoreCase))
                {
                    AddUserSecurityCode(existingUser, message.ReturnUrl);
                }
                else
                {
                    // for static code generator simply set failed attempts back to zero as creating another same security code breaks the unique constraint
                    var firstSecurityCode = existingUser
                        .SecurityCodes
                        .Where(sc => sc.CodeType == SecurityCodeType.PasswordResetCode)
                        .OrderByDescending(sc => sc.ExpiryTime)
                        .FirstOrDefault();

                    if(firstSecurityCode == null)
                    {
                        AddUserSecurityCode(existingUser, message.ReturnUrl);
                    }
                    else
                    {
                        firstSecurityCode.FailedAttempts = 0;
                    }
                }

                await _userRepository.Update(existingUser);
            }

            await _auditService.WriteAudit(new PasswordResetCodeAuditMessage(existingUser));

            await _communicationService.SendPasswordResetCodeMessage(existingUser, Guid.NewGuid().ToString(), _linkBuilder.GetForgottenPasswordUrl(_hashingService.HashValue(Guid.Parse(existingUser.Id))));
        }

        private static bool RequiresPasswordResetCode(User user)
        {
            if (!user.SecurityCodes.Any())
            {
                return true;
            }

            return user.SecurityCodes
                .Where(sc => sc.CodeType == SecurityCodeType.PasswordResetCode)
                .All(sc => sc.ExpiryTime <= DateTime.UtcNow || sc.FailedAttempts >= 3);
        }

        private void AddUserSecurityCode(User existingUser, string returnUrl)
        {
            existingUser.AddSecurityCode(new SecurityCode
            {
                Code = _codeGenerator.GenerateAlphaNumeric(),
                CodeType = SecurityCodeType.PasswordResetCode,
                ExpiryTime = DateTimeProvider.Current.UtcNow.AddDays(1),
                ReturnUrl = returnUrl
            });
        }
    }
}
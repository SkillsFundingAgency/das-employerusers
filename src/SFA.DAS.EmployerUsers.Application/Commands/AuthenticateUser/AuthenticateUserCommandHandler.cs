using MediatR;
using NLog;
using SFA.DAS.EmployerUsers.Application.Events.AccountLocked;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Login;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Application.Commands.AuthenticateUser
{
    public class AuthenticateUserCommandHandler : IAsyncRequestHandler<AuthenticateUserCommand, User>
    {
        private readonly ILogger _logger;
        private readonly IValidator<AuthenticateUserCommand> _validator;
        private readonly IAuditService _auditService;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly EmployerUsersConfiguration _configuration;
        private readonly IMediator _mediator;

        public AuthenticateUserCommandHandler(IUserRepository userRepository,
                                              IPasswordService passwordService,
                                              EmployerUsersConfiguration configuration,
                                              IMediator mediator,
                                              ILogger logger,
                                              IValidator<AuthenticateUserCommand> validator,
                                              IAuditService auditService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _configuration = configuration;
            _mediator = mediator;
            _logger = logger;
            _validator = validator;
            _auditService = auditService;
        }

        public async Task<User> Handle(AuthenticateUserCommand message)
        {
            _logger.Debug($"Received AuthenticateUserCommand for user '{message.EmailAddress}'");

            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var user = await _userRepository.GetByEmailAddress(message.EmailAddress);
            if (user == null)
            {
                _logger.Debug($"AuthenticateUserCommandHandler Could not find user '{message.EmailAddress}'");
                return null;
            }

            if (user.IsLocked)
            {
                throw new AccountLockedException(user);
            }

            if (user.IsSuspended)
            {
                throw new AccountSuspendedException();
            }

            var isPasswordCorrect = await _passwordService.VerifyAsync(message.Password, user.Password, user.Salt, user.PasswordProfileId);
            if (!isPasswordCorrect)
            {
                await ProcessFailedLogin(user, message.ReturnUrl);
                return null;
            }

            if (user.FailedLoginAttempts > 0)
            {
                user.FailedLoginAttempts = 0;
                await _userRepository.Update(user);
            }
            await _auditService.WriteAudit(new SuccessfulLoginAuditMessage(user));

            return user;
        }

        private async Task ProcessFailedLogin(User user, string returnUrl)
        {
            var config = _configuration.Account;

            user.FailedLoginAttempts++;
            await _auditService.WriteAudit(new FailedLoginAuditMessage(user.Email, user));

            if (user.FailedLoginAttempts >= config.AllowedFailedLoginAttempts)
            {
                _logger.Info($"Locking user (id: {user.Id})");
                user.IsLocked = true;
                await _auditService.WriteAudit(new AccountLockedAuditMessage(user));
            }
            await _userRepository.Update(user);

            if (user.IsLocked)
            {
                _logger.Debug($"Publishing event for user (id: {user.Id}) being locked");
                await _mediator.PublishAsync(new AccountLockedEvent { ReturnUrl = returnUrl, User = user });
                throw new AccountLockedException(user);
            }
        }
    }
}

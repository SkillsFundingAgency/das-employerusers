using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Application.Events.AccountLocked;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.Commands.AuthenticateUser
{
    public class AuthenticateUserCommandHandler : IAsyncRequestHandler<AuthenticateUserCommand, User>
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IConfigurationService _configurationService;
        private readonly IMediator _mediator;

        public AuthenticateUserCommandHandler(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IConfigurationService configurationService,
            IMediator mediator)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _configurationService = configurationService;
            _mediator = mediator;
        }

        public async Task<User> Handle(AuthenticateUserCommand message)
        {
            Logger.Debug($"Received AuthenticateUserCommand for user '{message.EmailAddress}'");

            var user = await _userRepository.GetByEmailAddress(message.EmailAddress);
            if (user == null)
            {
                return null;
            }

            if (user.IsLocked)
            {
                throw new AccountLockedException(user);
            }

            var isPasswordCorrect = await _passwordService.VerifyAsync(message.Password, user.Password, user.Salt, user.PasswordProfileId);
            if (!isPasswordCorrect)
            {
                await ProcessFailedLogin(user);
                return null;
            }

            if (user.FailedLoginAttempts > 0)
            {
                user.FailedLoginAttempts = 0;
                await _userRepository.Update(user);
            }

            return user;
        }

        private async Task ProcessFailedLogin(User user)
        {
            var config = await GetAccountConfiguration();

            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= config.AllowedFailedLoginAttempts)
            {
                Logger.Info($"Locking user '{user.Email}' (id: {user.Id})");
                user.IsLocked = true;
            }
            await _userRepository.Update(user);

            if (user.IsLocked)
            {
                Logger.Debug($"Publishing event for user '{user.Email}' (id: {user.Id}) being locked");
                await _mediator.PublishAsync(new AccountLockedEvent { User = user });
                throw new AccountLockedException(user);
            }
        }
        private async Task<AccountConfiguration> GetAccountConfiguration()
        {
            return (await _configurationService.GetAsync<EmployerUsersConfiguration>())?.Account;
        }
    }
}

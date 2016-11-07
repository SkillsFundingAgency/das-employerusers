using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;
using System.Collections.Generic;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerUsers.Application.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : AsyncRequestHandler<RegisterUserCommand>
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IUserRepository _userRepository;
        private readonly ICommunicationService _communicationService;
        private readonly ICodeGenerator _codeGenerator;
        private readonly IValidator<RegisterUserCommand> _registerUserCommandValidator;
        private readonly IPasswordService _passwordService;

        public RegisterUserCommandHandler(IValidator<RegisterUserCommand> registerUserCommandValidator,
                                          IPasswordService passwordService,
                                          IUserRepository userRepository,
                                          ICommunicationService communicationService,
                                          ICodeGenerator codeGenerator)
        {
            _userRepository = userRepository;
            _communicationService = communicationService;
            _codeGenerator = codeGenerator;
            _registerUserCommandValidator = registerUserCommandValidator;
            _passwordService = passwordService;
        }

        protected override async Task HandleCore(RegisterUserCommand message)
        {
            Logger.Debug($"Received RegisterUserCommand for user '{message.Email}'");

            var validationResult = _registerUserCommandValidator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var existingUser = await _userRepository.GetByEmailAddress(message.Email);

            if (existingUser != null && existingUser.IsActive)
            {
                throw new InvalidRequestException(new Dictionary<string, string>
                {
                    {nameof(message.Email), "Your email address has already been activated. Please try signing in again. If you've forgotten your password you can reset it." }
                });
            }

            var securedPassword = await _passwordService.GenerateAsync(message.Password);

            if (existingUser == null)
            {
                var registerUser = Create(message, securedPassword);

                await AddSecurityCode(registerUser);
                await _userRepository.Create(registerUser);
                await SendUserRegistrationMessage(registerUser);
            }
            else
            {
                Update(existingUser, message, securedPassword);

                await _userRepository.Update(existingUser);
                await SendUserRegistrationMessage(existingUser);
            }
        }

        private async Task SendUserRegistrationMessage(User user)
        {
            await _communicationService.SendUserRegistrationMessage(user, Guid.NewGuid().ToString());
        }

        private void Update(User user, RegisterUserCommand message, SecuredPassword securedPassword)
        {
            user.FirstName = message.FirstName;
            user.LastName = message.LastName;
            user.Password = securedPassword.HashedPassword;
            user.Salt = securedPassword.Salt;
            user.PasswordProfileId = securedPassword.ProfileId;
        }

        private User Create(RegisterUserCommand message, SecuredPassword securedPassword)
        {

            var user = new User
            {
                Id = message.Id,
                Email = message.Email
            };

            Update(user, message, securedPassword);

            return user;
        }

        private async Task AddSecurityCode(User user)
        {
            var securityCode = new SecurityCode
            {
                Code = _codeGenerator.GenerateAlphaNumeric(),
                CodeType = SecurityCodeType.AccessCode,
                ExpiryTime = DateTimeProvider.Current.UtcNow.AddMinutes(30) //TODO: Make time configurable
            };
            await _userRepository.StoreSecurityCode(user, securityCode.Code, securityCode.CodeType, securityCode.ExpiryTime);

            if (user.SecurityCodes == null)
            {
                user.SecurityCodes = new[]
                {
                    securityCode
                };
            }
            else
            {
                user.SecurityCodes = user.SecurityCodes.Concat(new[] { securityCode }).ToArray();
            }
        }
    }

}
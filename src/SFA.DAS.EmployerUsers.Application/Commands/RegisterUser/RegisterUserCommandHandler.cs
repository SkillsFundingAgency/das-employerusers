using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : AsyncRequestHandler<RegisterUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICommunicationService _communicationService;
        private readonly ICodeGenerator _codeGenerator;
        private readonly IValidator<RegisterUserCommand> _registerUserCommandValidator;
        private readonly IPasswordService _passwordService;

        public RegisterUserCommandHandler(IValidator<RegisterUserCommand> registerUserCommandValidator, IPasswordService passwordService, IUserRepository userRepository, ICommunicationService communicationService, ICodeGenerator codeGenerator)
        {
            _userRepository = userRepository;
            _communicationService = communicationService;
            _codeGenerator = codeGenerator;
            _registerUserCommandValidator = registerUserCommandValidator;
            _passwordService = passwordService;
        }

        protected override async Task HandleCore(RegisterUserCommand message)
        {
            if (!_registerUserCommandValidator.Validate(message))
            {
                throw new InvalidRequestException(new[] { "NotValid" });
            }

            var securedPassword = await _passwordService.GenerateAsync(message.Password);

            var accessCode = _codeGenerator.GenerateAlphaNumeric();
            var registerUser = new User
            {
                Id = message.Id,
                Email = message.Email,
                FirstName = message.FirstName,
                LastName = message.LastName,
                AccessCode = accessCode,
                Password = securedPassword.HashedPassword,
                Salt = securedPassword.Salt,
                PasswordProfileId = securedPassword.ProfileId
            };

            await _userRepository.Create(registerUser);
            await _communicationService.SendUserRegistrationMessage(registerUser, Guid.NewGuid().ToString());



        }
    }
    
}
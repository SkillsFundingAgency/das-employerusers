using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : AsyncRequestHandler<RegisterUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<RegisterUserCommand> _registerUserCommandValidator;
        private readonly IPasswordService _passwordService;

        public RegisterUserCommandHandler(IValidator<RegisterUserCommand> registerUserCommandValidator, IPasswordService passwordService, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _registerUserCommandValidator = registerUserCommandValidator;
            _passwordService = passwordService;
        }
        
        protected override async Task HandleCore(RegisterUserCommand message)
        {
            if (!_registerUserCommandValidator.Validate(message))
            {
                throw new InvalidRequestException(new [] {"NotValid"});
            }

            var securedPassword = await _passwordService.GenerateAsync(message.Password);

            var registerUser = new User
            {
                Email = message.Email,
                FirstName = message.FirstName,
                LastName = message.LastName,
                AccessCode = "ABC123ZXY"
                Password = securedPassword.HashedPassword,
                Salt = securedPassword.Salt,
                PasswordProfileId = securedPassword.ProfileId
            };

            await _userRepository.Create(registerUser);
        }
    }
    
}
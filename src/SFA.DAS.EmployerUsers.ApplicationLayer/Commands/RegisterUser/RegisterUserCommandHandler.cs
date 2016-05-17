using MediatR;
using SFA.DAS.EmployerUsers.Data.User;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.ApplicationLayer.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : RequestHandler<RegisterUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<RegisterUserCommand> _registerUserCommandValidator;

        public RegisterUserCommandHandler(IValidator<RegisterUserCommand> registerUserCommandValidator, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _registerUserCommandValidator = registerUserCommandValidator;
        }

        protected override void HandleCore(RegisterUserCommand message)
        {
            if (_registerUserCommandValidator.Validate(message))
            {
                var registerUser = new User
                {
                    Email = message.Email,
                    FirstName = message.FirstName,
                    LastName = message.LastName,
                    Password = message.Password
                };
                _userRepository.Create(registerUser);
            }
        }
    }
}
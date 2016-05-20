using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : AsyncRequestHandler<RegisterUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<RegisterUserCommand> _registerUserCommandValidator;

        public RegisterUserCommandHandler(IValidator<RegisterUserCommand> registerUserCommandValidator, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _registerUserCommandValidator = registerUserCommandValidator;
        }
        
        protected override async Task HandleCore(RegisterUserCommand message)
        {
            if (!_registerUserCommandValidator.Validate(message))
            {
                throw new InvalidRequestException(new [] {"NotValid"});
            }

            var registerUser = new User
            {
                Email = message.Email,
                FirstName = message.FirstName,
                LastName = message.LastName,
                Password = message.Password,
                AccessCode = "ABC123ZXY"
            };

            await _userRepository.Create(registerUser);
        }
    }
    
}
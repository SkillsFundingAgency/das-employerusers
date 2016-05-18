﻿using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Data.User;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.ApplicationLayer.Commands.RegisterUser
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
                Password = message.Password
            };

            await _userRepository.Create(registerUser);
        }
    }
    
}
using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.AuthenticateUser
{
    public class AuthenticateUserCommandHandler : IAsyncRequestHandler<AuthenticateUserCommand, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public AuthenticateUserCommandHandler(IUserRepository userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public async Task<User> Handle(AuthenticateUserCommand message)
        {
            var user = await _userRepository.GetByEmailAddress(message.EmailAddress);
            if (user == null)
            {
                return null;
            }

            var isPasswordCorrect = await _passwordService.VerifyAsync(message.Password, user.Password, user.Salt, user.PasswordProfileId);
            if (!isPasswordCorrect)
            {
                return null;
            }

            return user;
        }
    }
}

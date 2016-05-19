using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.ApplicationLayer.Commands.ActivateUser
{
    public class ActivateUserCommandHandler : AsyncRequestHandler<ActivateUserCommand>
    {
        private readonly IValidator<ActivateUserCommand> _activateUserCommandValidator;
        private readonly IUserRepository _userRepository;


        public ActivateUserCommandHandler(IValidator<ActivateUserCommand> activateUserCommandValidator, IUserRepository userRepository)
        {
            _activateUserCommandValidator = activateUserCommandValidator;
            _userRepository = userRepository;
        }

        protected override async Task HandleCore(ActivateUserCommand message)
        {
            var result = _activateUserCommandValidator.Validate(message);

            if (!result)
            {
                throw new InvalidRequestException(new[] { "NotValid" });
            }

            var user = await _userRepository.GetById(message.UserId);
            user.IsActive = true;

            await _userRepository.Update(user); 
        }
    }
}

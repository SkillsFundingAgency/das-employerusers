using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IAsyncRequestHandler<UpdateUserCommand, UpdateUserCommandResponse>
    {
        private readonly IValidator<UpdateUserCommand> _updateUserCommandValidator;
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandHandler(IValidator<UpdateUserCommand> updateUserCommandValidator, IUserRepository userRepository)
        {
            _updateUserCommandValidator = updateUserCommandValidator;
            _userRepository = userRepository;
        }
        public async Task<UpdateUserCommandResponse> Handle(UpdateUserCommand message)
        {
            var validationResult = await _updateUserCommandValidator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {   
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var user = new User
            {
                GovUkIdentifier = message.GovUkIdentifier,
                Email = message.Email,
                FirstName = message.FirstName,
                LastName = message.LastName
            };
            await _userRepository.UpsertWithGovIdentifier(user);
            
            user = await _userRepository.GetByEmailAddress(message.Email);
            
            return new UpdateUserCommandResponse
            {
                User = user
            };
        }
    }
}
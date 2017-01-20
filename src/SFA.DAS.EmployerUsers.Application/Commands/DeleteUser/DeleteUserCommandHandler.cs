using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IAsyncRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(DeleteUserCommand message)
        {
            await _userRepository.Delete(message.User);

            return Unit.Value;
        }
    }
}
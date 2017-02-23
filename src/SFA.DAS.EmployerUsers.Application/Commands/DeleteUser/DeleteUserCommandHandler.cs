using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Delete;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IAsyncRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;

        public DeleteUserCommandHandler(IUserRepository userRepository, IAuditService auditService)
        {
            _userRepository = userRepository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(DeleteUserCommand message)
        {
            await _userRepository.Delete(message.User);

            await _auditService.WriteAudit(new DeleteUserAuditMessage(message.User));

            return Unit.Value;
        }
    }
}
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Suspend;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.SuspendUser
{
    public class SuspendUserCommandHandler : IAsyncRequestHandler<SuspendUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;

        public SuspendUserCommandHandler(IUserRepository userRepository, IAuditService auditService)
        {
            _userRepository = userRepository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(SuspendUserCommand message)
        {
            await _userRepository.Suspend(message.User);

            await _auditService.WriteAudit(new SuspendUserAuditMessage(message.User));

            return Unit.Value;
        }
    }
}
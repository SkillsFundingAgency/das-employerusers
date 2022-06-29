using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Suspend;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.ResumeUser
{
    public class ResumeUserCommandHandler : IAsyncRequestHandler<ResumeUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;

        public ResumeUserCommandHandler(IUserRepository userRepository, IAuditService auditService)
        {
            _userRepository = userRepository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(ResumeUserCommand message)
        {
            await _userRepository.Resume(message.User);

            await _auditService.WriteAudit(new ResumeUserAuditMessage(message.User, message.ChangedByUserInfo));

            return Unit.Value;
        }
    }
}
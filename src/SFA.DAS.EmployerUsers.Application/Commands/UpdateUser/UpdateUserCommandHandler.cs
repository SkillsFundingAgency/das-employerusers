using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Update;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IAsyncRequestHandler<UpdateUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;

        public UpdateUserCommandHandler(IUserRepository userRepository, IAuditService auditService)
        {
            _userRepository = userRepository;
            _auditService = auditService;
        }

        public async Task<Unit> Handle(UpdateUserCommand message)
        {
            var oldUser = await _userRepository.GetById(message.User.Id);

            await _userRepository.Update(message.User);

            await _auditService.WriteAudit(new UpdateUserAuditMessage(oldUser, message.User));

            return Unit.Value;
        }
    }
}
using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Update;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.ForcePasswordReset
{
    public class ForcePasswordResetCommandHandler : IAsyncRequestHandler<ForcePasswordResetCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;
        private readonly ICommunicationService _communicationService;
        private readonly ICodeGenerator _codeGenerator;

        public ForcePasswordResetCommandHandler(IUserRepository userRepository,
                                                IAuditService auditService,
                                                ICommunicationService communicationService,
                                                ICodeGenerator codeGenerator)
        {
            _userRepository = userRepository;
            _auditService = auditService;
            _communicationService = communicationService;
            _codeGenerator = codeGenerator;
        }

        public async Task<Unit> Handle(ForcePasswordResetCommand message)
        {
            var user = await _userRepository.GetById(message.UserId);
            if (user.RequiresPasswordReset)
            {
                return Unit.Value;
            }

            user.RequiresPasswordReset = true;
            user.AddSecurityCode(new Domain.SecurityCode
            {
                Code = _codeGenerator.GenerateAlphaNumeric(),
                CodeType = Domain.SecurityCodeType.PasswordResetCode,
                ExpiryTime = DateTime.Today.AddDays(8).AddSeconds(-1)
            });
            await _userRepository.Update(user);

            await _communicationService.SendForcePasswordResetMessage(user, Guid.NewGuid().ToString());

            await _auditService.WriteAudit(new ForcePasswordResetAuditMessage(user));

            return Unit.Value;
        }
    }
}
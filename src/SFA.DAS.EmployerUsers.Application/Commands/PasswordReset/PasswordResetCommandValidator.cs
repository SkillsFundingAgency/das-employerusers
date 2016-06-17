using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.PasswordReset
{
    public class PasswordResetCommandValidator : IValidator<PasswordResetCommand>
    {
        public ValidationResult Validate(PasswordResetCommand item)
        {
            throw new NotImplementedException();
        }
    }
}

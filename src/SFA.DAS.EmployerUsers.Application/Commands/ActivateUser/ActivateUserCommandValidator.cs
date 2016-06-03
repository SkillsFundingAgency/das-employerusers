using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerUsers.Application.Commands.ActivateUser
{
    public class ActivateUserCommandValidator : IValidator<ActivateUserCommand>
    {
        public Dictionary<string, string> Validate(ActivateUserCommand item)
        {
            if (string.IsNullOrEmpty(item?.AccessCode) || string.IsNullOrEmpty(item.UserId))
            {
                return new Dictionary<string, string> { { "", "" } };
            }

            if (!item.AccessCode.Equals(item.User.AccessCode, StringComparison.CurrentCultureIgnoreCase))
            {
                return new Dictionary<string, string> { { "", "" } };
            }

            return new Dictionary<string, string>();
        }
    }
}

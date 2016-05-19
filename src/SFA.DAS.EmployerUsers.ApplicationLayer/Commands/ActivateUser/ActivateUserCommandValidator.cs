using System;

namespace SFA.DAS.EmployerUsers.ApplicationLayer.Commands.ActivateUser
{
    public class ActivateUserCommandValidator : IValidator<ActivateUserCommand>
    {
        public bool Validate(ActivateUserCommand item)
        {
            if (string.IsNullOrEmpty(item?.AccessCode) || string.IsNullOrEmpty(item.UserId))
            {
                return false;
            }

            if (!item.AccessCode.Equals(item.User.AccessCode,StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }
}

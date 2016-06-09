using System;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.UnlockUser
{
    public class UnlockUserCommandValidator : IValidator<UnlockUserCommand>
    {
        public ValidationResult Validate(UnlockUserCommand item)
        {
            var result = new ValidationResult();
            if (string.IsNullOrEmpty(item.Email))
            {
                result.ValidationDictionary.Add("Email","Email has not been supplied");
            }
            if (string.IsNullOrEmpty(item.UnlockCode))
            {
                result.ValidationDictionary.Add("UnlockCode","Unlock Code has not been supplied");
            }

            if (item.User == null)
            {
                result.ValidationDictionary.Add("User", "User Does Not Exist");
                return result;
            }

            if (item.User.UnlockCodeExpiry < DateTime.UtcNow )
            {
                result.ValidationDictionary.Add("UnlockCodeExpiry", "Unlock Code has expired");
                return result;
            }


            if(!item.UnlockCode.Equals(item.User.UnlockCode,StringComparison.CurrentCultureIgnoreCase))
            {
                result.ValidationDictionary.Add("UnlockCodeMatch", "Unlock Code is not correct");
            }

            return result;
        }
    }
}
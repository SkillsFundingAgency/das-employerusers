using System.Collections.Generic;

namespace SFA.DAS.EmployerUsers.Application.Commands.ResendActivationCode
{
    public class ResendActivationCodeCommandValidator : IValidator<ResendActivationCodeCommand>
    {
        public Dictionary<string,string> Validate(ResendActivationCodeCommand item)
        {
            var validate = !string.IsNullOrEmpty(item?.UserId);

            return validate ? new Dictionary<string, string>() : new Dictionary<string, string> { { "", "" } };
        }
    }
}
namespace SFA.DAS.EmployerUsers.Application.Commands.ResendActivationCode
{
    public class ResendActivationCodeCommandValidator : IValidator<ResendActivationCodeCommand>
    {
        public bool Validate(ResendActivationCodeCommand item)
        {
            return !string.IsNullOrEmpty(item?.UserId);
        }
    }
}
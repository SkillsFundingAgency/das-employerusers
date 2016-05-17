namespace SFA.DAS.EmployerUsers.ApplicationLayer.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : IValidator<RegisterUserCommand>
    {
        public bool Validate(RegisterUserCommand item)
        {
            return !string.IsNullOrWhiteSpace(item.Email) && !string.IsNullOrWhiteSpace(item.FirstName) && !string.IsNullOrWhiteSpace(item.LastName) && !string.IsNullOrWhiteSpace(item.Password);
        }
    }
}
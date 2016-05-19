namespace SFA.DAS.EmployerUsers.ApplicationLayer.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : IValidator<RegisterUserCommand>
    {
        public bool Validate(RegisterUserCommand item)
        {
            if (string.IsNullOrWhiteSpace(item?.Email) || string.IsNullOrWhiteSpace(item.FirstName) ||
                string.IsNullOrWhiteSpace(item.LastName) || string.IsNullOrWhiteSpace(item.Password) ||
                string.IsNullOrWhiteSpace(item.ConfirmPassword) || string.IsNullOrWhiteSpace(item.ConfirmEmail))
            {
                return false;
            }

            if (!item.ConfirmEmail.ToLower().Equals(item.Email.ToLower()))
            {
                return false;
            }

            if (!item.ConfirmPassword.Equals(item.Password))
            {
                return false;
            }

            return true;
        }
    }
}
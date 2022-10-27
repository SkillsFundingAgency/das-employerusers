using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : IValidator<UpdateUserCommand>
    {
        public Task<ValidationResult> ValidateAsync(UpdateUserCommand item)
        {
            var result = new ValidationResult();
            
            if (string.IsNullOrEmpty(item.Email))
            {
                result.AddError(nameof(item.Email));
            }
            if (string.IsNullOrEmpty(item.GovUkIdentifier))
            {
                result.AddError(nameof(item.GovUkIdentifier));
            }
            
            return Task.FromResult(result);
        }
    }
}
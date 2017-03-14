using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetUserByHashedId
{
    public class GetUserByHashedIdValidator : IValidator<GetUserByHashedIdQuery>
    {
        public Task<ValidationResult> ValidateAsync(GetUserByHashedIdQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedUserId))
            {
                validationResult.AddError(nameof(item.HashedUserId));
            }

            return Task.FromResult(validationResult);
        }
    }
}
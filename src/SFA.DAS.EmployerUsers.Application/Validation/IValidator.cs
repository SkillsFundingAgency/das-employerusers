using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Application.Validation
{
    public interface IValidator<T>
    {
        Task<ValidationResult> ValidateAsync(T item);           
    }
}
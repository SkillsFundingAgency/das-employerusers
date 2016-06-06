namespace SFA.DAS.EmployerUsers.Application.Validation
{
    public interface IValidator<T>
    {
        ValidationResult Validate(T item);           
    }
}
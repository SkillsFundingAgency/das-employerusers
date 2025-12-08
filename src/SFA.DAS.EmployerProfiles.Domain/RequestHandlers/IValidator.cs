namespace SFA.DAS.EmployerProfiles.Domain.RequestHandlers;

public interface IValidator<in T>
{
    Task<ValidationResult> ValidateAsync(T item);
}
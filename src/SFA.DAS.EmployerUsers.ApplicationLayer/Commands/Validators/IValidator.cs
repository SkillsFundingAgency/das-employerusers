namespace SFA.DAS.EmployerUsers.ApplicationLayer.Commands.Validators
{
    public interface IValidator<T>
    {

        bool Validate(T item);
        
    }
}
namespace SFA.DAS.EmployerUsers.ApplicationLayer
{
    public interface IValidator<T>
    {

        bool Validate(T item);
        
    }
}
namespace SFA.DAS.EmployerUsers.Application
{
    public interface IValidator<T>
    {

        bool Validate(T item);
        
    }
}
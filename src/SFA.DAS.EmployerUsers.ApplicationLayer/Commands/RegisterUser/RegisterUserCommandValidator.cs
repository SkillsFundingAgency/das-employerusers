namespace SFA.DAS.EmployerUsers.ApplicationLayer.Commands.RegisterUser
{
    public interface IValidator<T>
    {

        bool Validate(T item);
        
    }
}
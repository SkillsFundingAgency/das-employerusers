using System.Collections.Generic;

namespace SFA.DAS.EmployerUsers.Application
{
    public interface IValidator<T>
    {

        Dictionary<string,string> Validate(T item);
        
    }
}
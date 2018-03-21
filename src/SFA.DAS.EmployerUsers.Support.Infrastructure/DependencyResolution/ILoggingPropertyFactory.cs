using System.Collections.Generic;

namespace SFA.DAS.EmployerUsers.Support.Infrastructure.DependencyResolution
{
    public interface ILoggingPropertyFactory
    {
        IDictionary<string, object> GetProperties();
    }
}
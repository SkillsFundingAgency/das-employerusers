using SFA.DAS.EmployerUsers.RegistrationTidyUpJob.DependencyResolution;
using SFA.DAS.EmployerUsers.RegistrationTidyUpJob.RegistrationManagement;

namespace SFA.DAS.EmployerUsers.RegistrationTidyUpJob
{
    class Program
    {
        static void Main()
        {
            var container = IoC.Initialize();

            var manager = container.GetInstance<RegistrationManager>();
            manager.RemoveExpiredRegistrations().Wait();
        }
    }
}

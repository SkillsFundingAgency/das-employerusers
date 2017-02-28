using SFA.DAS.Audit.Client;
using SFA.DAS.Audit.Types;
using SFA.DAS.EmployerUsers.RegistrationTidyUpJob.DependencyResolution;
using SFA.DAS.EmployerUsers.RegistrationTidyUpJob.RegistrationManagement;

namespace SFA.DAS.EmployerUsers.RegistrationTidyUpJob
{
    class Program
    {
        static void Main()
        {

            AuditMessageFactory.RegisterBuilder(message =>
            {
                message.Source = new Source
                {
                    Component = typeof(Program).Assembly.GetName().Name,
                    System = "EMPU",
                    Version = typeof(Program).Assembly.GetName().Version.ToString()
                };
            });

            var container = IoC.Initialize();

            var manager = container.GetInstance<RegistrationManager>();
            manager.RemoveExpiredRegistrations().Wait();
        }
    }
}

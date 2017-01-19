using MediatR;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer;
using StructureMap;
using StructureMap.Graph;

namespace SFA.DAS.EmployerUsers.RegistrationTidyUpJob.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            For<IUserRepository>().Use<SqlServerUserRepository>();

            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();
        }
    }
}

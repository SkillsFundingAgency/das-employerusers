using System.Diagnostics.CodeAnalysis;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerUsers.Api.Client;
using SFA.DAS.EmployerUsers.Support.Application.Handlers;
using SFA.DAS.EmployerUsers.Support.Infrastructure;
using SFA.DAS.EmployerUsers.Support.Infrastructure.DependencyResolution;
using SFA.DAS.NLog.Logger;
using StructureMap.Configuration.DSL;

namespace SFA.DAS.EmployerUsers.Support.Web.DependencyResolution
{
    [ExcludeFromCodeCoverage]
    public class InfrastructureRegistry : Registry
    {
        public InfrastructureRegistry()
        {
            For<ILoggingPropertyFactory>().Use<LoggingPropertyFactory>();

            For<ILog>().Use(x => new NLogLogger(
                x.ParentType,
                x.GetInstance<IRequestContext>(),
                x.GetInstance<ILoggingPropertyFactory>().GetProperties())).AlwaysUnique();


            For<IEmployerUsersApiClient>().Use("", ctx =>
            {
                var empUserApiSettings = ctx.GetInstance<IEmployerUsersApiConfiguration>();
                return new EmployerUsersApiClient(empUserApiSettings);
            });
            For<IAccountApiClient>().Use<AccountApiClient>();
            For<IEmployerUserRepository>().Use<EmployerUserRepository>();
            For<IEmployerUserHandler>().Use<EmployerUserHandler>();
        }
    }
}
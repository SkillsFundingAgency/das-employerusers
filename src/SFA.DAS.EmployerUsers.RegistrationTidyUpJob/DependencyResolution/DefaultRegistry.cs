using System;
using MediatR;
using Microsoft.Azure;
using SFA.DAS.Audit.Client;
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
            RegisterEnvironmentSpecificInstances();

            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();


        }

        private void RegisterEnvironmentSpecificInstances()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            if (environment.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                For<IAuditApiClient>().Use<StubAuditApiClient>().Ctor<string>().Is(string.Format(@"{0}\App_Data\Audit\", AppDomain.CurrentDomain.BaseDirectory));
            }
            else
            {
                For<IAuditApiClient>().Use<AuditApiClient>();
                For<AuditApiConfiguration>().Use(() => new AuditApiConfiguration
                {
                    ApiBaseUrl = CloudConfigurationManager.GetSetting("AuditApiBaseUrl"),
                    ClientId = CloudConfigurationManager.GetSetting("AuditApiClientId"),
                    ClientSecret = CloudConfigurationManager.GetSetting("AuditApiSecret"),
                    IdentifierUri = CloudConfigurationManager.GetSetting("AuditApiIdentifierUri"),
                    Tenant = CloudConfigurationManager.GetSetting("AuditApiTenant")
                });
            }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Configuration;
using System.Web;
using System.Web.WebPages;
using MediatR;
using Microsoft.Azure;
using SFA.DAS.Audit.Client;
using SFA.DAS.CodeGenerator;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Auditing;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer;
using SFA.DAS.EmployerUsers.Infrastructure.Notification;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;
using StructureMap;
using StructureMap.Web.Pipeline;

namespace SFA.DAS.EmployerUsers.Web.DependencyResolution
{
    using SFA.DAS.HashingService;
    using StructureMap.Graph;
    using System.Threading.Tasks;

    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS.EmployerUsers")
                        && !a.GetName().Name.Equals("SFA.DAS.EmployerUsers.Infrastructure"));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            For<IOwinWrapper>().Transient().Use(() => new OwinWrapper(HttpContext.Current.GetOwinContext())).SetLifecycleTo(new HttpContextLifecycle());

            For<IdentityServerConfiguration>().Transient().Use(() => new IdentityServerConfiguration
            {
                ApplicationBaseUrl = ConfigurationManager.AppSettings["BaseExternalUrl"],
                EmployerPortalUrl = ConfigurationManager.AppSettings["EmployerPortalUrl"]
            });

            For<IAuditMessageFactory>().Use<AuditMessageFactory>().Singleton();
            For<IAuditService>().Use<AuditService>();

            AddConfigSpecifiedRegistrations();

            var environment = GetEnvironment();
            var configService = GetconfigService(environment);
            var employerUserConfig = EmployerUserConfig(configService);

            For<EmployerUsersConfiguration>().Use(employerUserConfig);
            For<IConfigurationService>().Use(configService);
            ConfigureHashingService(employerUserConfig);
            
            AddEnvironmentSpecificRegistrations(environment);
            AddMediatrRegistrations();
        }

        private string GetEnvironment()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = ConfigurationManager.AppSettings["EnvironmentName"];
            }

            return environment;
        }

        private ConfigurationService GetconfigService(string environment)
        {
            IConfigurationRepository configurationRepository;

            if (ConfigurationManager.AppSettings["LocalConfig"].AsBool())
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository = new AzureTableStorageConfigurationRepository(ConfigurationManager.AppSettings["ConfigurationStorageConnectionString"]);
            }

            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions("SFA.DAS.EmployerUsers.Web", environment, "1.0"));

            return configurationService;
        }

        private void AddEnvironmentSpecificRegistrations(string environment)
        {
            if (environment == "LOCAL")
            {
                AddDevelopmentRegistrations();
            }
            else
            {
                AddProductionRegistrations();
            }
        }

        private void AddDevelopmentRegistrations()
        {
            For<IUserRepository>().Use<SqlServerUserRepository>();
            For<IRelyingPartyRepository>().Use<SqlServerRelyingPartyRepository>();
            For<IPasswordProfileRepository>().Use<SqlServerPasswordProfileRepository>();
            For<IAuditApiClient>().Use<StubAuditApiClient>().Ctor<string>().Is(string.Format(@"{0}\App_Data\Audit\", AppDomain.CurrentDomain.BaseDirectory));
        }
        private void AddProductionRegistrations()
        {
            For<IUserRepository>().Use<SqlServerUserRepository>();
            For<IRelyingPartyRepository>().Use<SqlServerRelyingPartyRepository>();
            For<IPasswordProfileRepository>().Use<InMemoryPasswordProfileRepository>();
            For<IAuditApiClient>().Use<AuditApiClient>();
            For<IAuditApiConfiguration>().Use(() => new AuditApiConfiguration
            {
                ApiBaseUrl = ConfigurationManager.AppSettings["AuditApiBaseUrl"],
                ClientId = ConfigurationManager.AppSettings["AuditApiClientId"],
                ClientSecret = ConfigurationManager.AppSettings["AuditApiSecret"],
                IdentifierUri = ConfigurationManager.AppSettings["AuditApiIdentifierUri"],
                Tenant = ConfigurationManager.AppSettings["AuditApiTenant"]
            });
        }

        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();
        }

        private void AddConfigSpecifiedRegistrations()
        {
            var useStaticCodeGenerator = ConfigurationManager.AppSettings["UseStaticCodeGenerator"].Equals("true", StringComparison.CurrentCultureIgnoreCase);
            if (useStaticCodeGenerator)
            {
                For<ICodeGenerator>().Use(new StaticCodeGenerator());
            }
            else
            {
                For<ICodeGenerator>().Use(new RandomCodeGenerator());
            }

            var storeEmailsOnDisk = ConfigurationManager.AppSettings["StoreEmailsOnDisk"].Equals("true", StringComparison.CurrentCultureIgnoreCase);
            if (storeEmailsOnDisk)
            {
                For<INotificationsApi>().Use<StubNotificationsApi>();
            }
            else
            {
                For<INotificationsApiClientConfiguration>().Use<NotificationsApiConfiguration>();
                For<INotificationsApi>().Use<NotificationsApi>();
            }
        }

        private EmployerUsersConfiguration EmployerUserConfig(ConfigurationService configurationService)
        {
            return configurationService.Get<EmployerUsersConfiguration>();
        }

        private void ConfigureHashingService(EmployerUsersConfiguration config)
        {
            For<IHashingService>().Use(x => new HashingService(config.AllowedHashstringCharacters, config.Hashstring));
        }

    }
}
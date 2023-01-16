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
using System.Data;
using System.Data.SqlClient;
using System.Web;
using MediatR;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using NLog;
using SFA.DAS.Audit.Client;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Auditing;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer;
using SFA.DAS.EmployerUsers.Infrastructure.Notification;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.HashingService;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;
using StructureMap;
using StructureMap.Building.Interception;
using StructureMap.Web.Pipeline;

namespace SFA.DAS.EmployerUsers.Web.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string AzureResource = "https://database.windows.net/";
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();


        public DefaultRegistry()
        {
            try
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

                IncludeRegistry<AutoConfigurationRegistry>();
                For<EmployerUsersConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerUsersConfiguration>("SFA.DAS.EmployerUsers.Web")).Singleton();
                ConfigureHashingService();

                AddDatabaseRegistrations(environment);

                AddEnvironmentSpecificRegistrations(environment);
                AddMediatrRegistrations();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to configure the StructureMap container.");
                throw;
            }
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
            For<IPasswordProfileRepository>().Use<InMemoryPasswordProfileRepository>();
            //For<IAuditApiClient>().Use<StubAuditApiClient>().Ctor<string>().Is(string.Format(@"{0}\App_Data\Audit\", AppDomain.CurrentDomain.BaseDirectory));
            Toggle<IAuditApiClient, StubAuditApiClient>("UseStubAuditClient");
        }

        private void Toggle<TPluginType, TConcreteType>(string configurationKey) where TConcreteType : TPluginType
        {
            For<TPluginType>().InterceptWith(new FuncInterceptor<TPluginType>((c, o) => c.GetInstance<IConfiguration>().GetValue<bool>(configurationKey) ? c.GetInstance<TConcreteType>() : o));
        }

        private void AddProductionRegistrations()
        {
            For<IUserRepository>().Use<SqlServerUserRepository>();
            For<IRelyingPartyRepository>().Use<SqlServerRelyingPartyRepository>();
            For<IPasswordProfileRepository>().Use<InMemoryPasswordProfileRepository>();
            For<IAuditApiClient>().Use<AuditApiClient>();
            For<IAuditApiConfiguration>().Use($"Audit API", c =>
            {
                var employerUsersConfig = c.GetInstance<EmployerUsersConfiguration>();
                return new AuditApiConfiguration
                {
                    ApiBaseUrl = employerUsersConfig.AuditApi.ApiBaseUrl,
                    IdentifierUri = employerUsersConfig.AuditApi.IdentifierUri
                };
            });
        }

        private void AddDatabaseRegistrations(string environment)
        {
            For<IDbConnection>().Use($"Build IDbConnection", c =>
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var employerUsersConfig = c.GetInstance<EmployerUsersConfiguration>();
                return environment.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
                    ? new SqlConnection(employerUsersConfig.SqlConnectionString)
                    : new SqlConnection
                    {
                        ConnectionString = employerUsersConfig.SqlConnectionString,
                        AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                    };
            });

            For<IUnitOfWork>().Use<UnitOfWork>();
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

        private void ConfigureHashingService()
        {
            For<IHashingService>().Use("IHashingService", x =>
            {
                var employerUsersConfig = x.GetInstance<EmployerUsersConfiguration>();
                return new HashingService.HashingService(employerUsersConfig.AllowedHashstringCharacters, employerUsersConfig.Hashstring);
            });
        }

    }
}
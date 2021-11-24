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
using AutoMapper;
using MediatR;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.Audit.Client;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Auditing;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer;
using StructureMap;
using StructureMap.Graph;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SFA.DAS.EmployerUsers.Api.DependencyResolution
{
    public class DefaultRegistry : Registry 
    {
        private const string AzureResource = "https://database.windows.net/";

        public DefaultRegistry() 
        {
            Scan(
                scan => {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS.EmployerUsers")
                               && !a.GetName().Name.Equals("SFA.DAS.EmployerUsers.Infrastructure"));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            For<IAuditMessageFactory>().Use<AuditMessageFactory>().Singleton();
            For<IAuditService>().Use<AuditService>();

            var environment = GetEnvironment();
            var configService = GetConfigService(environment);
            var employerUsersConfig = EmployerUsersConfig(configService);

            For<EmployerUsersConfiguration>().Use(employerUsersConfig);
            For<IConfigurationService>().Use(configService);
          
            AddDatabaseRegistrations(environment, employerUsersConfig.SqlConnectionString);

            AddEnvironmentSpecificRegistrations(environment);
            AddAutoMapperRegistrations();
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

        private ConfigurationService GetConfigService(string environment)
        {
            IConfigurationRepository configurationRepository;

            if (ConfigurationManager.AppSettings["LocalConfig"] == bool.TrueString)
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
            For<IAuditApiClient>().Use<StubAuditApiClient>().Ctor<string>().Is(string.Format(@"{0}\App_Data\Audit\", AppDomain.CurrentDomain.BaseDirectory));
        }

        private void AddProductionRegistrations()
        {
            For<IUserRepository>().Use<SqlServerUserRepository>();
            For<IAuditApiClient>().Use<AuditApiClient>();
            For<IAuditMessageFactory>().Use<AuditMessageFactory>().Singleton();
            For<IAuditApiConfiguration>().Use(() => new AuditApiConfiguration
            {
                ApiBaseUrl = ConfigurationManager.AppSettings["AuditApiBaseUrl"],
                ClientId = ConfigurationManager.AppSettings["AuditApiClientId"],
                ClientSecret = ConfigurationManager.AppSettings["AuditApiSecret"],
                IdentifierUri = ConfigurationManager.AppSettings["AuditApiIdentifierUri"],
                Tenant = ConfigurationManager.AppSettings["AuditApiTenant"]
            });
        }

        private void AddDatabaseRegistrations(string environment, string sqlConnectionString)
        {
            For<IDbConnection>().Use($"Build IDbConnection", c => {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                return environment.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
                    ? new SqlConnection(sqlConnectionString)
                    : new SqlConnection
                    {
                        ConnectionString = sqlConnectionString,
                        AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                    };
            });

            For<IUnitOfWork>().Use<UnitOfWork>();
        }

        private void AddAutoMapperRegistrations()
        {
            var config = new MapperConfiguration(c =>
            {
                c.AddProfile<DefaultProfile>();
            });

            var mapper = config.CreateMapper();
            
            For<IMapper>().Use(mapper);
        }

        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();
        }

        private EmployerUsersConfiguration EmployerUsersConfig(ConfigurationService configurationService)
        {
            return configurationService.Get<EmployerUsersConfiguration>();
        }
    }
}
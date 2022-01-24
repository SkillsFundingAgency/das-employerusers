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
using AutoMapper;
using MediatR;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.Audit.Client;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Auditing;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer;
using StructureMap;

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

            IncludeRegistry<AutoConfigurationRegistry>();
            For<EmployerUsersConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerUsersConfiguration>("SFA.DAS.EmployerUsers.Web")).Singleton();

            AddDatabaseRegistrations(environment);

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
    }
}
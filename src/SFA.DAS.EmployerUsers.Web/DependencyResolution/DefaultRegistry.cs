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
using SFA.DAS.CodeGenerator;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Data.DocumentDb;
using SFA.DAS.EmployerUsers.Infrastructure.Data.FileSystem;
using SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer;
using SFA.DAS.EmployerUsers.Web.Authentication;
using StructureMap.Web.Pipeline;

namespace SFA.DAS.EmployerUsers.Web.DependencyResolution
{
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;

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
            For<ICodeGenerator>().Use(new RandomCodeGenerator());

            AddEnvironmentSpecificRegistrations();
            AddMediatrRegistrations();
        }

        private void AddEnvironmentSpecificRegistrations()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            IConfigurationRepository configurationRepository;

            if (ConfigurationManager.AppSettings["LocalConfig"].AsBool())
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            }

            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions("SFA.DAS.EmployerUsers.Web", environment, "1.0"));
            For<IConfigurationService>().Use(configurationService);

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
            For<IHttpClientWrapper>().Use<StubHttpClientWrapper>();
        }
        private void AddProductionRegistrations()
        {
            For<IUserRepository>().Use<DocumentDbUserRepository>();
            For<IRelyingPartyRepository>().Use<DocumentDbRelyingPartyRepository>();
            For<IPasswordProfileRepository>().Use<InMemoryPasswordProfileRepository>();
            For<IHttpClientWrapper>().Use<HttpClientWrapper>();
        }

        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();
        }
    }
}
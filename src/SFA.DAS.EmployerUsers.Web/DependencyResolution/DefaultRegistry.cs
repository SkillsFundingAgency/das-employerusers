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
using System.Web;
using MediatR;
using Microsoft.Owin;
using Microsoft.WindowsAzure.ServiceRuntime;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Data;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Orchestrators;
using StructureMap.Web;
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

            AddEnvironmentSpecificRegistrations();
            AddMediatrRegistrations();

            //For<IOwinContext>().Transient().Use(() => HttpContext.Current.GetOwinContext());
            For<IOwinWrapper>().Transient().Use(() => new OwinWrapper(HttpContext.Current.GetOwinContext())).SetLifecycleTo(new HttpContextLifecycle());
        }

        private void AddEnvironmentSpecificRegistrations()
        {
            var environment = RoleEnvironment.IsEmulated ? "DEV" : "CLOUD_DEV";

            var configurationService = new ConfigurationService(new AzureTableStorageConfigurationRepository(),
                new ConfigurationOptions("SFA.DAS.EmployerUsers.Web", environment, "1.0"));
            For<IConfigurationService>().Use(configurationService);

            if (environment == "DEV")
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
            For<IUserRepository>().Use<FileSystemUserRepository>();
            For<IPasswordProfileRepository>().Use<InMemoryPasswordProfileRepository>();
        }
        private void AddProductionRegistrations()
        {
            For<IUserRepository>().Use<DocumentDbUserRepository>();
            For<IPasswordProfileRepository>().Use<InMemoryPasswordProfileRepository>();
        }

        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();
        }
    }
}
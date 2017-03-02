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
using MediatR;
using Microsoft.Azure;
using SFA.DAS.Audit.Client;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Auditing;
using SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer;
using StructureMap;
using StructureMap.Graph;

namespace SFA.DAS.EmployerUsers.Api.DependencyResolution
{

    public class DefaultRegistry : Registry
    {


        public DefaultRegistry()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS.EmployerUsers")
                               && !a.GetName().Name.Equals("SFA.DAS.EmployerUsers.Infrastructure"));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });


            For<IUserRepository>().Use<SqlServerUserRepository>();

            AddMediatrRegistrations();


            For<IAuditMessageFactory>().Use<AuditMessageFactory>().Singleton();
            For<IAuditService>().Use<AuditService>();
            if (environment.Equals("LOCAL"))
            {
                For<IAuditApiClient>().Use<StubAuditApiClient>().Ctor<string>().Is(string.Format(@"{0}\App_Data\Audit\", AppDomain.CurrentDomain.BaseDirectory));
            }
            else
            {
                For<IAuditApiClient>().Use<AuditApiClient>();
                For<IAuditApiConfiguration>().Use(() => new AuditApiConfiguration
                {
                    ApiBaseUrl = CloudConfigurationManager.GetSetting("AuditApiBaseUrl"),
                    ClientId = CloudConfigurationManager.GetSetting("AuditApiClientId"),
                    ClientSecret = CloudConfigurationManager.GetSetting("AuditApiSecret"),
                    IdentifierUri = CloudConfigurationManager.GetSetting("AuditApiIdentifierUri"),
                    Tenant = CloudConfigurationManager.GetSetting("AuditApiTenant")
                });
            }
        }

        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();
        }
    }
}
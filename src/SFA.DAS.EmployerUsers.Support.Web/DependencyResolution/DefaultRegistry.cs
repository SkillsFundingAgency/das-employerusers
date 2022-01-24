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

namespace SFA.DAS.EmployerUsers.Support.Web.DependencyResolution
{
    using System.Diagnostics.CodeAnalysis;
    using SFA.DAS.AutoConfiguration;
    using SFA.DAS.AutoConfiguration.DependencyResolution;
    using SFA.DAS.EAS.Account.Api.Client;
    using SFA.DAS.EmployerUsers.Api.Client;
    using SFA.DAS.EmployerUsers.Support.Web.Configuration;
    using SFA.DAS.Support.Shared.Discovery;
    using SFA.DAS.Support.Shared.SiteConnection;
    using StructureMap;

    [ExcludeFromCodeCoverage]
    public class DefaultRegistry : Registry {
        private const string ServiceName = "SFA.DAS.Support.EmployerUsers";
        private const string Version = "1.0";

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
					scan.With(new ControllerConvention());
                });

            For<IServiceConfiguration>().Singleton().Use(new ServiceConfiguration
                {
                    new EmployerAccountSiteManifest(),
                    new EmployerUserSiteManifest()
                }
            );

            IncludeRegistry<AutoConfigurationRegistry>();
            For<SupportConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<SupportConfiguration>(ServiceName)).Singleton();
            For<ISupportConfiguration>().Use<SupportConfiguration>();
            
            For<IEmployerUsersApiConfiguration>().Use(x => x.GetInstance<SupportConfiguration>().EmployerUsersApi);
            For<IAccountApiConfiguration>().Use(x => x.GetInstance<SupportConfiguration>().AccountApi);
            For<ISiteValidatorSettings>().Use(x => x.GetInstance<SupportConfiguration>().SiteValidator);
        }
    }
}
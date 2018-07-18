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
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using SFA.DAS.Support.Shared.Navigation;

namespace SFA.DAS.EmployerUsers.Support.Web.DependencyResolution
{
    using Microsoft.Azure;
    using SFA.DAS.Configuration;
    using SFA.DAS.Configuration.AzureTableStorage;
    using SFA.DAS.EAS.Account.Api.Client;
    using SFA.DAS.EmployerUsers.Api.Client;
    using SFA.DAS.EmployerUsers.Support.Web.Configuration;
    using SFA.DAS.NLog.Logger;
    using SFA.DAS.Support.Shared.Challenge;
    using SFA.DAS.Support.Shared.Discovery;
    using SFA.DAS.Support.Shared.SiteConnection;
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Support.EmployerUsers";
        private const string Version = "1.0";

       
        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
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


            WebConfiguration configuration = GetConfiguration();

            For<IWebConfiguration>().Use(configuration);
            For<IEmployerUsersApiConfiguration>().Use(configuration.EmployerUsersApi);
            For<IAccountApiConfiguration>().Use(configuration.AccountApi);
          

            For<ISiteConnectorSettings>().Use(configuration.SiteConnector);
            For<ISiteValidatorSettings>().Use(configuration.SiteValidator);
            For<ISiteSettings>().Use(configuration.Site);

            Uri portalUri = new Uri(
                configuration.Site.BaseUrls
                    .Split(',').FirstOrDefault(x=>x.StartsWith($"{SupportServiceIdentity.SupportPortal}"))?
                    .Split('|').LastOrDefault()?? "/", UriKind.RelativeOrAbsolute);

            For<Uri>().Singleton().Use((portalUri));

            For<IMenuTemplateTransformer>().Singleton().Use<MenuTemplateTransformer>();
            For<IMenuTemplateDatasource>().Singleton().Use(x=> new MenuTemplateDatasource("~/App_Data", x.GetInstance<ILog>()));
            For<IMenuClient>().Singleton().Use<MenuClient>();
            For<IMenuService>().Singleton().Use<MenuService>();
            For<IChallengeSettings>().Use(configuration.Challenge);
            For<IChallengeService>().Singleton().Use(c => new InMemoryChallengeService(new Dictionary<Guid, SupportAgentChallenge>(), c.GetInstance<IChallengeSettings>()));

        }

        private WebConfiguration GetConfiguration()
        {
            var environment = CloudConfigurationManager.GetSetting("EnvironmentName") ??
                              "LOCAL";
            var storageConnectionString = CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString") ??
                                          "UseDevelopmentStorage=true;";

            var configurationRepository = new AzureTableStorageConfigurationRepository(storageConnectionString); ;

            var configurationOptions = new ConfigurationOptions(ServiceName, environment, Version);

            var configurationService = new ConfigurationService(configurationRepository, configurationOptions);

            var webConfiguration = configurationService.Get<WebConfiguration>();

            return webConfiguration;
        }

       
    }
}
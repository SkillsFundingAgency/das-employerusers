using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.EmployerProfiles.Api.AppStart;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpsertUser;
using SFA.DAS.EmployerProfiles.Data;
using SFA.DAS.EmployerProfiles.Domain.Configuration;
using SFA.DAS.EmployerProfiles.Domain.RequestHandlers;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Api.UnitTests.AppStart;

public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(IEmployerProfilesDataContext))]
    [TestCase(typeof(IUserProfileRepository))]
    [TestCase(typeof(IValidator<UpsertUserRequest>))]
    public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
    {
        var hostEnvironment = new Mock<IWebHostEnvironment>();
        var serviceCollection = new ServiceCollection();

        var configuration = GenerateConfiguration();
        serviceCollection.AddLogging();
        serviceCollection.AddSingleton(hostEnvironment.Object);
        serviceCollection.AddSingleton(Mock.Of<IConfiguration>());
        serviceCollection.AddDistributedMemoryCache();
        serviceCollection.AddServiceRegistration();

        var apimDeveloperApiConfiguration = configuration
            .GetSection(nameof(EmployerProfilesConfiguration))
            .Get<EmployerProfilesConfiguration>();
        serviceCollection.AddDatabaseRegistration(apimDeveloperApiConfiguration, configuration["EnvironmentName"]);

        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
        Assert.That(type, Is.Not.Null);
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("EmployerProfilesConfiguration:ConnectionString", "test"),
                new KeyValuePair<string, string>("EnvironmentName", "test")
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}
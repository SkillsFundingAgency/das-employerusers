using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.EmployerProfiles.Api.AppStart;

public static class ConfigurationExtensions
{
    public static void LoadConfiguration(this IConfiguration config)
    {
        var configBuilder = new ConfigurationBuilder()
            .AddConfiguration(config)
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables();


        if (!config["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {

#if DEBUG
            configBuilder
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.Development.json", true);
#endif

            configBuilder.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = config["ConfigNames"].Split(",");
                    options.StorageConnectionString = config["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = config["Environment"];
                    options.PreFixConfigurationKeys = false;
                }
            );
        }

        configBuilder.Build();
    }
}
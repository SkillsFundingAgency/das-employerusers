using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using SFA.DAS.EmployerProfiles.Data;
using SFA.DAS.EmployerProfiles.Domain.Configuration;

namespace SFA.DAS.EmployerProfiles.Api.AppStart;

public static class DatabaseExtensions
{
    public static void AddDatabaseRegistration(this IServiceCollection services, EmployerProfilesConfiguration config, string? environmentName)
    {
        services.AddHttpContextAccessor();
        if (environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddDbContext<EmployerProfilesDataContext>(options => options.UseInMemoryDatabase("SFA.DAS.EmployerProfiles"), ServiceLifetime.Transient);
        }
        else if (environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddDbContext<EmployerProfilesDataContext>(options=>options.UseSqlServer(config.ConnectionString),ServiceLifetime.Transient);
        }
        else
        {
            services.AddDbContext<EmployerProfilesDataContext>(ServiceLifetime.Transient);    
        }
            
        services.AddSingleton(new EnvironmentConfiguration(environmentName));

        services.AddTransient<IEmployerProfilesDataContext, EmployerProfilesDataContext>(provider => provider.GetService<EmployerProfilesDataContext>()!);
        services.AddTransient(provider => new Lazy<EmployerProfilesDataContext>(provider.GetService<EmployerProfilesDataContext>()!));
        services.AddSingleton(new ChainedTokenCredential(
            new ManagedIdentityCredential(),
            new AzureCliCredential(),
            new VisualStudioCodeCredential(),
            new VisualStudioCredential())
        );
    }
}
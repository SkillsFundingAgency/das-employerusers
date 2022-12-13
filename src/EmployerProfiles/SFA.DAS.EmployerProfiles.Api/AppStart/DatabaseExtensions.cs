using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerProfiles.Data;
using SFA.DAS.EmployerProfiles.Domain.Configuration;

namespace SFA.DAS.EmployerProfiles.Api.AppStart;

public static class DatabaseExtensions
{
    public static void AddDatabaseRegistration(this IServiceCollection services, EmployerProfilesConfiguration config, string environmentName)
    {
        if (environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddDbContext<EmployerProfilesDataContext>(options => options.UseInMemoryDatabase("SFA.DAS.EmployerProfiles"), ServiceLifetime.Transient);
        }
        else //if (environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddDbContext<EmployerProfilesDataContext>(options=>options.UseSqlServer(config.ConnectionString),ServiceLifetime.Transient);
        }
        //else
        //{
            // services.AddSingleton(new AzureServiceTokenProvider());
            // services.AddDbContext<CoursesDataContext>(ServiceLifetime.Transient);    
        //}
            
            

        services.AddTransient<IEmployerProfilesDataContext, EmployerProfilesDataContext>(provider => provider.GetService<EmployerProfilesDataContext>()!);
        services.AddTransient(provider => new Lazy<EmployerProfilesDataContext>(provider.GetService<EmployerProfilesDataContext>()!));
            
    }
}
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerProfiles.Domain.Configuration;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Data;

public interface IEmployerProfilesDataContext
{
    DbSet<UserProfileEntity> UserProfileEntities { get; set; }
}

public class EmployerProfilesDataContext : DbContext, IEmployerProfilesDataContext
{
    public DbSet<UserProfileEntity> UserProfileEntities { get; set; }

    private readonly EmployerProfilesConfiguration? _configuration;

    public EmployerProfilesDataContext()
    {
    }

    public EmployerProfilesDataContext(DbContextOptions options) : base(options)
    {
            
    }
    public EmployerProfilesDataContext(IOptions<EmployerProfilesConfiguration> config, DbContextOptions options) :base(options)//, AzureServiceTokenProvider azureServiceTokenProvider
    {
        _configuration = config.Value;
        //_azureServiceTokenProvider = azureServiceTokenProvider;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
            
        // if (_configuration == null || _azureServiceTokenProvider == null)
        // {
        //     return;
        // }
            
        var connection = new SqlConnection
        {
            ConnectionString = _configuration?.ConnectionString,
     //       AccessToken = _azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result,
        };
            
        optionsBuilder.UseSqlServer(connection,options=>
            options.EnableRetryOnFailure(
                5,
                TimeSpan.FromSeconds(20),
                null
            ));

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new Users.UserProfilesEntityConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}
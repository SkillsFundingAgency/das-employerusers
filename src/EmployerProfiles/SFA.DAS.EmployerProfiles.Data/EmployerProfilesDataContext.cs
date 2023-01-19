using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerProfiles.Domain.Configuration;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Data;

public interface IEmployerProfilesDataContext
{
    DbSet<UserProfileEntity> UserProfileEntities { get; set; }
    int SaveChanges();
}

public class EmployerProfilesDataContext : DbContext, IEmployerProfilesDataContext
{
    private const string AzureResource = "https://database.windows.net/";
    private readonly ChainedTokenCredential _azureServiceTokenProvider;
    private readonly EnvironmentConfiguration _environmentConfiguration;
    public DbSet<UserProfileEntity> UserProfileEntities { get; set; }

    private readonly EmployerProfilesConfiguration? _configuration;

    public EmployerProfilesDataContext()
    {
    }

    public EmployerProfilesDataContext(DbContextOptions options) : base(options)
    {
            
    }
    public EmployerProfilesDataContext(IOptions<EmployerProfilesConfiguration> config, DbContextOptions options, ChainedTokenCredential azureServiceTokenProvider, EnvironmentConfiguration environmentConfiguration) :base(options)//, AzureServiceTokenProvider azureServiceTokenProvider
    {
        _azureServiceTokenProvider = azureServiceTokenProvider;
        _environmentConfiguration = environmentConfiguration;
        _configuration = config.Value;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
            
        if (_configuration == null 
            || _environmentConfiguration.EnvironmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase)
            || _environmentConfiguration.EnvironmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
        {
            return;
        }
            
        var connection = new SqlConnection
        {
            ConnectionString = _configuration.ConnectionString,
            AccessToken = _azureServiceTokenProvider.GetTokenAsync(new TokenRequestContext(scopes: new string[] { AzureResource })).Result.Token,
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
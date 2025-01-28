using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Data.Users;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly IEmployerProfilesDataContext _employerProfilesDataContext;
    private readonly ILogger<UserProfileRepository> _logger;

    public UserProfileRepository(IEmployerProfilesDataContext employerProfilesDataContext, ILogger<UserProfileRepository> logger)
    {
        _employerProfilesDataContext = employerProfilesDataContext;
        _logger = logger;
    }

    public async Task<UserProfileEntity?> GetByEmail(string searchEntityEmail)
    {
        return await _employerProfilesDataContext.UserProfileEntities.SingleOrDefaultAsync(c =>
            c.Email == searchEntityEmail);
    }

    public async Task<UserProfileEntity?> GetById(Guid id)
    {
        return await _employerProfilesDataContext.UserProfileEntities.SingleOrDefaultAsync(c => c!.Id == id.ToString());
    }
    
    public async Task<UserProfileEntity?> GetByGovIdentifier(string govUkIdentifier)
    {
        var singleOrDefaultAsync = await _employerProfilesDataContext.UserProfileEntities.SingleOrDefaultAsync(c =>
            c.GovUkIdentifier == govUkIdentifier);
        return singleOrDefaultAsync;
    }

    public async Task<Tuple<UserProfileEntity,bool>> Upsert(UserProfileEntity entity)
    {
        if (string.IsNullOrEmpty(entity.Id))
        {
            throw new ArgumentNullException(nameof(entity.Id));
        }

        if (string.IsNullOrEmpty(entity.Email))
        {
            throw new ArgumentNullException(nameof(entity.Email));
        }

        var userById = await GetById(new Guid(entity.Id));
        var userByEmail = await GetByEmail(entity.Email);
        
        _logger.LogInformation("UserProfileRepository-Upsert getById {Id} result: {Data}", entity.Id, JsonSerializer.Serialize(userById));
        _logger.LogInformation("UserProfileRepository-Upsert getByEmail {Id} result: {Data}", entity.Email, JsonSerializer.Serialize(userByEmail));
        
        var userProfileUpdate = await GetById(new Guid(entity.Id)) ?? await GetByEmail(entity.Email);
        _logger.LogInformation("UserProfileRepository-Upsert userProfileUpdate is null {Result}", userProfileUpdate == null);
        
        if (userProfileUpdate == null)
        {
            _employerProfilesDataContext.UserProfileEntities.Add(entity);
            _employerProfilesDataContext.SaveChanges();
            return new Tuple<UserProfileEntity, bool>(entity, true);
        }

        userProfileUpdate.Email = entity.Email ?? entity.Email;
        userProfileUpdate.FirstName = entity.FirstName ?? userProfileUpdate.FirstName;
        userProfileUpdate.LastName = entity.LastName ?? userProfileUpdate.LastName;
        userProfileUpdate.GovUkIdentifier = entity.GovUkIdentifier ?? userProfileUpdate.GovUkIdentifier;
        
        _employerProfilesDataContext.SaveChanges();
        
        return new Tuple<UserProfileEntity, bool>(userProfileUpdate, false);
    }

    public async Task<bool> UpdateUserSuspendedFlag(Guid id, bool isSuspended)
    {
        var userProfileUpdate = await GetById(id);
        if (userProfileUpdate == null)
        {
            return false;
        }   
        userProfileUpdate.IsSuspended = isSuspended;
        _employerProfilesDataContext.SaveChanges();
        return true;
    }
}
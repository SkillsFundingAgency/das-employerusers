using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Data.Users;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly IEmployerProfilesDataContext _employerProfilesDataContext;

    public UserProfileRepository(IEmployerProfilesDataContext employerProfilesDataContext)
    {
        _employerProfilesDataContext = employerProfilesDataContext;
    }

    public async Task<UserProfileEntity?> GetByEmail(string searchEntityEmail)
    {
        return await _employerProfilesDataContext.UserProfileEntities.SingleOrDefaultAsync(c =>
            c!.Email.Equals(searchEntityEmail, StringComparison.CurrentCultureIgnoreCase));
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

    public async Task<UserProfileEntity?> Upsert(UserProfileEntity entity)
    {
        var userProfileUpdate = await GetById(Guid.Parse(entity.Id));
        if (userProfileUpdate == null)
        {
            _employerProfilesDataContext.UserProfileEntities.Add(entity);
            _employerProfilesDataContext.SaveChanges();
            return entity;
        }
        userProfileUpdate.FirstName = entity.FirstName ?? userProfileUpdate.FirstName;
        userProfileUpdate.LastName = entity.LastName ?? userProfileUpdate.LastName;
        userProfileUpdate.GovUkIdentifier = entity.GovUkIdentifier ?? userProfileUpdate.GovUkIdentifier;
        _employerProfilesDataContext.SaveChanges();
        return userProfileUpdate;
    }

    public async Task UpdateUserSuspendedFlag(Guid id, bool isSuspended)
    {
        var userProfileUpdate = await GetById(id);
        if (userProfileUpdate == null)
        {
            return;
        }   
        userProfileUpdate.IsSuspended = isSuspended;
        _employerProfilesDataContext.SaveChanges();
    }
}
namespace SFA.DAS.EmployerProfiles.Domain.UserProfiles;

public interface IUserProfileRepository
{
    Task<UserProfileEntity?> GetByEmail(string searchEntityEmail);
    Task<UserProfileEntity?> GetById(Guid id);
    Task<UserProfileEntity?> GetByGovIdentifier(string govUkIdentifier);
    Task<Tuple<UserProfileEntity, bool>> Upsert(UserProfileEntity entity);
    Task<bool> UpdateUserSuspendedFlag(Guid id, bool isSuspended);
}
using Newtonsoft.Json;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.DocumentDb
{
    internal class DocumentDbUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string PasswordProfileId { get; set; }
        public bool IsActive { get; set; }
        public string AccessCode { get; set; }
        public int FailedLoginAttempts { get; set; }
        public bool IsLocked { get; set; }
        public string UnlockCode { get; set; }
        public SecurityCode[] SecurityCodes { get; set; }

        internal static DocumentDbUser FromDomainUser(User user)
        {
            return new DocumentDbUser
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                IsActive = user.IsActive,
                LastName = user.LastName,
                Password = user.Password,
                Salt = user.Salt,
                PasswordProfileId = user.PasswordProfileId,
                FailedLoginAttempts = user.FailedLoginAttempts,
                IsLocked = user.IsLocked,
                SecurityCodes = user.SecurityCodes
            };
        }

        internal User ToDomainUser()
        {
            return new User
            {
                Id = Id,
                Email = Email,
                FirstName = FirstName,
                IsActive = IsActive,
                LastName = LastName,
                Password = Password,
                Salt = Salt,
                PasswordProfileId = PasswordProfileId,
                FailedLoginAttempts = FailedLoginAttempts,
                IsLocked = IsLocked,
                SecurityCodes = SecurityCodes
            };
        }
    }
}

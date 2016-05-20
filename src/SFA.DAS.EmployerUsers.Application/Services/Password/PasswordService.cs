using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Services.Password
{
    public class PasswordService : IPasswordService
    {
        private readonly IPasswordProfileRepository _passwordProfileRepo;

        public PasswordService(IPasswordProfileRepository passwordProfileRepo)
        {
            _passwordProfileRepo = passwordProfileRepo;
        }

        public async Task<SecuredPassword> GenerateAsync(string plainText)
        {
            var profile = await GetActivePasswordProfile();

            var rng = new RNGCryptoServiceProvider();
            var salt = new byte[profile.SaltLength];
            rng.GetBytes(salt);

            var saltedPassword = salt.Concat(Encoding.Unicode.GetBytes(plainText)).ToArray();

            var hasher = new HMACSHA256(Convert.FromBase64String(profile.Key));
            var hash = hasher.ComputeHash(saltedPassword);

            var pbkdf2 = new Rfc2898DeriveBytes(Convert.ToBase64String(hash), salt, profile.WorkFactor);
            var password = pbkdf2.GetBytes(profile.StorageLength);

            return new SecuredPassword
            {
                HashedPassword = Convert.ToBase64String(password),
                Salt = Convert.ToBase64String(salt),
                ProfileId = profile.Id
            };
        }

        public Task<bool> VerifyAsync(string hashedPassword, string salt, string profileId)
        {
            throw new NotImplementedException();
        }

        private Task<PasswordProfile> GetActivePasswordProfile()
        {
            return GetPasswordProfile("XXX");
        }
        private async Task<PasswordProfile> GetPasswordProfile(string id)
        {
            var profiles = await _passwordProfileRepo.GetAllAsync();
            return profiles.SingleOrDefault(pp => pp.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }
    }
}

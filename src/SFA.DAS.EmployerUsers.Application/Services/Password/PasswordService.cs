using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.Services.Password
{
    public class PasswordService : IPasswordService
    {
        private readonly IConfigurationService _configurationService;
        private readonly IPasswordProfileRepository _passwordProfileRepo;

        public PasswordService(IConfigurationService configurationService, IPasswordProfileRepository passwordProfileRepo)
        {
            _configurationService = configurationService;
            _passwordProfileRepo = passwordProfileRepo;
        }

        public async Task<SecuredPassword> GenerateAsync(string plainText)
        {
            var profile = await GetActivePasswordProfile();

            var rng = new RNGCryptoServiceProvider();
            var salt = new byte[profile.SaltLength];
            rng.GetBytes(salt);

            var password = SecurePassword(plainText, salt, profile);

            return new SecuredPassword
            {
                HashedPassword = password,
                Salt = Convert.ToBase64String(salt),
                ProfileId = profile.Id
            };
        }

        public async Task<bool> VerifyAsync(string plainTextPassword, string hashedPassword, string salt, string profileId)
        {
            if (string.IsNullOrEmpty(plainTextPassword))
            {
                throw new ArgumentNullException(nameof(plainTextPassword));
            }
            if (string.IsNullOrEmpty(hashedPassword))
            {
                throw new ArgumentNullException(nameof(hashedPassword));
            }
            if (string.IsNullOrEmpty(salt))
            {
                throw new ArgumentNullException(nameof(salt));
            }
            if (string.IsNullOrEmpty(profileId))
            {
                throw new ArgumentNullException(nameof(profileId));
            }

            var profile = await GetPasswordProfile(profileId);

            var password = SecurePassword(plainTextPassword, Convert.FromBase64String(salt), profile);

            Console.WriteLine(password);
            return password == hashedPassword;
        }

        public bool CheckPasswordMatchesRequiredComplexity(string password)
        {
            return !string.IsNullOrEmpty(password) && Regex.IsMatch(password, @"^(?=(.*[0-9].*))(?=(.*[a-z].*))(?=(.*[A-Z].*)).{8,}$");
        }

        private async Task<PasswordProfile> GetActivePasswordProfile()
        {
            var configuration = await _configurationService.GetAsync<EmployerUsersConfiguration>();
            return await GetPasswordProfile(configuration.Account.ActivePasswordProfileId);
        }
        private async Task<PasswordProfile> GetPasswordProfile(string id)
        {
            var profiles = await _passwordProfileRepo.GetAllAsync();
            return profiles.SingleOrDefault(pp => pp.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        private string SecurePassword(string plainText, byte[] salt, PasswordProfile profile)
        {
            var saltedPassword = salt.Concat(Encoding.Unicode.GetBytes(plainText)).ToArray();

            var hasher = new HMACSHA256(Convert.FromBase64String(profile.Key));
            var hash = hasher.ComputeHash(saltedPassword);

            var pbkdf2 = new Rfc2898DeriveBytes(Convert.ToBase64String(hash), salt, profile.WorkFactor);
            var password = pbkdf2.GetBytes(profile.StorageLength);

            return Convert.ToBase64String(password);
        }
    }
}

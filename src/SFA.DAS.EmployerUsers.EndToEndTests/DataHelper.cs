using System;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.EndToEndTests
{
    public class DataHelper
    {
        private readonly TestSettings _settings;

        public DataHelper(TestSettings settings)
        {
            _settings = settings;
        }

        public string GetAccessCodeForUser(string emailAddress)
        {
            using (var connection = new SqlConnection(_settings.UsersConnectionString))
            {
                return QuerySingle<string>(connection,
                    "SELECT TOP 1 sc.Code FROM [User] u INNER JOIN [UserSecurityCode] sc ON u.Id = sc.UserId WHERE Email=@EmailAddress ORDER BY ExpiryTime DESC",
                    new { emailAddress });
            }
        }

        public bool IsUserActive(string emailAddress)
        {
            using (var connection = new SqlConnection(_settings.UsersConnectionString))
            {
                return QuerySingle<bool>(connection,
                    "SELECT u.IsActive FROM [User] u WHERE Email=@EmailAddress",
                    new { emailAddress });
            }
        }

        public void CreateUser(string emailAddress, string password, string firstName, string lastName)
        {
            PasswordProfile profile;
            using (var connection = new SqlConnection(_settings.ProfilesConnectionString))
            {
                profile = QuerySingle<PasswordProfile>(connection, "SELECT TOP 1 * FROM PasswordProfile");
            }

            var rng = new RNGCryptoServiceProvider();
            var salt = new byte[profile.SaltLength];
            rng.GetBytes(salt);

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                Password = SecurePassword(password, salt, profile),
                Salt = Convert.ToBase64String(salt),
                PasswordProfileId = profile.Id,
                IsActive = true,
                FailedLoginAttempts = 0,
                IsLocked = false
            };

            using (var connection = new SqlConnection(_settings.UsersConnectionString))
            {
                Execute(connection, "CreateUser @Id, @FirstName, @LastName, @Email, @Password, @Salt, @PasswordProfileId, @IsActive, @FailedLoginAttempts, @IsLocked",
                    user);

                Execute(connection, "CreateHistoricalPassword @UserId, @Password, @Salt, @PasswordProfileId, @DateSet",
                       new { UserId = user.Id, user.Password, user.Salt, user.PasswordProfileId, DateSet = DateTime.Now });
            }
        }



        private T[] Query<T>(SqlConnection connection, string command, object param = null)
        {
            return connection.Query<T>(command, param).ToArray();
        }
        private T QuerySingle<T>(SqlConnection connection, string command, object param = null)
        {
            return Query<T>(connection, command, param).SingleOrDefault();
        }
        private void Execute(SqlConnection connection, string command, object param = null)
        {
            connection.Execute(command, param);
        }

        private string SecurePassword(string plainText, byte[] salt, PasswordProfile profile)
        {
            //TODO: Refactor password service code to make it consumable from here too
            var saltedPassword = salt.Concat(Encoding.Unicode.GetBytes(plainText)).ToArray();

            var hasher = new HMACSHA256(Convert.FromBase64String(profile.Key));
            var hash = hasher.ComputeHash(saltedPassword);

            var pbkdf2 = new Rfc2898DeriveBytes(Convert.ToBase64String(hash), salt, profile.WorkFactor);
            var password = pbkdf2.GetBytes(profile.StorageLength);

            return Convert.ToBase64String(password);
        }
    }
}

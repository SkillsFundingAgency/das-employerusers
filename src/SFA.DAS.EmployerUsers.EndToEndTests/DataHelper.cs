using System;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

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



        private T[] Query<T>(SqlConnection connection, string command, object param = null)
        {
            return connection.Query<T>(command, param).ToArray();
        }
        private T QuerySingle<T>(SqlConnection connection, string command, object param = null)
        {
            return Query<T>(connection, command, param).SingleOrDefault();
        }
    }
}

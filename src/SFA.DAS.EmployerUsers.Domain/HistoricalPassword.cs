using System;
using System.Security.Cryptography;
using System.Text;

namespace SFA.DAS.EmployerUsers.Domain
{
    public class HistoricalPassword
    {
        public string Password { get; set; }
        public string Salt { get; set; }
        public string PasswordProfileId { get; set; }
        public DateTime DateSet { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is HistoricalPassword))
            {
                return false;
            }

            var historicalPassword = (HistoricalPassword)obj;
            return Password == historicalPassword.Password
                && Salt == historicalPassword.Salt
                && PasswordProfileId == historicalPassword.PasswordProfileId;
        }
        public override int GetHashCode()
        {
            var hashString = $"{Password}|{Salt}|{PasswordProfileId}";
            var hashBuffer = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(hashString));
            return BitConverter.ToInt32(hashBuffer, 0);
        }
    }
}
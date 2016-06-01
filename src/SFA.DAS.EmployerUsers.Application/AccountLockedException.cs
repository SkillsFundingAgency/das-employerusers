using System;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application
{
    public class AccountLockedException : Exception
    {
        public AccountLockedException(User user)
            : base(MakeDefaultErrorMessage(user))
        {
            User = user;
        }

        public User User { get; set; }

        private static string MakeDefaultErrorMessage(User user)
        {
            return $"Account for {user.Email} is locked.";
        }
    }
}

using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Events.AccountLocked
{
    public class AccountLockedEvent : IAsyncNotification
    {
        public User User { get; set; }
    }
}

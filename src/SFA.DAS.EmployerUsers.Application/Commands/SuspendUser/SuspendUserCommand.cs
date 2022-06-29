using MediatR;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Commands.SuspendUser
{
    public class SuspendUserCommand : IAsyncRequest
    {
        public User User { get; private set; }
        public ChangedByUserInfo ChangedByUserInfo { get; private set; }

        public SuspendUserCommand(User user, ChangedByUserInfo changedByUserInfo)
        {
            User = user;
            ChangedByUserInfo = changedByUserInfo;
        }
    }
}

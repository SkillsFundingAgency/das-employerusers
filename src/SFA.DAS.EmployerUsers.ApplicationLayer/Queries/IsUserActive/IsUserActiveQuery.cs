using MediatR;

namespace SFA.DAS.EmployerUsers.ApplicationLayer.Queries.IsUserActive
{
    public class IsUserActiveQuery : IAsyncRequest<bool>
    {
        public string UserId { get; set; }
    }
}

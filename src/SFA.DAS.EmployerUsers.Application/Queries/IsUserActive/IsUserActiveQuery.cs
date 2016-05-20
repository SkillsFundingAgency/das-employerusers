using MediatR;

namespace SFA.DAS.EmployerUsers.Application.Queries.IsUserActive
{
    public class IsUserActiveQuery : IAsyncRequest<bool>
    {
        public string UserId { get; set; }
    }
}

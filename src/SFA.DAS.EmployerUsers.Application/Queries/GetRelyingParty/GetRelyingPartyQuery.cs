using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetRelyingParty
{
    public class GetRelyingPartyQuery : IAsyncRequest<RelyingParty>
    {
        public string Id { get; set; }
    }
}

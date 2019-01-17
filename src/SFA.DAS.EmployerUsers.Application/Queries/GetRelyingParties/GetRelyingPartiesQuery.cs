using System.Collections.Generic;
using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetRelyingParties
{
    public class GetRelyingPartiesQuery : IAsyncRequest<IEnumerable<RelyingParty>>
    {
    }
}

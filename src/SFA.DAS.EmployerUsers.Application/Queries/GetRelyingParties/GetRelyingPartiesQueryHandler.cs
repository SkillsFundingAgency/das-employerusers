using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetRelyingParties
{
    public class GetRelyingPartiesQueryHandler : IAsyncRequestHandler<GetRelyingPartiesQuery, IEnumerable<RelyingParty>>
    {
        private readonly IRelyingPartyRepository _relyingPartyRepository;

        public GetRelyingPartiesQueryHandler(IRelyingPartyRepository relyingPartyRepository)
        {
            _relyingPartyRepository = relyingPartyRepository;
        }

        public async Task<IEnumerable<RelyingParty>> Handle(GetRelyingPartiesQuery message)
        {
            var relyingParties = await _relyingPartyRepository.GetAllAsync();
            return relyingParties;
        }
    }
}
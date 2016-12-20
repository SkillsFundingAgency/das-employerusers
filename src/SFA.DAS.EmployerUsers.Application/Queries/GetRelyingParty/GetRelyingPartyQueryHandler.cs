using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetRelyingParty
{
    public class GetRelyingPartyQueryHandler : IAsyncRequestHandler<GetRelyingPartyQuery, RelyingParty>
    {
        private readonly IRelyingPartyRepository _relyingPartyRepository;

        public GetRelyingPartyQueryHandler(IRelyingPartyRepository relyingPartyRepository)
        {
            _relyingPartyRepository = relyingPartyRepository;
        }

        public async Task<RelyingParty> Handle(GetRelyingPartyQuery message)
        {
            var relyingParties = await _relyingPartyRepository.GetAllAsync();
            return relyingParties.SingleOrDefault(rp => rp.Id.Equals(message.Id, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
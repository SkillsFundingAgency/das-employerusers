using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByGovIdentifier;

public class GetUserByGovIdentifierQuery : IRequest<GetUserByGovIdentifierQueryResult>
{
    public string GovIdentifier { get; set; }
}
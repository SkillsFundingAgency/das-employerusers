using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetUnlockCodeLength
{
    public class GetUnlockCodeDetailsQueryHandler : IAsyncRequestHandler<GetUnlockCodeQuery, GetUnlockCodeResponse>
    {
        private readonly IConfigurationService _configurationService;

        public GetUnlockCodeDetailsQueryHandler(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }
        public async Task<GetUnlockCodeResponse> Handle(GetUnlockCodeQuery message)
        {
            var config = (await _configurationService.GetAsync<EmployerUsersConfiguration>())?.Account;

            if (config == null)
            {
                throw new ArgumentException("Cannot find config to get unlock code length");
            }

            return new GetUnlockCodeResponse { UnlockCodeLength = config.UnlockCodeLength };
        }
    }
}

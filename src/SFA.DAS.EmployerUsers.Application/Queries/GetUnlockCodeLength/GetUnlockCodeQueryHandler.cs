using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetUnlockCodeLength
{
    public class GetUnlockCodeDetailsQueryHandler : IAsyncRequestHandler<GetUnlockCodeQuery, GetUnlockCodeResponse>
    {
        private readonly EmployerUsersConfiguration _configuration;

        public GetUnlockCodeDetailsQueryHandler(EmployerUsersConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<GetUnlockCodeResponse> Handle(GetUnlockCodeQuery message)
        {
            return await Task.Run(() =>
            {
                var config = _configuration?.Account;

                if (config == null)
                {
                    throw new ArgumentException("Cannot find config to get unlock code length");
                }

                return new GetUnlockCodeResponse { UnlockCodeLength = config.UnlockCodeLength };
            });
            
        }
    }
}

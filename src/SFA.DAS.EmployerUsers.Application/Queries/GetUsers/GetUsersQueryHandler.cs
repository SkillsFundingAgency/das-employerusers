using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetUsers
{
    public class GetUsersQueryHandler : IAsyncRequestHandler<GetUsersQuery, GetUsersQueryResponse>
    {
        private readonly IUserRepository _userRepository;

        public GetUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GetUsersQueryResponse> Handle(GetUsersQuery message)
        {

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var users = await _userRepository.GetUsers(message.PageSize, message.PageNumber);
            var recordCount = await _userRepository.GetUserCount();

            return new GetUsersQueryResponse(){Users = users, RecordCount = recordCount };
        }
    }
}

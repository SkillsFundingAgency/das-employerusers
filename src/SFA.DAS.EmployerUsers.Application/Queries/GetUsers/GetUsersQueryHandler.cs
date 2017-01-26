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
    public class GetUsersQueryHandler : IAsyncRequestHandler<GetUsersQuery, User[]>
    {
        private readonly IUserRepository _userRepository;

        public GetUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<User[]> Handle(GetUsersQuery message)
        {

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return _userRepository.GetUsers(message.PageSize, message.PageNumber);
        }
    }
}

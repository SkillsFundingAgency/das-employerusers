using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Queries.SearchUsers
{
    public class SearchUsersQueryHandler : IAsyncRequestHandler<SearchUsersQuery, SearchUsersQueryResponse>
    {
        private readonly IUserRepository _userRepository;

        public SearchUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<SearchUsersQueryResponse> Handle(SearchUsersQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var users = await _userRepository.SearchUsers(message.Criteria, message.PageSize, message.PageNumber);

            return new SearchUsersQueryResponse { Users = users.UserList, RecordCount = users.UserCount };
        }
    }
}

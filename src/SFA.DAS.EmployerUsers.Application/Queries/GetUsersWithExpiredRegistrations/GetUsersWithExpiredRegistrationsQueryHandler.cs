using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetUsersWithExpiredRegistrations
{
    public class GetUsersWithExpiredRegistrationsQueryHandler : IAsyncRequestHandler<GetUsersWithExpiredRegistrationsQuery, User[]>
    {
        private readonly IUserRepository _userRepository;

        public GetUsersWithExpiredRegistrationsQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User[]> Handle(GetUsersWithExpiredRegistrationsQuery message)
        {
            return await _userRepository.GetUsersWithExpiredRegistrations();
        }
    }
}
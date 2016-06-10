using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetUserByEmailAddress
{
    public class GetUserByEmailAddressQueryHandler : IAsyncRequestHandler<GetUserByIdQuery, User>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByEmailAddressQueryHandler(IUserRepository userRepository)
        {
            if (userRepository == null)
                throw new ArgumentNullException(nameof(userRepository));
            _userRepository = userRepository;
        }

        public Task<User> Handle(GetUserByEmailAddressQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return _userRepository.GetByEmailAddress(message.EmailAddress);
        }
    }
}
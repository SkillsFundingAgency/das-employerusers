using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Data.User;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.ApplicationLayer.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IAsyncRequestHandler<GetUserByIdQuery, User>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<User> Handle(GetUserByIdQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return _userRepository.GetById(message.UserId);
        }
    }
}

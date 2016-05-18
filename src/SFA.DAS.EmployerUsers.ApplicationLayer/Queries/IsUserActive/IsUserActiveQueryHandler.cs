using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Data.User;

namespace SFA.DAS.EmployerUsers.ApplicationLayer.Queries.IsUserActive
{
    public class IsUserActiveQueryHandler : IAsyncRequestHandler<IsUserActiveQuery, bool>
    {
        private readonly IUserRepository _userRepository;

        public IsUserActiveQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(IsUserActiveQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var user = await _userRepository.GetById(message.UserId);
            if (user == null)
            {
                return false;
            }
            return user.IsActive;
        }
    }
}

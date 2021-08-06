using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.HashingService;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Application.Exceptions;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetUserByHashedId
{
    public class GetUserByHashedIdQueryHandler : IAsyncRequestHandler<GetUserByHashedIdQuery, User>
    {
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetUserByHashedIdQuery> _validator;
        private readonly IUserRepository _userRepository;

        public GetUserByHashedIdQueryHandler(IHashingService hashingService, IValidator<GetUserByHashedIdQuery> validator, IUserRepository userRepository)
        {
            _hashingService = hashingService;
            _validator = validator;
            _userRepository = userRepository;
        }

        public async Task<User> Handle(GetUserByHashedIdQuery message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var userId = _hashingService.DecodeValueToGuid(message.HashedUserId);

            if (userId == Guid.Empty)
            {
                return null;
            }

            var user = await _userRepository.GetById(userId.ToString());

            return user;
        }

        
    }
}
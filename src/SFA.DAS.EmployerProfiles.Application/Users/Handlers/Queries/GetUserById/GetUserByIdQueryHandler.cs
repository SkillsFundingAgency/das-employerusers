using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdQueryResult>
{
    private readonly IUserProfileRepository _repository;

    public GetUserByIdQueryHandler(IUserProfileRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<GetUserByIdQueryResult> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetById(request.Id);

        return new GetUserByIdQueryResult
        {
            UserProfile = result
        };
    }
}
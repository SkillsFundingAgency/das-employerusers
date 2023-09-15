using MediatR;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByEmail;

public class GetUserByEmailQuery : IRequest<GetUserByEmailQueryResult>
{
    public string Email { get; set; }
}
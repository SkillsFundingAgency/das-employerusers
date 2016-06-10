using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetUserByEmailAddress
{
    public class GetUserByEmailAddressQuery : IAsyncRequest<User>
    {
        public string EmailAddress { get; set; }
    }
}
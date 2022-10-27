using MediatR;

namespace SFA.DAS.EmployerUsers.Application.Commands.UpdateUser
{
    public class UpdateUserCommand : IAsyncRequest<UpdateUserCommandResponse>
    {
        public string Email { get; set; }
        public string GovUkIdentifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
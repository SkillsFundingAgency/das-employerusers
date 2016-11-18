using MediatR;

namespace SFA.DAS.EmployerUsers.Application.Commands.RegisterUser
{
    public class RegisterUserCommand : IAsyncRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool HasAcceptedTermsAndConditions { get; set; }
        public string Id { get; set; }
        public string ReturnUrl { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerUsers.ApplicationLayer.Commands.RegisterUser
{
    public class RegisterUserCommand : IAsyncRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmEmail { get; set; }
        public string ConfirmPassword { get; set; }
    }
}

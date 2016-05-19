using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.ApplicationLayer.Commands.ActivateUser
{
    public class ActivateUserCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public string AccessCode { get; set; }
        public User User { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerUsers.Application.Commands.ResendUnlockCode
{
    public class ResendUnlockCodeCommand : IAsyncRequest
    {
        public string Email { get; set; }
    }
}

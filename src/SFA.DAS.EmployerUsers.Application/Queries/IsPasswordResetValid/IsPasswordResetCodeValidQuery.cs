using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerUsers.Application.Queries.IsPasswordResetValid
{
    public class IsPasswordResetCodeValidQuery : IAsyncRequest<PasswordResetCodeResponse>
    {
        public string Email { get; set; }
        public string PasswordResetCode { get; set; }
    }

    public class PasswordResetCodeResponse
    {
        public bool IsValid { get; set; }   

        public bool HasExpired { get; set; }
    }
}

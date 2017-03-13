using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetUserByHashedId
{
    public class GetUserByHashedIdQuery : IAsyncRequest<User>
    {
        public string HashedUserId { get; set; }
    }
}

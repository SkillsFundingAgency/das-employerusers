using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetUsers
{
    public class GetUsersQuery : IAsyncRequest<User[]>
    {
        public int PageSize  { get; set; }
        public int PageNumber { get; set; }
    }
}

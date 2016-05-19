using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data
{
    public class DocumentDbUserRepository : IUserRepository
    {
        public Task<User> GetById(string id)
        {
            return Task.FromResult<User>(null);
        }

        public Task Create(User registerUser)
        {
            return Task.FromResult<object>(null);
        }

        public Task Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}

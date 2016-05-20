using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data
{
    public class InMemoryPasswordProfileRepository : IPasswordProfileRepository
    {
        public Task<IEnumerable<PasswordProfile>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<PasswordProfile>>(new[]
            {
                new PasswordProfile
                {
                    Id = "b1fae38b-2325-4aa9-b0c3-3a31ef367210",
                    Key = Convert.ToBase64String(Encoding.ASCII.GetBytes("DZUvwHBMEdtGMi6CC@tRrFrcj7sJx[")),
                    WorkFactor = 10000,
                    SaltLength = 16,
                    StorageLength = 256
                }
            });
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer
{
    public class SqlServerPasswordProfileRepository : SqlServerRepository, IPasswordProfileRepository
    {
        public SqlServerPasswordProfileRepository() 
            : base("ProfileConnectionString")
        {
        }

        public async Task<IEnumerable<PasswordProfile>> GetAllAsync()
        {
            return await Query<PasswordProfile>("GetAllPasswordProfiles");
        }
    }
}

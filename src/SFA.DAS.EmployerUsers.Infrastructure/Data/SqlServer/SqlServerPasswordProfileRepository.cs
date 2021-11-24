using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer
{
    public class SqlServerPasswordProfileRepository : IPasswordProfileRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public SqlServerPasswordProfileRepository(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PasswordProfile>> GetAllAsync()
        {
            return await _unitOfWork.Query<PasswordProfile>("GetAllPasswordProfiles");
        }
    }
}

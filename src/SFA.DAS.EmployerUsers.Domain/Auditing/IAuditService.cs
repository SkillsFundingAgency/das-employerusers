using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Domain.Auditing
{
    public interface IAuditService
    {
        Task WriteAudit(EmployerUsersAuditMessage message);
    }
}
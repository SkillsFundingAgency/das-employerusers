using System.Collections.Generic;
using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing
{
    public class EmployerUsersAuditMessage
    {
        public List<PropertyUpdate> ChangedProperties { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public List<Entity> RelatedEntities { get; set; }
        public Entity AffectedEntity { get; set; }
    }
}
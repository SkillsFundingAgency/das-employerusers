using System.Linq;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application
{
    internal static class DomainObjectExtensions
    {
        internal static void ExpireSecurityCodesOfType(this User user, SecurityCodeType codeType)
        {
            user.SecurityCodes = user.SecurityCodes?.Where(sc => sc.CodeType != codeType).ToArray() ?? new SecurityCode[0];
        }

        internal static void AddSecurityCode(this User user, SecurityCode code)
        {
            user.SecurityCodes = user.SecurityCodes == null
                ? new[] { code }
                : user.SecurityCodes.Concat(new[] { code }).ToArray();
        }
    }
}

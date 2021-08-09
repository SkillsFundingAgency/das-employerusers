using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Extensions
{
    public static class SecurityCodeExtensions
    {
        public static SecurityCode MatchSecurityCode(this IEnumerable<SecurityCode> securityCodes, string passwordResetCode)
        {
            return securityCodes
                .OrderByDescending(sc => sc.ExpiryTime)
                .FirstOrDefault(sc => sc.Code.Equals(passwordResetCode, StringComparison.InvariantCultureIgnoreCase) && sc.CodeType == SecurityCodeType.PasswordResetCode);
        }

        public static SecurityCode LatestValidSecurityCode(this IEnumerable<SecurityCode> securityCodes)
        {
            return securityCodes
                .OrderByDescending(sc => sc.ExpiryTime)
                .Where(sc => sc.ExpiryTime >= DateTime.UtcNow)
                .FirstOrDefault(sc => sc.CodeType == SecurityCodeType.PasswordResetCode);
        }
    }
}

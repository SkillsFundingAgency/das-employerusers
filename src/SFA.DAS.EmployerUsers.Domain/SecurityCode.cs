using System;

namespace SFA.DAS.EmployerUsers.Domain
{
    public class SecurityCode
    {
        public string Code { get; set; }
        public SecurityCodeType CodeType { get; set; }
        public DateTime ExpiryTime { get; set; }
        public string ReturnUrl { get; set; }
        public string PendingValue { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is SecurityCode))
            {
                return false;
            }

            var securityCode = (SecurityCode)obj;
            return Code == securityCode.Code;
        }
        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
    }
}
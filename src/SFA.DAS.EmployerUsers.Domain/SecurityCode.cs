using System;

namespace SFA.DAS.EmployerUsers.Domain
{
    public class SecurityCode
    {
        public string Code { get; set; }
        public SecurityCodeType CodeType { get; set; }
        public DateTime ExpiryTime { get; set; }
        public string ReturnUrl { get; set; }
    }
}
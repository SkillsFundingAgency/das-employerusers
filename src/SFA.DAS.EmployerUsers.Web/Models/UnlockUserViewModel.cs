using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class UnlockUserViewModel
    {
        public string UnlockCode { get; set; }
        public string Email { get; set; }
        public bool Valid { get; set; }
        public bool UnlockCodeExpiry { get; set; }
    }
}
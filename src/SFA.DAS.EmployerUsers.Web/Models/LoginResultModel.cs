﻿namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class LoginResultModel
    {
        public bool Success { get; set; }
        public bool RequiresActivation { get; set; }
        public bool AccountIsLocked { get; set; }
        public bool AccountIsSuspended { get; set; }
    }
}
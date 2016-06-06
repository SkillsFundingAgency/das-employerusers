using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerUsers.Application
{
    public class InvalidRequestException : Exception
    {
        public  Dictionary<string,string> ErrorMessages { get; private set; }

        public InvalidRequestException(Dictionary<string,string> errorMessages)
        {
            this.ErrorMessages = errorMessages;
        }
    }
}

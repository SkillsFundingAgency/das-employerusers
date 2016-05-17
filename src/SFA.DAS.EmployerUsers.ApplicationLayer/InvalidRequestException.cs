using System;

namespace SFA.DAS.EmployerUsers.ApplicationLayer
{
    public class InvalidRequestException : ApplicationException
    {
        private string[] v;

        public InvalidRequestException(string[] v)
        {
            this.v = v;
        }
    }
}

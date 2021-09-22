using System;

namespace SFA.DAS.EmployerUsers.Application.Exceptions
{
    public class ExceededLimitPasswordResetCodeException : Exception
    {
        public ExceededLimitPasswordResetCodeException()
        {
        }

        public ExceededLimitPasswordResetCodeException(string message) : base(message)
        {
        }

        public ExceededLimitPasswordResetCodeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

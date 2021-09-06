using System;

namespace SFA.DAS.EmployerUsers.Application.Exceptions
{
    public class InvalidPasswordResetCodeException : Exception
    {
        public InvalidPasswordResetCodeException()
        {
        }

        public InvalidPasswordResetCodeException(string message) : base(message)
        {
        }

        public InvalidPasswordResetCodeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

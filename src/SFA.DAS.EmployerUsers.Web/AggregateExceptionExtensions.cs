using System;
using NLog;

namespace SFA.DAS.EmployerUsers.Web
{
    public static class AggregateExceptionExtensions
    {
        public static void UnpackAndLog(this AggregateException ex, ILogger logger)
        {
            foreach (var actualException in ex.InnerExceptions)
            {
                if (actualException is AggregateException)
                {
                    ex.UnpackAndLog(logger);
                }
                else
                {
                    logger.Error(actualException, actualException.Message);
                }
            }
        }
    }
}
using Microsoft.Extensions.Configuration;
using Serilog;

namespace BillingSystem.SharedKernel
{
    public static class AppExceptionHandler
    {
        public static void HandleException(this Exception exception, ILogger logger, IConfiguration configuration)
        {
            if (exception != null)
            {
                logger.Error($"exception occured {exception.Message} , stackTrace {exception.StackTrace}");
                if (exception.InnerException != null)
                {
                    logger.Error($"exception inner exception {exception.InnerException.Message} , stackTrace {exception.InnerException.StackTrace}");
                }
            }
        }
    }
}

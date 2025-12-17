using Microsoft.Extensions.Configuration;
using Serilog;

namespace BillingSystem.SharedKernel
{
    public class RetryUtil
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private int attempts = 0;
        private int maxAttempts = 3;
        private int pauseTimemiliseconds = 500;

        public RetryUtil(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public T ExecuteRetry<T>(Func<T> action)
        {
            try
            {
                attempts++;
                return action();
            }
            catch (Exception ex)
            {
                ex.HandleException(_logger, _configuration);
                if (attempts <= maxAttempts)
                {
                    Thread.Sleep(pauseTimemiliseconds);
                    return ExecuteRetry(action);
                }
                else
                {
                    throw;
                }
            }
        }

        public void ExecuteRetry(Action action)
        {
            try
            {
                attempts++;
                action();
            }
            catch (Exception ex)
            {
                ex.HandleException(_logger, _configuration);
                if (attempts <= maxAttempts)
                {
                    Thread.Sleep(pauseTimemiliseconds);
                    ExecuteRetry(action);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}

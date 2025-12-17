using Serilog;
using System.Net;

namespace BillingSystem.API.Middlewares
{
    public class CorrelationIdHeaderValidatorMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _logger;

        public CorrelationIdHeaderValidatorMiddleware(RequestDelegate next, Serilog.ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var idempotentKey = context.Request.Headers["x-idempotent-key"].FirstOrDefault();
            var correlationId = context.Request.Headers["x-Correlation-Id"].FirstOrDefault();

            if (string.IsNullOrEmpty(idempotentKey))
            {
                _logger.Warning("x-idempotent-key header not found in the request.");
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("x-idempotent-key header not found");
            }
            if (string.IsNullOrEmpty(correlationId))
            {
                _logger.Warning("x-Correlation-Id header not found in the request.");
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("x-Correlation-Id header not found");
            }

            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }

    }
}

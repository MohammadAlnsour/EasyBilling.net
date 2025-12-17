namespace BillingSystem.API.Middlewares
{
    public class CorrelationMiddleware
    {
        private const string Header = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId = context.Request.Headers[Header].FirstOrDefault() ?? Guid.NewGuid().ToString();

            context.Items[Header] = correlationId;
            context.Response.Headers[Header] = correlationId;

            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}

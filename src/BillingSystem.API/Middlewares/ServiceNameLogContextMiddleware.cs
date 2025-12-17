using System.Reflection;

namespace BillingSystem.API.Middlewares
{
    public class ServiceNameLogContextMiddleware
    {
        private readonly RequestDelegate _next;

        public ServiceNameLogContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            var serviceName = Assembly.GetEntryAssembly()?.GetName().Name;

            using (Serilog.Context.LogContext.PushProperty("ServiceName", serviceName))
            {
                await _next(context);
            }
        }
    }
}

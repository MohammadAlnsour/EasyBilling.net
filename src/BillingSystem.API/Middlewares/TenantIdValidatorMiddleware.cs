using BillingSystem.Application.Contracts.Services;
using System.Net;

namespace BillingSystem.API.Middlewares
{
    public class TenantIdValidatorMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public TenantIdValidatorMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
        {
            var tenantId = context.Request.Headers["x-tenant-id"].FirstOrDefault();
            if (string.IsNullOrEmpty(tenantId))
            {
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("x-tenant-id header not found");
            }
            if (Guid.TryParse(tenantId, out var id))
            {
                var tenant = await tenantService.GetTenantAsync(id);
                if (tenant != null)
                {
                    // Attach the tenant object to the HttpContext for later use.
                    // context.Items["Tenant"] = tenant;
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // Bad Request
                    await context.Response.WriteAsync("Invalid x-tenant-id header.");
                    return;
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // Bad Request
                await context.Response.WriteAsync("Invalid x-tenant-id header.");
                return;
            }
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }

    }
}

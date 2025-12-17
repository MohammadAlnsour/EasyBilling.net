using BillingSystem.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace BillingSystem.API.ActionFilters
{
    public class AuditBillingRequestsFilter : IAsyncActionFilter
    {
        private readonly IInvoiceAuditService _invoiceAuditService;

        public AuditBillingRequestsFilter(IInvoiceAuditService invoiceAuditService)
        {
            _invoiceAuditService = invoiceAuditService;
        }

        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    base.OnActionExecuting(context);
        //}

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var http = context.HttpContext;
            var idemKey = http.Request.Headers["x-idempotent-key"].FirstOrDefault();
            var client = http.User.Identity?.Name ?? "Unknown";

            http.Request.EnableBuffering();
            var body = string.Empty;
            using (var reader = new StreamReader(http.Request.Body, leaveOpen: true))
            {
                body = await reader.ReadToEndAsync();
                http.Request.Body.Position = 0; // reset
            }

            await _invoiceAuditService.LogRequestAsync(idemKey, client, body);

            // Continue pipeline
            var executedContext = await next();

            // Capture response
            var result = executedContext.Result as ObjectResult;
            string responsePayload = result != null
                ? JsonConvert.SerializeObject(result.Value)
                : "No response body";

            string status = executedContext.Exception == null ? "Success" : "Error";

            await _invoiceAuditService.LogResponseAsync(http.TraceIdentifier, idemKey, responsePayload, status);
        }
    }
}

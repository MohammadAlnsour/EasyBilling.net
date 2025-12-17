using BillingSystem.Application.Contracts.Services;
using Serilog;

namespace BillingSystem.Application.Services
{
    public class InvoiceAuditService : IInvoiceAuditService
    {
        private readonly ILogger _logger;

        public InvoiceAuditService(ILogger logger)
        {
            _logger = logger;
        }
        public Task LogRequestAsync(string idempotentKey, string client, string requestPayload)
        {
            //throw new NotImplementedException();
           return Task.FromResult("");
        }
        public Task LogResponseAsync(string requestId, string idempotentKey, string responsePayload, string status, long? invoiceId = null)
        {
            //  throw new NotImplementedException();
            return Task.FromResult("");
        }

    }
}

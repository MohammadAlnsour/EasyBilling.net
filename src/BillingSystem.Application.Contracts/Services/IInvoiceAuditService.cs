using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Application.Contracts.Services
{
    public interface IInvoiceAuditService
    {
        Task LogRequestAsync(string idempotentKey, string client, string requestPayload);
        Task LogResponseAsync(string requestId, string idempotentKey, string responsePayload, string status, long? invoiceId = null);
    }
}

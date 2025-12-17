using BillingSystem.Application.Responses;
using MediatR;

namespace BillingSystem.Application.Requests
{
    public class GetInvoicesByTenantRequest : IRequest<IEnumerable<GetInvoicesByTenantResponse>>
    {
        public int TenantId { get; set; }
    }
}

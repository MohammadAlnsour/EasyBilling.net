using MediatR;
using System.ComponentModel;

namespace BillingSystem.Application.Requests
{
    public class UpdateInvoiceRequest : IRequest<bool>
    {
        public long InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public int InvoiceType { get; set; }
        public int InvoiceStatus { get; set; } = 0;
    }
}

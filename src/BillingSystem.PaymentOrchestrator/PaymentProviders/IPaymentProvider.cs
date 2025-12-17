using BillingSystem.Domain.Entities;
using System.Net;

namespace BillingSystem.PaymentOrchestrator.PaymentProviders
{
    public interface IPaymentProvider
    {
        ProviderResponse SendInvoice(Invoice invoice);
    }
}

using System.Net;

namespace BillingSystem.PaymentOrchestrator.PaymentProviders
{
    public class ProviderResponse
    {
        public HttpStatusCode ProviderResponseStatusCode { get; set; }
        public string ProviderResponseMessage { get; set; }
        public string ProviderReferenceNumber { get; set; }
        public string ProviderName { get; set; }
    }
}

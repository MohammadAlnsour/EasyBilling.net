using BillingSystem.Domain.Entities;
using System.Net;

namespace BillingSystem.PaymentOrchestrator.PaymentProviders
{
    public class Tahseel : IPaymentProvider
    {
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;
        public Tahseel( Serilog.ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public ProviderResponse SendInvoice(Invoice invoice)
        {
            var providerRes = new ProviderResponse()
            {
                ProviderResponseStatusCode = HttpStatusCode.OK,
                ProviderResponseMessage = "success",
                ProviderName = "Tahseel",
                ProviderReferenceNumber = ""
            };
            return providerRes;
        }
    }
}

using BillingSystem.Domain.Entities;
using BillingSystem.Infrastructure.Integration;
using System.Net;

namespace BillingSystem.PaymentOrchestrator.PaymentProviders
{
    public class NPV : IPaymentProvider
    {
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        public NPV(Serilog.ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public ProviderResponse SendInvoice(Invoice invoice)
        {
            var providerRes = new ProviderResponse() 
            { 
                ProviderResponseStatusCode = HttpStatusCode.OK,
                ProviderResponseMessage="success",
                ProviderName = "NPV",
                ProviderReferenceNumber = ""
            };
            return providerRes;
        }
    }
}

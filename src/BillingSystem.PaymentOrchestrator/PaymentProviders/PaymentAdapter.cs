using BillingSystem.Domain.Entities;
using BillingSystem.Domain.enums;
using BillingSystem.Infrastructure.Integration;
using System.Net;

namespace BillingSystem.PaymentOrchestrator.PaymentProviders
{
    public class PaymentAdapter
    {
        private readonly IConfiguration _configuration;
        private readonly Serilog.ILogger _logger;
        private readonly Efaa _efaa;
        private readonly Tahseel _tahseel;
        private readonly NPV _nPV;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Dictionary<InvoiceTypes, IPaymentProvider> paymentProvidersMapping;


        public PaymentAdapter(
            IConfiguration configuration,
            Serilog.ILogger logger,
            Efaa efaa,
            Tahseel tahseel,
            NPV nPV,
            IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _efaa = efaa;
            _tahseel = tahseel;
            _nPV = nPV;
            _serviceScopeFactory = serviceScopeFactory;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var restClient = scope.ServiceProvider.GetRequiredService<IRESTClient>();

                paymentProvidersMapping = new Dictionary<InvoiceTypes, IPaymentProvider>()
                                        {
                                            { InvoiceTypes.SVInvoice,  _tahseel},
                                            { InvoiceTypes.VIInvoice,  _efaa },
                                            { InvoiceTypes.INInvoice,  _nPV }
                                        };
            }
        }

        public ProviderResponse SendInvoice(Invoice invoice)
        {
            var invoiceType = (InvoiceTypes)invoice.InvoiceType;
            paymentProvidersMapping.TryGetValue(invoiceType, out var provider);
            if (provider == null) { throw new Exception("Payment provider not found"); }
            return provider.SendInvoice(invoice);
        }

    }
}

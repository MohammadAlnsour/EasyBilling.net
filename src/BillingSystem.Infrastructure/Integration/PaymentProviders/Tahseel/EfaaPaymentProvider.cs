using Microsoft.Extensions.Configuration;
using Serilog;

namespace BillingSystem.Infrastructure.Integration.PaymentProviders.Tahseel
{
    public class TahseelPaymentProvider : IProvider
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IRESTClient _rESTClient;

        public TahseelPaymentProvider(ILogger logger, IConfiguration configuration, IRESTClient rESTClient)
        {
            _logger = logger;
            _configuration = configuration;
            _rESTClient = rESTClient;
        }
        public string Name => "تحصيل";
        public string Description => "تحصيل";

        public R PostInvoice<T, R>(T invoice)
        {
            throw new NotImplementedException();
        }
    }
}

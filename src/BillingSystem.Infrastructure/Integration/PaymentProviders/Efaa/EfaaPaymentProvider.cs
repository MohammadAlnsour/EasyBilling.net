using Microsoft.Extensions.Configuration;
using Serilog;

namespace BillingSystem.Infrastructure.Integration.PaymentProviders.Efaa
{
    public class EfaaPaymentProvider : IProvider
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IRESTClient _rESTClient;

        public EfaaPaymentProvider(ILogger logger, IConfiguration configuration, IRESTClient rESTClient)
        {
            _logger = logger;
            _configuration = configuration;
            _rESTClient = rESTClient;
        }
        public string Name => "ايفاء";
        public string Description => "ايفاء";

        public R PostInvoice<T, R>(T invoice)
        {
            throw new NotImplementedException();
        }
    }
}

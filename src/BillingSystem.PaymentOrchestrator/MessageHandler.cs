using BillingSystem.Domain.Entities;
using BillingSystem.Domain.enums;
using BillingSystem.Infrastructure.Persistence.Interfaces;
using BillingSystem.PaymentOrchestrator.PaymentProviders;
using BillingSystem.SharedKernel;
using ILogger = Serilog.ILogger;

namespace BillingSystem.PaymentOrchestrator
{
    public class MessageHandler
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly PaymentAdapter _paymentAdapter;

        public MessageHandler(ILogger logger, IConfiguration configuration, IServiceScopeFactory scopeFactory, PaymentAdapter paymentAdapter)
        {
            _logger = logger;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _paymentAdapter = paymentAdapter;
        }
        public void ProcessInvoiceMessage(Invoice invoice)
        {
            try
            {
                var providerRes = _paymentAdapter.SendInvoice(invoice);
                var status = providerRes.ProviderResponseStatusCode;
                int invoiceStatus = GetResponseStatus(status);

                using var scope = _scopeFactory.CreateScope();
                var _invoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();

                invoice.InvoiceStatus = invoiceStatus;
                invoice.PaymentReference = providerRes.ProviderReferenceNumber;
                invoice.ProviderResponse = providerRes.ProviderResponseMessage;
                invoice.PaymentProvider = providerRes.ProviderName;
                var result = _invoiceRepository.Update(invoice).Result;
            }
            catch (Exception ex)
            {
                ex.HandleException(_logger, _configuration);
                throw;
            }

        }

        private static int GetResponseStatus(System.Net.HttpStatusCode status)
        {
            var invoiceStatus = 0;

            switch (status)
            {
                case System.Net.HttpStatusCode.Continue:
                    break;
                case System.Net.HttpStatusCode.SwitchingProtocols:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.Processing:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.EarlyHints:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.OK:
                    invoiceStatus = (int)InvoiceStatus.PendingPayment;
                    break;
                case System.Net.HttpStatusCode.Created:
                    invoiceStatus = (int)InvoiceStatus.PendingPayment;
                    break;
                case System.Net.HttpStatusCode.Accepted:
                    invoiceStatus = (int)InvoiceStatus.PendingPayment;
                    break;
                case System.Net.HttpStatusCode.NonAuthoritativeInformation:
                    invoiceStatus = (int)InvoiceStatus.PendingPayment;
                    break;
                case System.Net.HttpStatusCode.NoContent:
                    invoiceStatus = (int)InvoiceStatus.PendingPayment;
                    break;
                case System.Net.HttpStatusCode.ResetContent:
                    invoiceStatus = (int)InvoiceStatus.PendingPayment;
                    break;
                case System.Net.HttpStatusCode.PartialContent:
                    invoiceStatus = (int)InvoiceStatus.PendingPayment;
                    break;
                case System.Net.HttpStatusCode.MultiStatus:
                    invoiceStatus = (int)InvoiceStatus.PendingPayment;
                    break;
                case System.Net.HttpStatusCode.AlreadyReported:
                    invoiceStatus = (int)InvoiceStatus.PendingPayment;
                    break;
                case System.Net.HttpStatusCode.IMUsed:
                    invoiceStatus = (int)InvoiceStatus.PendingPayment;
                    break;
                case System.Net.HttpStatusCode.Ambiguous:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.Moved:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.Found:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.RedirectMethod:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.NotModified:
                    invoiceStatus = (int)InvoiceStatus.PendingPayment;
                    break;
                case System.Net.HttpStatusCode.UseProxy:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.Unused:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.RedirectKeepVerb:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.PermanentRedirect:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.PaymentRequired:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.Forbidden:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.NotFound:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.MethodNotAllowed:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.NotAcceptable:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.ProxyAuthenticationRequired:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.RequestTimeout:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.Conflict:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.Gone:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.LengthRequired:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.PreconditionFailed:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.RequestEntityTooLarge:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.RequestUriTooLong:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.UnsupportedMediaType:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.RequestedRangeNotSatisfiable:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.ExpectationFailed:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.MisdirectedRequest:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.UnprocessableEntity:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.Locked:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.FailedDependency:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.UpgradeRequired:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.PreconditionRequired:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.TooManyRequests:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.RequestHeaderFieldsTooLarge:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.UnavailableForLegalReasons:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.InternalServerError:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.NotImplemented:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.BadGateway:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.ServiceUnavailable:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.GatewayTimeout:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.HttpVersionNotSupported:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.VariantAlsoNegotiates:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.InsufficientStorage:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.LoopDetected:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.NotExtended:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                case System.Net.HttpStatusCode.NetworkAuthenticationRequired:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
                default:
                    invoiceStatus = (int)InvoiceStatus.FailedSendingToProvider;
                    break;
            }

            return invoiceStatus;
        }
    }
}

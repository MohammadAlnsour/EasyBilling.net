using BillingSystem.Domain.Entities;
using BillingSystem.Infrastructure.ConfigOptions;
using BillingSystem.Infrastructure.Consul;
using BillingSystem.Infrastructure.Integration;
using BillingSystem.PaymentOrchestrator.PaymentProviders.IntegrationMappers;
using BillingSystem.PaymentOrchestrator.PaymentProviders.Requests;
using BillingSystem.SharedKernel;
using Consul;
using StackExchange.Redis;
using System.Net;
using ILogger = Serilog.ILogger;

namespace BillingSystem.PaymentOrchestrator.PaymentProviders
{
    public class Efaa : IPaymentProvider
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDatabase _redisDb;
        private readonly string IntegrationSettingsConsulKey = "BillingService/IntegrationService";

        public Efaa(ILogger logger, IConfiguration configuration, IServiceScopeFactory scopeFactory, IDatabase redisDb)
        {
            _logger = logger;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _redisDb = redisDb;
        }

        public ProviderResponse SendInvoice(Invoice invoice)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var _rESTClient = scope.ServiceProvider.GetRequiredService<IRESTClient>();
                var consulClient = scope.ServiceProvider.GetRequiredService<IConsulClient>();
                var integrationServiceConfig = ConsulHelper.ReadKeyValueFromConsulAsync<IntegrationServiceSettings>(consulClient, IntegrationSettingsConsulKey, _logger, _configuration, _redisDb).Result;
                var providerRes = new ProviderResponse() { ProviderName = "Effaa" };

                var mappedFine = InvoiceToNPVFine.Map(invoice);
                var apiCallResponse = _rESTClient.PostAsync<NPVFineRequest, ApiResponse<string>>(mappedFine, integrationServiceConfig.Base, integrationServiceConfig.NPVURl).Result;
                if (apiCallResponse != null && apiCallResponse.IsSucceed)
                {
                    providerRes.ProviderResponseStatusCode = HttpStatusCode.OK;
                    providerRes.ProviderResponseMessage = apiCallResponse.Message;
                    providerRes.ProviderReferenceNumber = apiCallResponse.Data;
                }
                else if (apiCallResponse != null && !apiCallResponse.IsSucceed)
                {
                    providerRes.ProviderResponseStatusCode = HttpStatusCode.InternalServerError;
                    providerRes.ProviderResponseMessage = apiCallResponse.Message;
                    providerRes.ProviderReferenceNumber = apiCallResponse.Data;
                }
                else if (apiCallResponse == null)
                {
                    providerRes.ProviderResponseStatusCode = HttpStatusCode.InternalServerError;
                    providerRes.ProviderResponseMessage = "Error occured contacting provider.";
                    providerRes.ProviderReferenceNumber = string.Empty;
                }
                else
                {
                    providerRes.ProviderResponseStatusCode = HttpStatusCode.InternalServerError;
                    providerRes.ProviderResponseMessage = "Error occured contacting provider.";
                    providerRes.ProviderReferenceNumber = string.Empty;
                }

                return providerRes;
            }
            catch (Exception ex)
            {
                ex.HandleException(_logger, _configuration);
                throw;
            }
        }
    }
}

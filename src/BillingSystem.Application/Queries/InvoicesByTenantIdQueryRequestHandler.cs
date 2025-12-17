using AutoMapper;
using BillingSystem.Application.Requests;
using BillingSystem.Application.Responses;
using BillingSystem.Infrastructure.Persistence.Interfaces;
using BillingSystem.SharedKernel;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace BillingSystem.Application.Queries
{
    public class InvoicesByTenantIdQueryRequestHandler : IRequestHandler<GetInvoicesByTenantRequest, IEnumerable<GetInvoicesByTenantResponse>>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public InvoicesByTenantIdQueryRequestHandler(
            ILogger logger,
            IConfiguration configuration,
            IInvoiceRepository invoiceRepository,
            IMapper mapper)
        {
            _logger = logger;
            _configuration = configuration;
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<GetInvoicesByTenantResponse>> Handle(GetInvoicesByTenantRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.TenantId <= 0)
                throw new Exception("Tenant Id should be more than 0");

            try
            {
                var tenantInvoices = await _invoiceRepository.Query(i => i.TenantId == request.TenantId);
                var response = _mapper.Map<List<GetInvoicesByTenantResponse>>(tenantInvoices);
                return response;
            }
            catch (Exception ex)
            {
                ex.HandleException(_logger, _configuration);
                throw;
            }
        }
    }
}

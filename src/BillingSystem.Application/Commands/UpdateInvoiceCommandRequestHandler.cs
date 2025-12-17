using AutoMapper;
using BillingSystem.Application.Contracts.Services;
using BillingSystem.Application.Requests;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.enums;
using BillingSystem.Infrastructure.Persistence.Interfaces;
using BillingSystem.SharedKernel;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace BillingSystem.Application.Commands
{
    public class UpdateInvoiceCommandRequestHandler : IRequestHandler<UpdateInvoiceRequest, bool>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceNumberGeneratorService _invNumService;
        private readonly IMapper _mapper;

        public UpdateInvoiceCommandRequestHandler(
            ILogger logger,
            IConfiguration configuration,
            IInvoiceRepository invoiceRepository,
            IInvoiceNumberGeneratorService invoiceNumberGeneratorService,
            IMapper mapper)
        {
            _logger = logger;
            _configuration = configuration;
            _invoiceRepository = invoiceRepository;
            _invNumService = invoiceNumberGeneratorService;
            _mapper = mapper;
        }
        public async Task<bool> Handle(UpdateInvoiceRequest request, CancellationToken cancellationToken)
        {
            var requestInvoice = _mapper.Map<Invoice>(request);
            var dbInvoice = await _invoiceRepository.Get((int)request.InvoiceId);

            if ((InvoiceStatus)dbInvoice.InvoiceStatus == InvoiceStatus.PaymentSucceed
                || (InvoiceStatus)dbInvoice.InvoiceStatus == InvoiceStatus.PartiallyPayed)
                return false;

            var changes = ReflectionAuditHelper.GetDifferences(dbInvoice, requestInvoice);
            var serializedChanges = JsonConvert.SerializeObject(changes);
            var auditEvent = new AuditEvent
            {
                UserId = dbInvoice.TenantId.ToString(),
                EntityType = EntityTypes.Invoices.ToString(),
                EntityId = string.Empty,
                Action = EventsTypes.InvoiceUpdated.ToString(),
                Description = $"Invoice updated",
                Changes = serializedChanges,
                CreatedBy = dbInvoice.TenantId.ToString(),
                CreatedDate = DateTime.UtcNow
            };
            var issuccess = await _invoiceRepository.Update(requestInvoice);
            return issuccess;
        }
    }
}

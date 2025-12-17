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
using System.Collections.Generic;

namespace BillingSystem.Application.Commands
{
    public class CreateInvoiceCommandRequestHandler : IRequestHandler<CreateInvoiceRequest, Int64>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceNumberGeneratorService _invNumService;
        private readonly IMapper _mapper;

        public CreateInvoiceCommandRequestHandler(
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
        public async Task<Int64> Handle(CreateInvoiceRequest request, CancellationToken cancellationToken)
        {
            var invoiceItems = _mapper.Map<List<InvoicesItem>>(request.Items);
            var invoiceAttachs = _mapper.Map<List<InvoicesAttachment>>(request.Attachments);
            var dbInvoice = _mapper.Map<Invoice>(request);
            dbInvoice.InvoicesItems = invoiceItems;
            dbInvoice.InvoicesAttachments = invoiceAttachs;
            var invoiceNumber = string.Empty;

            try
            {
                invoiceNumber = await _invNumService.GenerateInvoiceNumber((InvoiceTypes)dbInvoice.InvoiceType);
                dbInvoice.InvoiceNumber = invoiceNumber;
            }
            catch (Exception ex)
            {
                ex.HandleException(_logger, _configuration);
                throw;
            }

            var changes = ReflectionAuditHelper.GetDifferences(dbInvoice, dbInvoice);
            var serializedChanges = JsonConvert.SerializeObject(changes);
            var auditEvent = new AuditEvent
            {
                UserId = request.TenantId.ToString(),
                EntityType = EntityTypes.Invoices.ToString(),
                EntityId = string.Empty,
                Action = EventsTypes.InvoiceCreated.ToString(),
                Description = $"Invoice created",
                Changes = serializedChanges,
                CreatedBy = request.TenantId.ToString(),
                CreatedDate = DateTime.UtcNow
            };
            var invoiceId = await _invoiceRepository.InsertTransactional(dbInvoice, auditEvent);
            return invoiceId;
        }
    }
}

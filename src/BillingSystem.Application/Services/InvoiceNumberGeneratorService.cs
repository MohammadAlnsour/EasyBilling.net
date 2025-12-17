using BillingSystem.Application.Contracts.Services;
using BillingSystem.Domain.enums;
using BillingSystem.Infrastructure.Persistence.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace BillingSystem.Application.Services
{
    public class InvoiceNumberGeneratorService : IInvoiceNumberGeneratorService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public InvoiceNumberGeneratorService(IInvoiceRepository invoiceRepository, ILogger logger, IConfiguration configuration)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<string> GenerateInvoiceNumber(InvoiceTypes invoiceType)
        {
            var invoiceShortCode = string.Empty;
            switch (invoiceType)
            {
                case InvoiceTypes.SVInvoice:
                    invoiceShortCode = "SV";
                    break;
                case InvoiceTypes.VIInvoice:
                    invoiceShortCode = "VI";
                    break;
                case InvoiceTypes.INInvoice:
                    invoiceShortCode = "IN";
                    break;
                default:
                    invoiceShortCode = "IN";
                    break;
            }

            var invoiceNumber = string.Empty;
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month.ToString("00");
            var currentDay = DateTime.Now.Day.ToString("00");
            //var invoiceSeqNumberSixNumbers = string.Empty;
            //try
            //{
            //    var invoiceSeqNumber = await _invoiceRepository.GetNextInvoiceNumberAsync();
            //    invoiceSeqNumberSixNumbers = invoiceSeqNumber.ToString("000000");
            //}
            //catch (Exception ex)
            //{
            //    ex.HandleException(_logger, _configuration);
            //    throw;
            //}
            invoiceNumber = $"{invoiceShortCode}{currentYear}{currentMonth}{currentDay}";
            return invoiceNumber;
        }
    }
}


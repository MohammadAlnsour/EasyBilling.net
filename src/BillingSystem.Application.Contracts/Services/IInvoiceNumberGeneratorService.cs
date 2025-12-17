using BillingSystem.Domain.enums;

namespace BillingSystem.Application.Contracts.Services
{
    public interface IInvoiceNumberGeneratorService
    {
        Task<string> GenerateInvoiceNumber(InvoiceTypes invoiceType);
    }
}

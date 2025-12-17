using BillingSystem.Domain.enums;
using MediatR;

namespace BillingSystem.Application.Requests
{
    public class CreateInvoiceRequest : IRequest<Int64>
    {
        public long Id { get; set; }
        public Guid ExternalId { get; set; }
        public int TenantId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public int InvoiceType { get; set; }
        public int InvoiceStatus { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueAt { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool? IsSubInvoice { get; set; }
        public string? CancelationReasons { get; set; }
         public string? ClientReferenceNumber { get; set; }
        public string? Metadata { get; set; }
        public string? CorrelationId { get; set; }

        public InvoiceCustomer Customer { get; set; }
        public List<InvoiceAttachments>? Attachments { get; set; }
        public List<InvoiceItems>? Items { get; set; }
    }

    public class InvoiceCustomer
    {
        public string FullName { get; set; } = null!;
        public CustomerTypes CustomerType { get; set; } = CustomerTypes.Person;
        public CustomerAddress Address { get; set; }
        public string IdentityNumber { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? Vatnumber { get; set; }
        public IdentityTypes IdentityType { get; set; }
    }
    public class CustomerAddress
    {
        public string AddressCountry { get; set; } = null!;
        public string AddressCity { get; set; } = null!;
        public string AddressPostalCode { get; set; } = null!;
        public string AddressDistrict { get; set; } = null!;
        public string? AddressStreet { get; set; }
        public string? AddressBuildingNumber { get; set; }
        public string? AddressSubNumber { get; set; }
    }
    public class InvoiceAttachments
    {
        public string FileName { get; set; } = null!;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
    public class InvoiceItems
    {
        public string ItemName { get; set; } = null!;
        public string ItemDesc { get; set; } = null!;
        public decimal Amount { get; set; }
        public int? Quantity { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

}

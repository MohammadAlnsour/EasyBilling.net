using BillingSystem.Domain.Entities;

namespace BillingSystem.Application.Responses
{
    public class GetInvoicesByTenantResponse
    {
        public long Id { get; set; }
        public Guid ExternalId { get; set; }
        public int TenantId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public int InvoiceType { get; set; }
        public int InvoiceStatus { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime? DueAt { get; set; }
        public string? PaymentReference { get; set; }
        public string? PaymentProvider { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public bool? IsSubInvoice { get; set; }
        public string? CancelationReasons { get; set; }
        public bool? IsSubstitution { get; set; }
        public long? SubstitutionPrimaryInvoiceId { get; set; }
        public bool? IsDiscounted { get; set; }
        public long? DiscountedPrimaryInvoiceId { get; set; }
        public string? ClientReferenceNumber { get; set; }
        public string? Metadata { get; set; }
        public long CustomerId { get; set; }
        public string? ProviderResponse { get; set; }
        public virtual GetInvoicesByTenantCustomer Customer { get; set; } = null!;
        public virtual ICollection<InvoicesAttachment> InvoicesAttachments { get; set; } = new List<InvoicesAttachment>();
        public virtual ICollection<InvoicesItem> InvoicesItems { get; set; } = new List<InvoicesItem>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual Tenant Tenant { get; set; } = null!;
    }

    public class GetInvoicesByTenantCustomer
    {
        public long Id { get; set; }
        public string FullName { get; set; } = null!;
        public string CustomerType { get; set; } = null!;
        public string AddressCountry { get; set; } = null!;
        public string AddressCity { get; set; } = null!;
        public string AddressPostalCode { get; set; } = null!;
        public string AddressDistrict { get; set; } = null!;
        public string? AddressStreet { get; set; }
        public string? AddressBuildingNumber { get; set; }
        public string? AddressSubNumber { get; set; }
        public string IdentityNumber { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? Vatnumber { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public int? IdentityType { get; set; }
        public decimal? OverpaymentBalance { get; set; }
    }
}

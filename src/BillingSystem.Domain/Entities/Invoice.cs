using BillingSystem.Domain.Contracts;

namespace BillingSystem.Domain.Entities
{
    public partial class Invoice : IAggregateRoot
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
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public bool? IsSubInvoice { get; set; }
        public long? PrimaryInvoiceId { get; set; }
        public string? CancelationReasons { get; set; }
        public bool? IsSubstitution { get; set; }
        public long? SubstitutionPrimaryInvoiceId { get; set; }
        public bool? IsDiscounted { get; set; }
        public long? DiscountedPrimaryInvoiceId { get; set; }
        public string? ClientReferenceNumber { get; set; }
        public string? Metadata { get; set; }
        public long CustomerId { get; set; }
        public string? ProviderResponse { get; set; }
        public virtual Customer Customer { get; set; } = null!;
        public virtual Invoice? DiscountedPrimaryInvoice { get; set; }
        public virtual ICollection<Invoice> InverseDiscountedPrimaryInvoice { get; set; } = new List<Invoice>();
        public virtual ICollection<Invoice> InversePrimaryInvoice { get; set; } = new List<Invoice>();
        public virtual ICollection<Invoice> InverseSubstitutionPrimaryInvoice { get; set; } = new List<Invoice>();
        public virtual ICollection<InvoicesAttachment> InvoicesAttachments { get; set; } = new List<InvoicesAttachment>();
        public virtual ICollection<InvoicesItem> InvoicesItems { get; set; } = new List<InvoicesItem>();
        public virtual ICollection<InvoicesLog> InvoicesLogs { get; set; } = new List<InvoicesLog>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual Invoice? PrimaryInvoice { get; set; }
        public virtual Invoice? SubstitutionPrimaryInvoice { get; set; }
        public virtual Tenant Tenant { get; set; } = null!;
    }
}

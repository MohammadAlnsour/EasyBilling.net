using BillingSystem.Domain.Contracts;

namespace BillingSystem.Domain.Entities
{
    public partial class InvoicesLog
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
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
        public DateTime LastUpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public virtual Invoice Invoice { get; set; } = null!;
        public virtual Tenant Tenant { get; set; } = null!;
    }
}

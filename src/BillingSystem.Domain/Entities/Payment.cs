using BillingSystem.Domain.Contracts;

namespace BillingSystem.Domain.Entities
{
    public partial class Payment : IAggregateRoot
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public string Provider { get; set; } = null!;
        public int Attempts { get; set; }
        public string? ProviderReference { get; set; }
        public string? LastError { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ProviderResponse { get; set; }
        public virtual Invoice Invoice { get; set; } = null!;
    }
}

using BillingSystem.Domain.Contracts;

namespace BillingSystem.Domain.Entities
{
    public partial class Tenant : IAggregateRoot
    {
        public int Id { get; set; }
        public string TenantName { get; set; } = null!;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int TenantStatus { get; set; }
        public bool IsDeleted { get; set; }
        public Guid TenantGuid { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public virtual ICollection<InvoicesLog> InvoicesLogs { get; set; } = new List<InvoicesLog>();
    }
}

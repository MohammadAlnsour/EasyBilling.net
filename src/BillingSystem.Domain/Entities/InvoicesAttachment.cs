using BillingSystem.Domain.Contracts;

namespace BillingSystem.Domain.Entities
{
    public partial class InvoicesAttachment : IEntity
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public string FileName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public virtual Invoice Invoice { get; set; } = null!;
    }
}

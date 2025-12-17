namespace BillingSystem.Domain.Entities
{
    public partial class InvoicesItem
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public string ItemName { get; set; } = null!;
        public string ItemDesc { get; set; } = null!;
        public decimal Amount { get; set; }
        public int? Quantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public virtual Invoice Invoice { get; set; } = null!;
    }
}

using BillingSystem.Domain.Contracts;

namespace BillingSystem.Domain.Entities
{
    public partial class ProductsPrice : IAggregateRoot
    {
        public long Id { get; set; }
        public string TypeName { get; set; } = null!;
        public string ReferenceNumber { get; set; } = null!;
        public decimal MinimumAmount { get; set; }
        public decimal MaximumAmount { get; set; }
        public bool? AllowTax { get; set; }
        public bool? IsViolationProduct { get; set; }
        public bool? IsServiceProduct { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public bool IsEnabled { get; set; }
    }
}

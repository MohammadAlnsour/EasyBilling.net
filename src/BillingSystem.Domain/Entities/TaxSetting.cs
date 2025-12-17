using BillingSystem.Domain.Contracts;

namespace BillingSystem.Domain.Entities
{
    public partial class TaxSetting : IAggregateRoot
    {
        public long Id { get; set; }
        public decimal TaxPercentage { get; set; }
        public string TaxName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public bool IsEnabled { get; set; }
    }
}

using BillingSystem.Domain.Contracts;

namespace BillingSystem.Domain.Entities
{
    public class AuditEvent : IAggregateRoot
    {
        public long Id { get; set; }
        public string UserId { get; set; } = null!;
        public string EntityType { get; set; } = null!;
        public string EntityId { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string? Description { get; set; }
        public string Changes { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
    }
}

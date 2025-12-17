using BillingSystem.Domain.Contracts;

namespace BillingSystem.Domain.Entities
{
    public class OutboxEvent : IAggregateRoot
    {
        public long Id { get; set; }
        public long AggregateId { get; set; }
        public string EventType { get; set; } = null!;
        public string? Payload { get; set; }
        public bool Processed { get; set; }
        public string? ProcessingStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ErrorMsg { get; set; }
        public string? CorrelationId { get; set; }
    }
}

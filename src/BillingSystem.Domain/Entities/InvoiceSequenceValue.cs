using BillingSystem.Domain.Contracts;

namespace BillingSystem.Domain.Entities
{
    public class InvoiceSequenceValue : IAggregateRoot
    {
        public long Value { get; set; }
    }
}

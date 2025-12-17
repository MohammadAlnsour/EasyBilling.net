using BillingSystem.Domain.Entities;

namespace BillingSystem.Application.Contracts.Services
{
    public interface IOutboxeventsService
    {
        Task UpdateOutboxEventProcessed(OutboxEvent outboxEvent);
        Task<IEnumerable<OutboxEvent>> FetchBulkUnProcessedOutboxEvents(int bulkSize);
        Task UpdateOutboxEventError(OutboxEvent outboxEvent);
    }
}

using BillingSystem.Domain.Entities;

namespace BillingSystem.Infrastructure.Persistence.Interfaces
{
    public interface IOutboxEventsRepository : IRepository<OutboxEvent>
    {
        Task<IEnumerable<OutboxEvent>> GetBulkWorkerAsync(int count);
    }
}

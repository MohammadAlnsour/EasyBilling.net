using BillingSystem.Domain.Entities;

namespace BillingSystem.Infrastructure.Persistence.Interfaces
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<bool> ExistIdempotentKey(string idempotentKey);
        Task<long> InsertTransactional(Invoice entity, AuditEvent auditEvent);
    }
}

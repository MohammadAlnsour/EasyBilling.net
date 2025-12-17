using BillingSystem.Domain.Entities;

namespace BillingSystem.Application.Contracts.Services
{
    public interface ITenantService
    {
        Task<Tenant> GetTenantAsync(Guid tenantId);
    }
}

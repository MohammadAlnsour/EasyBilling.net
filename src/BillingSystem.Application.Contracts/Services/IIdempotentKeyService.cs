namespace BillingSystem.Application.Contracts.Services
{
    public interface IIdempotentKeyService
    {
        Task<bool> ExistsAsync(string externalKey);
    }
}

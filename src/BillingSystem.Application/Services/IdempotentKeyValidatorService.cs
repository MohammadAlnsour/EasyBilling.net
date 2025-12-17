using BillingSystem.Application.Contracts.Services;
using BillingSystem.Infrastructure.Persistence.Interfaces;
using StackExchange.Redis;

namespace BillingSystem.Application.Services
{
    public class IdempotentKeyValidatorService : IIdempotentKeyService
    {
        private readonly TimeSpan _ttl = TimeSpan.FromDays(2);
        private readonly IDatabase _redisDb;
        private readonly IInvoiceRepository _invoiceRepository;

        public IdempotentKeyValidatorService(IDatabase redisDb, IInvoiceRepository invoiceRepository)
        {
            _redisDb = redisDb;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<bool> ExistsAsync(string externalKey)
        {
            var redisKey = $"idempotent-{externalKey}";
            if (await _redisDb.KeyExistsAsync(externalKey)) return true;

            var existInDb = false;
            //todo : inject db repository and check db invoices table.
            existInDb = await _invoiceRepository.ExistIdempotentKey(externalKey);
            if (!existInDb) await _redisDb.StringSetAsync(redisKey, "1", _ttl);

            return existInDb;
        }
    }
}

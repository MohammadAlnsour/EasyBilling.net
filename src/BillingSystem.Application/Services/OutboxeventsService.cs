using BillingSystem.Application.Contracts.Services;
using BillingSystem.Domain.Entities;
using BillingSystem.Infrastructure.Persistence.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;
using StackExchange.Redis;

namespace BillingSystem.Application.Services
{
    public class OutboxeventsService : IOutboxeventsService
    {
        private readonly IOutboxEventsRepository _outboxRepository;
        private readonly IDatabase _redisDb;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public OutboxeventsService(IOutboxEventsRepository outboxRepository, IDatabase redisDb, ILogger logger, IConfiguration configuration)
        {
            _outboxRepository = outboxRepository;
            _redisDb = redisDb;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<OutboxEvent>> FetchBulkUnProcessedOutboxEvents(int bulkSize)
        {
            //return _outboxRepository.Query(e => e.Processed == false).OrderBy(e => e.Id).Take(bulkSize).ToList();
            return await _outboxRepository.GetBulkWorkerAsync(bulkSize);
        }

        public async Task UpdateOutboxEventProcessed(OutboxEvent outboxEvent)
        {
            if (outboxEvent != null && !outboxEvent.Processed) outboxEvent.Processed = true;
            await _outboxRepository.Update(outboxEvent);
        }

        public async Task UpdateOutboxEventError(OutboxEvent outboxEvent)
        {
            await _outboxRepository.Update(outboxEvent);
        }

    }
}

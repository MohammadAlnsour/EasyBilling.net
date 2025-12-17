using BillingSystem.Domain.Entities;
using BillingSystem.Infrastructure.EF;
using BillingSystem.Infrastructure.Persistence.Interfaces;
using BillingSystem.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace BillingSystem.Infrastructure.Persistence.EF
{
    public class OutboxEventsRepository : IOutboxEventsRepository
    {
        private readonly BillingServiceDBContext _billingServiceDB;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public OutboxEventsRepository(BillingServiceDBContext billingServiceDB, IConfiguration configuration, ILogger logger)
        {
            _billingServiceDB = billingServiceDB;
            _configuration = configuration;
            _logger = logger;
        }

        public Task<bool> Delete(OutboxEvent entity)
        {
            throw new NotImplementedException();
        }

        public async Task<long> Insert(OutboxEvent entity)
        {
            _billingServiceDB.OutboxEvents.Add(entity);
            await _billingServiceDB.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<IEnumerable<OutboxEvent>> Query(Func<OutboxEvent, bool> query)
        {
            return _billingServiceDB.OutboxEvents.Where(query).ToList();
        }

        public async Task<bool> Update(OutboxEvent entity)
        {
            try
            {
                var dbOutboxEvent = _billingServiceDB.OutboxEvents.SingleOrDefault(p => p.Id == entity.Id);

                if (dbOutboxEvent != null)
                {
                    if (entity.AggregateId < 0) entity.AggregateId = dbOutboxEvent.AggregateId;
                    if (string.IsNullOrEmpty(entity.EventType)) entity.EventType = dbOutboxEvent.EventType;
                    if (string.IsNullOrEmpty(entity.Payload)) entity.Payload = dbOutboxEvent.Payload;
                    _billingServiceDB.Entry(dbOutboxEvent).CurrentValues.SetValues(entity);
                    await _billingServiceDB.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.HandleException(_logger, _configuration);
                return false;
            }
        }

        public async Task<OutboxEvent> Get(int id)
        {
            return await _billingServiceDB.OutboxEvents.FirstOrDefaultAsync(t => t.Id == id);
        }

        public Task<IEnumerable<OutboxEvent>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OutboxEvent>> GetPaged(int pageNumber, int pageSize)
        {
            return await _billingServiceDB.OutboxEvents
                .OrderByDescending(t => t.Id)
                .Skip((pageNumber * pageSize) - 1)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<OutboxEvent>> GetBulkWorkerAsync(int count)
        {
            try
            {
                var parameterValue = count;
                var results = await _billingServiceDB.OutboxEvents
                    .FromSqlInterpolated($"EXECUTE dbo.GetNextPendingRowsForProcessing {parameterValue}")
                    .ToListAsync();

                return results;
            }
            catch (Exception ex)
            {
                ex?.HandleException(_logger, _configuration);
                throw;
            }

        }
    }
}

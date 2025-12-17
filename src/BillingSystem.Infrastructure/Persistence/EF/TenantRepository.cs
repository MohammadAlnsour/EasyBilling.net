using BillingSystem.Domain.Entities;
using BillingSystem.Infrastructure.EF;
using BillingSystem.Infrastructure.Persistence.Interfaces;
using BillingSystem.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace BillingSystem.Infrastructure.Persistence.EF
{
    public class TenantRepository : ITenantRepository
    {
        private readonly BillingServiceDBContext _billingServiceDB;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public TenantRepository(BillingServiceDBContext billingServiceDB, IConfiguration configuration, ILogger logger)
        {
            _billingServiceDB = billingServiceDB;
            _configuration = configuration;
            _logger = logger;
        }
        public Task<bool> Delete(Tenant entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Tenant> Get(int id)
        {
            return await _billingServiceDB.Tenants.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Tenant>> GetAll()
        {
            return await _billingServiceDB.Tenants.ToListAsync();
        }

        public async Task<IEnumerable<Tenant>> GetPaged(int pageNumber, int pageSize)
        {
            return await _billingServiceDB.Tenants
                .OrderByDescending(t => t.Id)
                .Skip((pageNumber * pageSize) - 1)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<long> Insert(Tenant entity)
        {
            _billingServiceDB.Tenants.Add(entity);
            await _billingServiceDB.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<IEnumerable<Tenant>> Query(Func<Tenant, bool> query)
        {
            return _billingServiceDB.Tenants.Where(query).ToList();
        }

        public async Task<bool> Update(Tenant entity)
        {
            try
            {
                var dbTenant = _billingServiceDB.Tenants.SingleOrDefault(p => p.Id == entity.Id);

                if (dbTenant != null)
                {
                    _billingServiceDB.Entry(dbTenant).CurrentValues.SetValues(entity);
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
    }
}

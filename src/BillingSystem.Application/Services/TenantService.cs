using BillingSystem.Application.Contracts.Services;
using BillingSystem.Domain.Entities;
using BillingSystem.Infrastructure.Persistence.Interfaces;
using BillingSystem.SharedKernel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;

namespace BillingSystem.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IDatabase _redisDb;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private const string TenantsRedisKeyName = "GacBillingTenants";

        public TenantService(ITenantRepository tenantRepository, IDatabase redisDb, ILogger logger, IConfiguration configuration)
        {
            _tenantRepository = tenantRepository;
            _redisDb = redisDb;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<Tenant> GetTenantAsync(Guid tenantId)
        {
            try
            {
                var tenantsList = await _redisDb.StringGetAsync(TenantsRedisKeyName);
                if (string.IsNullOrEmpty(tenantsList))
                {
                    var dbTenants = await _tenantRepository.GetAll();
                    var serializedTenants = JsonConvert.SerializeObject(dbTenants);
                    _redisDb.StringSet(TenantsRedisKeyName, serializedTenants);
                    var tenant = dbTenants.FirstOrDefault(t => t.TenantGuid == tenantId);
                    return tenant;
                }
                else
                {
                    var cachedTenants = JsonConvert.DeserializeObject<IEnumerable<Tenant>>(tenantsList);
                    var tenant = cachedTenants.FirstOrDefault(t => t.TenantGuid == tenantId);
                    return tenant;
                }
            }
            catch (Exception ex)
            {
                ex.HandleException(_logger, _configuration);
                throw;
            }
        }
    }
}

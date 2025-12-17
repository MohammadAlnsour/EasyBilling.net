using BillingSystem.Domain.Entities;
using BillingSystem.Domain.enums;
using BillingSystem.Infrastructure.EF;
using BillingSystem.Infrastructure.Persistence.Interfaces;
using BillingSystem.SharedKernel;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace BillingSystem.Infrastructure.Persistence.EF
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly BillingServiceDBContext _billingServiceDbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public InvoiceRepository(BillingServiceDBContext billingServiceDB, IConfiguration configuration, ILogger logger)
        {
            _billingServiceDbContext = billingServiceDB;
            _configuration = configuration;
            _logger = logger;
        }
        public Task<bool> Delete(Invoice entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistIdempotentKey(string idempotentKey)
        {
            return await _billingServiceDbContext.Invoices.AsNoTracking().AnyAsync(inv => inv.ExternalId.ToString() == idempotentKey);
        }

        public Task<Invoice> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Invoice>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Invoice>> GetPaged(int pageNumber, int pageSize)
        {
            return await _billingServiceDbContext.Invoices
                .Include(inv => inv.Customer)
                .Include(inv => inv.InvoicesItems)
                .Include(inv => inv.InvoicesAttachments)
                .OrderByDescending(t => t.Id)
                .Skip((pageNumber * pageSize) - 1)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<long> InsertTransactional(Invoice entity, AuditEvent auditEvent)
        {
            using (var transaction = _billingServiceDbContext.Database.BeginTransaction())
            {
                try
                {
                    var nextInvoiceNumber = _billingServiceDbContext.InvoiceSequenceValues
                        .FromSqlRaw("EXECUTE dbo.GetNextInvoiceNumber")
                        .AsEnumerable()
                        .FirstOrDefault();

                    if (nextInvoiceNumber == null)
                        throw new InvalidOperationException("Could not retrieve next sequence value.");

                    entity.InvoiceNumber += nextInvoiceNumber.Value.ToString("000000");
                    entity.CreatedBy = entity.TenantId.ToString();

                    if (entity.Customer != null)
                    {
                        var cutomerIdentityNumber = entity.Customer.IdentityNumber;
                        var dbCustomer = _billingServiceDbContext.Customers.FirstOrDefault(c => c.IdentityNumber == cutomerIdentityNumber);
                        if (dbCustomer != null)
                        {
                            dbCustomer.AddressSubNumber = entity.Customer.AddressSubNumber;
                            dbCustomer.Vatnumber = entity.Customer.Vatnumber;
                            dbCustomer.AddressBuildingNumber = entity.Customer.AddressBuildingNumber;
                            dbCustomer.AddressCity = entity.Customer.AddressCity;
                            dbCustomer.AddressCountry = entity.Customer.AddressCountry;
                            dbCustomer.AddressDistrict = entity.Customer.AddressDistrict;
                            dbCustomer.AddressPostalCode = entity.Customer.AddressPostalCode;
                            dbCustomer.AddressStreet = entity.Customer.AddressStreet;
                            dbCustomer.EmailAddress = entity.Customer.EmailAddress;
                            dbCustomer.FullName = entity.Customer.FullName;
                            dbCustomer.PhoneNumber = entity.Customer.PhoneNumber;
                            dbCustomer.CustomerType = entity.Customer.CustomerType;
                            dbCustomer.IdentityType = entity.Customer.IdentityType;
                            entity.Customer = dbCustomer;
                        }
                        entity.Customer.CreatedBy = entity.TenantId.ToString();
                    }

                    _billingServiceDbContext.Invoices.Add(entity);
                    await _billingServiceDbContext.SaveChangesAsync();

                    var payload = JsonConvert.SerializeObject(new
                    {
                        entity.Id,
                        entity.ExternalId,
                        entity.TenantId,
                        entity.Amount,
                        entity.ClientReferenceNumber,
                        entity.Currency,
                        entity.DueAt,
                        entity.InvoiceNumber,
                        entity.InvoiceStatus,
                        entity.InvoiceType,
                        entity.IssuedAt,
                        entity.Metadata,
                        Customer = new { entity.Customer?.Id, entity.Customer?.Vatnumber, entity.Customer?.AddressSubNumber, entity.Customer?.PhoneNumber, entity.Customer?.IdentityNumber, entity.Customer?.AddressBuildingNumber, entity.Customer?.AddressCity, entity.Customer?.AddressCountry, entity.Customer?.AddressDistrict, entity.Customer?.AddressPostalCode, entity.Customer?.CustomerType, entity.Customer?.EmailAddress },
                        InvoicesAttachments = entity.InvoicesAttachments.Select(a => new { a.InvoiceId, a.FileName }).ToList(),
                        InvoicesItems = entity.InvoicesItems.Select(i => new { i.InvoiceId, i.ItemDesc, i.Amount, i.ItemName, i.Quantity }).ToList(),
                    });
                    var outboxEvent = new OutboxEvent()
                    {
                        AggregateId = entity.Id,
                        EventType = EventsTypes.InvoiceCreated.ToString(),
                        Payload = payload,
                        Processed = false,
                        CreatedDate = DateTime.UtcNow
                    };
                    _billingServiceDbContext.OutboxEvents.Add(outboxEvent);

                    auditEvent.EntityId = entity.Id.ToString();
                    _billingServiceDbContext.AuditEvents.Add(auditEvent);

                    await _billingServiceDbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return entity.Id;
                }
                catch (Exception ex)
                {
                    ex.HandleException(_logger, _configuration);
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<long> Insert(Invoice entity)
        {
            using (var transaction = _billingServiceDbContext.Database.BeginTransaction())
            {
                try
                {

                    //var nextInvoiceNumber = await _billingServiceDbContext.Database
                    //    .SqlQueryRaw<long>("SELECT CAST(NEXT VALUE FOR dbo.InvoiceNumberSequence AS BIGINT) AS Value")
                    //    .FirstAsync();

                    //entity.InvoiceNumber += nextInvoiceNumber.ToString("000000");
                    entity.CreatedBy = entity.TenantId.ToString();
                    _billingServiceDbContext.Invoices.Add(entity);
                    //await _billingServiceDbContext.SaveChangesAsync();
                    _billingServiceDbContext.OutboxEvents.Add(
                        new OutboxEvent()
                        {
                            AggregateId = entity.Id,
                            EventType = EventsTypes.InvoiceCreated.ToString(),
                            Payload = "",
                            Processed = false,
                            CreatedDate = DateTime.UtcNow
                        });
                    await _billingServiceDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return entity.Id;
                }
                catch (Exception ex)
                {
                    ex.HandleException(_logger, _configuration);
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<IEnumerable<Invoice>> Query(Func<Invoice, bool> query)
        {
            var queryResults = _billingServiceDbContext.Invoices
                 .Include(i => i.InvoicesItems)
                 .Include(i => i.InvoicesAttachments)
                 .Include(i => i.Customer)
                 .Where(query)
                 .ToList();

            return queryResults;
        }

        public IEnumerable<Invoice> QueryPaging(Func<Invoice, bool> query, int pageSize, int pageNumber)
        {
            var queryResults = _billingServiceDbContext.Invoices
                 .Include(i => i.InvoicesItems)
                 .Include(i => i.InvoicesAttachments)
                 .Include(i => i.Customer)
                 .Where(query)
                 .Skip(pageSize * (pageNumber - 1))
                 .Take(pageSize)
                 .ToList();

            return queryResults;
        }

        public async Task<bool> Update(Invoice entity)
        {
            try
            {
                var dbInvoice = _billingServiceDbContext.Invoices.SingleOrDefault(p => p.Id == entity.Id);

                if (dbInvoice != null)
                {
                    if (entity.InvoiceStatus < 0) entity.InvoiceStatus = dbInvoice.InvoiceStatus;
                    if (string.IsNullOrEmpty(entity.Currency)) entity.Currency = dbInvoice.Currency;
                    if (entity.LastUpdatedDate == null) entity.LastUpdatedDate = DateTime.Now;
                    if (entity.TenantId < 0) entity.TenantId = dbInvoice.TenantId;
                    if (entity.InvoiceType < 0) entity.InvoiceType = dbInvoice.InvoiceType;
                    if (entity.DueAt == null) entity.DueAt = dbInvoice.DueAt;
                    if (entity.IssuedAt == null) entity.IssuedAt = dbInvoice.IssuedAt;
                    if (entity.CreatedBy == null) entity.CreatedBy = dbInvoice.CreatedBy;
                    if (entity.IsDeleted == null) entity.IsDeleted = dbInvoice.IsDeleted;
                    if (entity.CustomerId <= 0) entity.CustomerId = dbInvoice.CustomerId;
                    if (entity.Amount < 0) entity.Amount = dbInvoice.Amount;
                    if (entity.IsSubInvoice == null) entity.IsSubInvoice = dbInvoice.IsSubInvoice;

                    _billingServiceDbContext.Entry(dbInvoice).CurrentValues.SetValues(entity);
                    await _billingServiceDbContext.SaveChangesAsync();
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

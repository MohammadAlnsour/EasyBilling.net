using BillingSystem.Application.Contracts.Services;
using BillingSystem.Application.MapperRules;
using BillingSystem.Application.Requests;
using BillingSystem.Application.Services;
using BillingSystem.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BillingSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IInvoiceNumberGeneratorService, InvoiceNumberGeneratorService>();
            services.AddScoped<IIdempotentKeyService, IdempotentKeyValidatorService>();
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<IInvoiceAuditService, InvoiceAuditService>();
            services.AddScoped<IOutboxeventsService, OutboxeventsService>();

            services.AddScoped<IValidator<CreateInvoiceRequest>, CreateInvoiceValidator>();
            services.AddScoped<IValidator<UpdateInvoiceRequest>, UpdateInvoiceValidator>();
            services.AddAutoMapper(cfg => cfg.AddProfile<InvoiceMapperProfile>());
            services.AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            return services;

        }
    }
}

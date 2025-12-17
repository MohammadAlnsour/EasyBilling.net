using BillingSystem.Infrastructure.Bus;
using BillingSystem.Infrastructure.ConfigOptions;
using BillingSystem.Infrastructure.EF;
using BillingSystem.Infrastructure.Integration;
using BillingSystem.Infrastructure.Persistence.EF;
using BillingSystem.Infrastructure.Persistence.Interfaces;
using BillingSystem.SharedKernel;
using Consul;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace BillingSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            RabbitMqSettings rabbitMqSettings,
            RedisSettings redisSettings,
            ConsulSettings consulSettings)
        {
            //Configure the Consul client for DI
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = consulSettings.address;
                consulConfig.Address = new Uri(address);
            }));

            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IOutboxEventsRepository, OutboxEventsRepository>();
            services.AddDbContext<BillingServiceDBContext>((provider, options) =>
            {
                IConfiguration config = provider.GetRequiredService<IConfiguration>();
                string? connectionString = config.GetConnectionString("ReadWriteConnection");
                options.UseSqlServer(connectionString);
            });

            var redisConnectionString = $"{redisSettings?.server},password={redisSettings?.password}";
            services.AddSingleton(cfg =>
            {
                IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect($"{redisConnectionString}");
                return multiplexer.GetDatabase();
            });

            services.AddMassTransit(x =>
            {
                // Optional: Register consumers if you have any
                // x.AddConsumer<MyMessageConsumer>(); 

                x.UsingRabbitMq((context, cfg) =>
                {
                    // Configure RabbitMQ host
                    cfg.Host("rabbitmq://localhost", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    // Configure receive endpoints for consumers (if any)
                    // cfg.ReceiveEndpoint("my-queue-name", e =>
                    // {
                    //     e.ConfigureConsumer<MyMessageConsumer>(context);
                    // });

                    // Optional: Configure message retry policies, circuit breakers, etc.
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });
            });

            var rabbitMqServer = rabbitMqSettings?.HostName ?? "localhost";
            var rabbitMqUser = rabbitMqSettings?.UserName ?? "guest";
            var rabbitMqPassword = rabbitMqSettings?.Password ?? "guest";
            var rabbitMqVirtualHost = rabbitMqSettings?.VirtualHost ?? "/";

            services.AddSingleton<IConnection>(sp =>
            {
                var factory = new ConnectionFactory()
                {
                    HostName = rabbitMqServer, // Or read from configuration
                    UserName = rabbitMqUser,
                    Password = rabbitMqPassword,
                    VirtualHost = rabbitMqVirtualHost
                };
                return factory.CreateConnectionAsync().Result;
            });
            services.AddSingleton<IChannel>(sp =>
            {
                var connection = sp.GetRequiredService<IConnection>();
                return connection.CreateChannelAsync().Result;
            });
            services.AddSingleton<RetryUtil>();
            services.AddSingleton<RabbitMqBus>();
            services.AddScoped<IRESTClient, RESTClient>();
           
            return services;
        }
    }
}

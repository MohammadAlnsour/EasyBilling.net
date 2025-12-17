using BillingSystem.Application;
using BillingSystem.Infrastructure;
using BillingSystem.Infrastructure.ConfigOptions;
using BillingSystem.PaymentOrchestrator.IOptions;
using BillingSystem.PaymentOrchestrator.PaymentProviders;
using Consul;
using Serilog;
using System.Reflection;

namespace BillingSystem.PaymentOrchestrator
{
    public class Program
    {
        private static string env = "Dev";

        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            Log.Logger = new LoggerConfiguration()
             .ReadFrom.Configuration(builder.Configuration)
             .Enrich.WithProperty("ServiceName", Assembly.GetEntryAssembly()?.GetName().Name)
             .Enrich.WithProperty("Environment", env)
             .Enrich.FromLogContext()
             .CreateLogger();

            builder.Services.AddOptions<RedisSettings>().Bind(builder.Configuration.GetSection(RedisSettings.SectionName));
            builder.Services.AddOptions<RabbitMqSettings>().Bind(builder.Configuration.GetSection(RabbitMqSettings.SectionName));
            builder.Services.AddOptions<InvoiceCreatedQueueSettings>().Bind(builder.Configuration.GetSection(InvoiceCreatedQueueSettings.SectionName));
            builder.Services.AddOptions<IntegrationServiceSettings>().Bind(builder.Configuration.GetSection(IntegrationServiceSettings.SectionName));
            builder.Services.AddOptions<ConsulSettings>().Bind(builder.Configuration.GetSection(ConsulSettings.SectionName));


            var redisSettings = builder.Configuration.GetSection(RedisSettings.SectionName).Get<RedisSettings>();
            var rabbitmqSettings = builder.Configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>();
            var consulSettings = builder.Configuration.GetSection(ConsulSettings.SectionName).Get<ConsulSettings>();

            builder.Services.AddInfrastructureServices(rabbitmqSettings, redisSettings, consulSettings);
            builder.Services.AddApplicationServices();
            builder.Services.AddSingleton<PaymentAdapter>();
            builder.Services.AddSingleton<MessageHandler>();
            builder.Services.AddSingleton<Efaa>();
            builder.Services.AddSingleton<Tahseel>();
            builder.Services.AddSingleton<NPV>();
            builder.Services.AddSerilog(Log.Logger);
            builder.Services.AddHostedService<PaymentOrchestratorWorker>();

            var host = builder.Build();
            host.Run();
        }
    }
}
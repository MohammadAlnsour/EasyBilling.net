using BillingSystem.Application;
using BillingSystem.Infrastructure;
using BillingSystem.Infrastructure.ConfigOptions;
using BillingSystem.OutboxProcessor.IOptions;
using BillingSystem.SharedKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Reflection;

namespace BillingSystem.OutboxProcessor
{
    public class Program
    {
        private static string env = "Dev";

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                          .AddJsonFile("appsettings.json", optional: false)
                          .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.WithProperty("ServiceName", Assembly.GetEntryAssembly()?.GetName().Name)
                .Enrich.WithProperty("Environment", env)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                var builder = Host.CreateApplicationBuilder(args);

                builder.Services.AddOptions<RedisSettings>().Bind(builder.Configuration.GetSection(RedisSettings.SectionName));
                builder.Services.AddOptions<RabbitMqSettings>().Bind(builder.Configuration.GetSection(RabbitMqSettings.SectionName));
                builder.Services.AddOptions<WorkerSettings>().Bind(builder.Configuration.GetSection(WorkerSettings.SectionName));
                builder.Services.AddOptions<OutboxProcessorRabbitMqSettings>().Bind(builder.Configuration.GetSection(OutboxProcessorRabbitMqSettings.SectionName));

                var redisSettings = builder.Configuration.GetSection(RedisSettings.SectionName).Get<RedisSettings>();
                var rabbitmqSettings = builder.Configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>();
                var consulSettings = builder.Configuration.GetSection(ConsulSettings.SectionName).Get<ConsulSettings>();

                builder.Services.AddInfrastructureServices(rabbitmqSettings, redisSettings, consulSettings);
                builder.Services.AddApplicationServices();
                builder.Services.AddSerilog(Log.Logger);
                builder.Services.AddHostedService<OutboxProcessorWorker>();

                var host = builder.Build();
                host.Run();
            }
            catch (Exception ex)
            {
                ex.HandleException(Log.Logger, config);
                throw;
            }


        }
    }
}
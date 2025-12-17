using BillingSystem.API.ActionFilters;
using BillingSystem.API.Middlewares;
using BillingSystem.Application;
using BillingSystem.Infrastructure;
using BillingSystem.Infrastructure.ConfigOptions;
using BillingSystem.Infrastructure.Consul;
using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using StackExchange.Redis;
using System.Reflection;

namespace BillingSystem.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, services, configuration) => configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.WithProperty("ServiceName", Assembly.GetEntryAssembly()?.GetName().Name)
                        .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
                        .Enrich.FromLogContext());

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddOptions<RedisSettings>().Bind(builder.Configuration.GetSection(RedisSettings.SectionName));
            builder.Services.AddOptions<RabbitMqSettings>().Bind(builder.Configuration.GetSection(RabbitMqSettings.SectionName));
            builder.Services.AddOptions<ConsulSettings>().Bind(builder.Configuration.GetSection(ConsulSettings.SectionName));

            var redisSettings = builder.Configuration.GetSection(RedisSettings.SectionName).Get<RedisSettings>();
            var rabbitmqSettings = builder.Configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>();
            var consulSettings = builder.Configuration.GetSection(ConsulSettings.SectionName).Get<ConsulSettings>();

            builder.Services.AddInfrastructureServices(rabbitmqSettings, redisSettings, consulSettings);
            builder.Services.AddApplicationServices();

            IServiceProvider temporaryServiceProvider = builder.Services.BuildServiceProvider();
            var consulClient = temporaryServiceProvider.GetRequiredService<IConsulClient>();
            var serilogLogger = temporaryServiceProvider.GetRequiredService<Serilog.ILogger>();
            var redisDB = temporaryServiceProvider.GetRequiredService<IDatabase>();

            var indentityServerSettings = await ConsulHelper.ReadKeyValueFromConsulAsync<IdentityServerSettings>(consulClient, "BillingService/BillingAPI-IdentityServer", serilogLogger, builder.Configuration, redisDB);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = indentityServerSettings.Authority;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidateAudience = false;
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScopePolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "scope2");
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAdminUI",
                    builder => builder
                    .WithOrigins("http://localhost:5094", "http://localhost:8365", "https://localhost:44345", "https://billingAdminUI.local")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    );
            });


            var app = builder.Build();

            app.UseSerilogRequestLogging();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAdminUI");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseMiddleware<CorrelationMiddleware>();
            app.UseMiddleware<ServiceNameLogContextMiddleware>();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<CorrelationIdHeaderValidatorMiddleware>();
            app.UseMiddleware<TenantIdValidatorMiddleware>();

            app.MapGet("/health", () => "OK");

            app.Run();
        }
    }
}

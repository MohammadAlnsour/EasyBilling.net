using BillingSystem.Application.Contracts.Services;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.enums;
using BillingSystem.Infrastructure.Bus;
using BillingSystem.OutboxProcessor.IOptions;
using BillingSystem.SharedKernel;
using Microsoft.Extensions.Options;
using ILogger = Serilog.ILogger;

namespace BillingSystem.OutboxProcessor
{
    public class OutboxProcessorWorker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly RabbitMqBus _rabbitMqBus;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IOptions<WorkerSettings> _workerSettings;
        private readonly IOptions<OutboxProcessorRabbitMqSettings> _rabbitMqSettings;

        //private SqlTableDependency<OutboxEvent> _dependency;
        //private readonly string _connectionString;

        public OutboxProcessorWorker(
            ILogger logger,
            IConfiguration configuration,
            RabbitMqBus rabbitMqBus,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<WorkerSettings> workerSettings,
            IOptions<OutboxProcessorRabbitMqSettings> rabbitMqSettings)
        {
            _logger = logger;
            _configuration = configuration;
            _rabbitMqBus = rabbitMqBus;
            _serviceScopeFactory = serviceScopeFactory;
            _workerSettings = workerSettings;
            _rabbitMqSettings = rabbitMqSettings;
            //_connectionString = configuration.GetConnectionString("ReadWriteConnection");

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
                        _logger.Information("Worker running at: {time}", DateTimeOffset.Now);

                    var timerInSeconds = _workerSettings.Value.RunEveryInSeconds;
                    await ProcessBulkInvoicesAsync();
                    await Task.Delay(TimeSpan.FromSeconds(timerInSeconds), stoppingToken);
                }
                catch (Exception ex)
                {
                    ex.HandleException(_logger, _configuration);
                }
                finally
                {
                    //_dependency?.Stop();
                }
            }
        }

        private async Task ProcessBulkInvoicesAsync()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _outBoxService = scope.ServiceProvider.GetRequiredService<IOutboxeventsService>();
                var bulkcount = _workerSettings.Value.BulkInvoicesCount;
                var queueName = _workerSettings.Value.InvoiceCreatedQueueName;
                var exchangeName = _rabbitMqSettings.Value.ExchangeName;
                var dlqName = _rabbitMqSettings.Value.DLQQueueName;

                var outboxInvoices = await _outBoxService.FetchBulkUnProcessedOutboxEvents(bulkcount);
                foreach (var outboxEvent in outboxInvoices)
                {
                    await CheckEventTypeAndPublishMessage(_outBoxService, queueName, exchangeName, dlqName, outboxEvent);
                }
            }
        }

        private async Task CheckEventTypeAndPublishMessage(
            IOutboxeventsService _outBoxService,
            string queueName,
            string exchangeName,
            string dlqName,
            OutboxEvent outboxEvent)
        {
            using (Serilog.Context.LogContext.PushProperty("CorrelationId", outboxEvent.CorrelationId))
            {
                try
                {
                    var eventType = (EventsTypes)Enum.Parse(typeof(EventsTypes), outboxEvent.EventType);
                    switch (eventType)
                    {
                        case EventsTypes.InvoiceCreated: queueName = _workerSettings.Value.InvoiceCreatedQueueName; break;
                        case EventsTypes.InvoiceUpdated: queueName = ""; break;
                        case EventsTypes.InvoiceCancelled: queueName = ""; break;
                        case EventsTypes.PaymentFailed: queueName = ""; break;
                        case EventsTypes.PaymentCancelled: queueName = ""; break;
                        case EventsTypes.PaymentSucceed: queueName = ""; break;
                        case EventsTypes.PartiallyPayed: queueName = ""; break;
                        case EventsTypes.PendingPayment: queueName = ""; break;
                        case EventsTypes.FailedSendingToProvider: queueName = ""; break;
                    }
                    await _rabbitMqBus.PublishMessageAsync(outboxEvent.Payload, queueName, exchangeName, dlqName);
                    _logger.Information($"Outbox Event: {outboxEvent.EventType} published to queue: {queueName}  successfully.");
                    await _outBoxService.UpdateOutboxEventProcessed(outboxEvent);
                    _logger.Information($"Outbox Event: {outboxEvent.EventType} updated in outbox DB table successfully.");
                }
                catch (Exception ex)
                {
                    ex.HandleException(_logger, _configuration);
                    _logger.Error($"Error occured processing outbox event message: {ex.Message} , stackstrace: {ex.StackTrace}");
                    outboxEvent.ErrorMsg = $"Error occured : {ex.Message}  ," +
                        $" stack trace : {ex.StackTrace} ," +
                        $" inner exception : {ex.InnerException?.Message}";
                    await _outBoxService.UpdateOutboxEventError(outboxEvent);
                    throw;
                }
            }
        }

        //private void StartTableDependency()
        //{
        //    _dependency = new SqlTableDependency<OutboxEvent>(_connectionString, "OutboxEvents");
        //    _dependency.OnChanged += TableDependency_OnChanged;
        //    _dependency.OnError += TableDependency_OnError;
        //    _dependency.OnStatusChanged += TableDependency_OnStatusChanged;
        //    _dependency.Start();
        //}
        //private void TableDependency_OnStatusChanged(object sender, StatusChangedEventArgs e)
        //{
        //    _logger.Information("SqlTableDependency status changed to: {status}", e.Status);
        //}
        //private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        //{
        //    _logger.Error(e.Error, "Error in SqlTableDependency: {message}", e.Error.Message);
        //    // Attempt to restart the dependency after an error.
        //    RestartTableDependency();
        //}
        //private void TableDependency_OnChanged(object sender, RecordChangedEventArgs<OutboxEvent> e)
        //{
        //    if (e.ChangeType != ChangeType.None && e.ChangeType == ChangeType.Insert)
        //    {
        //        // The record that was inserted/updated/deleted
        //        var changedRecord = e.Entity;
        //        _logger.Information("Change detected for OutboxEvent with ID: {id}", changedRecord.Id);
        //        // Process the event here.
        //        // e.g., Publish message to RabbitMQ.
        //        // You may need to inject the RabbitMQ client service here using a DI scope.
        //        if (!changedRecord.Processed)
        //        {
        //            using (var scope = _serviceScopeFactory.CreateScope())
        //            {
        //                var _outBoxService = scope.ServiceProvider.GetRequiredService<IOutboxeventsService>();
        //                lock (_rabbitMqBus)
        //                {
        //                    _rabbitMqBus.PublishMessageToQueueAsync(changedRecord, "InvoiceCreatedQueue").Wait();
        //                }
        //                lock (_outBoxService)
        //                {
        //                    changedRecord.Processed = true;
        //                    _outBoxService.UpdateOutboxEventProcessed(changedRecord).Wait();
        //                }
        //            }

        //        }
        //    }
        //}
        //private void RestartTableDependency()
        //{
        //    _dependency.Stop();
        //    _dependency.OnChanged -= TableDependency_OnChanged;
        //    _dependency.OnError -= TableDependency_OnError;
        //    _dependency.OnStatusChanged -= TableDependency_OnStatusChanged;
        //    StartTableDependency();
        //}

    }
}

using BillingSystem.Domain.Entities;
using BillingSystem.PaymentOrchestrator.DLQ;
using BillingSystem.PaymentOrchestrator.IOptions;
using BillingSystem.SharedKernel;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using ILogger = Serilog.ILogger;

namespace BillingSystem.PaymentOrchestrator
{
    public class PaymentOrchestratorWorker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IChannel _rabbitMqChannel;
        private readonly MessageHandler _messageHandler;
        private readonly IOptions<InvoiceCreatedQueueSettings> _queueSettings;


        public PaymentOrchestratorWorker(
            ILogger logger,
            IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory,
            IChannel rabbitMqChannel,
            MessageHandler messageHandler,
            IOptions<InvoiceCreatedQueueSettings> queueSettings)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _rabbitMqChannel = rabbitMqChannel;
            _messageHandler = messageHandler;
            _queueSettings = queueSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queueName = _queueSettings.Value.QueueName;
            await QueuesConfig.DeclareQueuesAsync(_rabbitMqChannel, _queueSettings);

            var consumer = new AsyncEventingBasicConsumer(_rabbitMqChannel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var retryCount = GetValueFromMessageHeaders(ea.BasicProperties, "x-retry-count");
                var correlationId = ea.BasicProperties.CorrelationId;

                using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);
                        var invoice = JsonConvert.DeserializeObject<Invoice>(json);

                        _messageHandler.ProcessInvoiceMessage(invoice!);
                        await _rabbitMqChannel.BasicAckAsync(ea.DeliveryTag, false);
                        _logger.Information($"Processed message: invoice Id: {invoice?.Id}");
                    }
                    catch (Exception ex)
                    {
                        ex.HandleException(_logger, _configuration);
                        _logger.Error(ex, "Error processing message (Attempt {Retry})", retryCount);

                        if (retryCount == 0)
                        {
                            // First failure — requeue it
                            await RequeueMessageAsync(_rabbitMqChannel, ea);
                            await _rabbitMqChannel.BasicAckAsync(ea.DeliveryTag, false);
                        }
                        else
                        {
                            // Second failure — move to DLQ
                            await MoveToDeadLetterAsync(_rabbitMqChannel, ea);
                            await _rabbitMqChannel.BasicAckAsync(ea.DeliveryTag, false);
                        }
                    }
                }
            };

            await _rabbitMqChannel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

            _logger.Information("RabbitMQ Consumer started for queue {Queue}", queueName);
            // return Task.CompletedTask;
        }

        private static int GetValueFromMessageHeaders(IReadOnlyBasicProperties props, string propName)
        {
            if (props.Headers != null && props.Headers.TryGetValue(propName, out var value))
            {
                if (value is byte[] bytes && int.TryParse(Encoding.UTF8.GetString(bytes), out var parsed))
                    return parsed;
                if (value is int i) return i;
            }
            return 0;
        }

        private async Task RequeueMessageAsync(IChannel channel, BasicDeliverEventArgs ea)
        {
            var exchangeName = _queueSettings.Value.ExchangeName;
            var mainQueueName = _queueSettings.Value.QueueName;
            var dlq = _queueSettings.Value.DLQQueueName;
            var correlationID = ea.BasicProperties.CorrelationId;

            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationID))
            {
                var props = new BasicProperties
                {
                    Persistent = true,
                    Headers = new Dictionary<string, object> { ["x-retry-count"] = 1 },
                    CorrelationId = correlationID,
                };
                await channel.BasicPublishAsync(
                    exchange: exchangeName,
                    routingKey: mainQueueName,
                    mandatory: false,
                    props,
                    ea.Body);

                _logger.Warning("Message requeued for retry (Attempt 1)");
            }
        }
        private async Task MoveToDeadLetterAsync(IChannel channel, BasicDeliverEventArgs ea)
        {
            var correlationID = ea.BasicProperties.CorrelationId;
            var props = new BasicProperties { Persistent = true, CorrelationId = correlationID };

            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationID))
            {
                var exchangeName = _queueSettings.Value.ExchangeName;
                var mainQueueName = _queueSettings.Value.QueueName;
                var dlq = _queueSettings.Value.DLQQueueName;

                await channel.BasicPublishAsync(
                    exchange: exchangeName,
                    routingKey: dlq,
                    mandatory: false,
                    props,
                    ea.Body);

                _logger.Warning("Message moved to DLQ after 2 failures");
            }
        }

    }
}

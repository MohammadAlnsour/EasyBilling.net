using BillingSystem.SharedKernel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;
using System.Text;

namespace BillingSystem.Infrastructure.Bus
{
    public class RabbitMqBus
    {
        private readonly IChannel _rabbitMqChannel;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly RetryUtil _retryUtil;

        public RabbitMqBus(IChannel rabbitMqChannel, IConfiguration configuration, ILogger logger, RetryUtil retryUtil)
        {
            _rabbitMqChannel = rabbitMqChannel;
            _configuration = configuration;
            _logger = logger;
            _retryUtil = retryUtil;
        }

        public async Task PublishMessageAsync<T>(T message, string queueName, string exchangeName, string deadLetterQueueName, string correlationId = "")
        {
            var serializedMessage = string.Empty;
            byte[] messageBytes;

            if (message is string jsonString && Utilities.IsJson(jsonString))
            {
                messageBytes = Encoding.UTF8.GetBytes(jsonString);
            }
            else
            {
                serializedMessage = JsonConvert.SerializeObject(message);
                messageBytes = Encoding.UTF8.GetBytes(serializedMessage);
            }

            var action = async () =>
            {
                await _rabbitMqChannel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                { "x-dead-letter-exchange", exchangeName },
                { "x-dead-letter-routing-key", deadLetterQueueName },
                });
                var properties = new BasicProperties();
                properties.CorrelationId = correlationId;
                properties.ContentType = "application/json";
                properties.DeliveryMode = DeliveryModes.Persistent;
                await _rabbitMqChannel.QueueBindAsync(queueName, exchangeName, queueName);
                await _rabbitMqChannel.BasicPublishAsync(exchange: "", routingKey: queueName, mandatory: false, basicProperties: properties, body: messageBytes);
            };
            await _retryUtil.ExecuteRetry(action);
        }

    }
}

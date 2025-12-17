using BillingSystem.PaymentOrchestrator.IOptions;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BillingSystem.PaymentOrchestrator.DLQ
{
    public static class QueuesConfig
    {
        public static async Task DeclareQueuesAsync(IChannel channel, IOptions<InvoiceCreatedQueueSettings> options)
        {
            var queueName = options.Value.QueueName;
            var deadLetterQueueName = options.Value.DLQQueueName;
            var exchangeName = options.Value.ExchangeName;


            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: true);

            // DLQ
            await channel.QueueDeclareAsync(deadLetterQueueName, durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(deadLetterQueueName, exchangeName, deadLetterQueueName);

            // Main Queue with DLX
            await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                { "x-dead-letter-exchange", exchangeName },
                { "x-dead-letter-routing-key", deadLetterQueueName }
                });
            await channel.QueueBindAsync(queueName, exchangeName, queueName);

        }
    }
}

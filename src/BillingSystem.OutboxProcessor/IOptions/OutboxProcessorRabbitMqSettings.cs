namespace BillingSystem.OutboxProcessor.IOptions
{
    public class OutboxProcessorRabbitMqSettings
    {
        public const string SectionName = "rabbitMQ";
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string QueueName { get; set; }
        public string DLQQueueName { get; set; }
        public string ExchangeName { get; set; }
        public int MaxRetries { get; set; }

    }
}

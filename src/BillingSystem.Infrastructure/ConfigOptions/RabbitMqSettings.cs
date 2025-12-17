namespace BillingSystem.Infrastructure.ConfigOptions
{
    public class RabbitMqSettings
    {
        public const string SectionName = "RabbitMq";

        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
    }
}

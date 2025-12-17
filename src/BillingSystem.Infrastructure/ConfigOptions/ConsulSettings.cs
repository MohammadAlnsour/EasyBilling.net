namespace BillingSystem.Infrastructure.ConfigOptions
{
    public class ConsulSettings
    {
        public const string SectionName = "Consul";

        public string address { get; set; }
        public string BaseFolderName { get; set; }
        public ConsulService Service { get; set; }
    }

    public class ConsulService
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string[] Tags { get; set; }
    }
}

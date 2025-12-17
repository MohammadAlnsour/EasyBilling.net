namespace BillingSystem.Infrastructure.ConfigOptions
{
    public class IntegrationServiceSettings
    {
        public const string SectionName = "IntegrationService";
        public string Base { get; set; }
        public string NPVURl { get; set; }
        public string TahseelURl { get; set; }
    }
}

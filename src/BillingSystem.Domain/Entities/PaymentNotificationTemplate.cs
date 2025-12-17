namespace BillingSystem.Domain.Entities
{
    public partial class PaymentNotificationTemplate
    {
        public long Id { get; set; }
        public string TemplateName { get; set; } = null!;
        public string Template { get; set; } = null!;
        public int? NotificationType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public bool IsEnabled { get; set; }
    }
}

namespace BillingSystem.OutboxProcessor.IOptions
{
    public class WorkerSettings
    {
        public const string SectionName = "WorkerSettings";
        public int RunEveryInSeconds { get; set; }
        public int BulkInvoicesCount { get; set; }
        public string InvoiceCreatedQueueName { get; set; }
    }
}

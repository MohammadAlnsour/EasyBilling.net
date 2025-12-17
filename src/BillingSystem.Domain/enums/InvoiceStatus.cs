namespace BillingSystem.Domain.enums
{
    public enum InvoiceStatus
    {
        InvoiceCreated = 1,
        InvoiceUpdated = 2,
        InvoiceCancelled = 3,
        PendingPayment = 4,
        PaymentSucceed = 5,
        PaymentFailed = 6,
        PaymentCancelled = 7,
        PartiallyPayed = 8,
        FailedSendingToProvider = 9,
    }
}

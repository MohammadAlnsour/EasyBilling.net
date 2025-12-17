using BillingSystem.Domain.Entities;
using BillingSystem.PaymentOrchestrator.PaymentProviders.Requests;
using Newtonsoft.Json;

namespace BillingSystem.PaymentOrchestrator.PaymentProviders.IntegrationMappers
{
    public static class InvoiceToNPVFine
    {
        public static NPVFineRequest Map(Invoice invoice)
        {
            var fine = new NPVFineRequest();

            if (invoice == null) throw new ArgumentNullException("invoice");

            var metaData = invoice.Metadata;
            if (metaData == null) throw new ArgumentNullException("invoice");

            var fineMetadata = JsonConvert.DeserializeObject<NPVFineMetadata>(metaData.ToString());
            TimeSpan difference = (TimeSpan)(invoice.DueAt - invoice.IssuedAt);

            fine.cityHappened = fineMetadata.cityHappened;
            fine.dateHappened = invoice.IssuedAt;
            fine.daysToBeEffective = difference.Days;
            fine.fineBookingType = 1;
            fine.fineBusinessRef = invoice.InvoiceNumber;
            fine.fineGroupCode = fineMetadata.fineGroupCode;
            fine.fineLocType = 0;
            fine.fineSubjectType = 0;
            fine.groupFineDetails = new List<GroupFineDetail>() { new GroupFineDetail() { countOrIntervals = 0, detailDaysMaxValue = difference.Days, detailFineAmount = (int)invoice.Amount, detailFineType = fineMetadata.detailFineType, detailPaymentBlockInd = 0, detailRegSecKey = string.Empty } };
            fine.locDetails = "";
            fine.regPersonID = fineMetadata.regPersonID;
            fine.registerAgent = fineMetadata.regPersonID;
            fine.remarks = fineMetadata.remarks;
            fine.resolutionRef = fineMetadata.resolutionRef;
            fine.resolutionSource = fineMetadata.resolutionSource;
            fine.resolutionType = fineMetadata.resolutionType;
            fine.securityOperatorID = fineMetadata.securityOperatorID;
            fine.timeHappened = invoice.IssuedAt.ToShortTimeString();
            fine.violatorID = Convert.ToInt32(invoice.Customer.IdentityNumber);

            return fine;
        }
    }

    public class NPVFineMetadata
    {
        public int cityHappened { get; set; }
        public int fineGroupCode { get; set; }
        public int detailFineType { get; set; }
        public int regPersonID { get; set; }
        public string remarks { get; set; }
        public string resolutionRef { get; set; }
        public int resolutionSource { get; set; } = 0;
        public int resolutionType { get; set; } = 0;
        public int securityOperatorID { get; set; }
    }
}

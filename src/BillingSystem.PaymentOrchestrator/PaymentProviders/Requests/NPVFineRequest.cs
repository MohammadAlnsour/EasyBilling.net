namespace BillingSystem.PaymentOrchestrator.PaymentProviders.Requests
{
    public class NPVFineRequest
    {
        public int cityHappened { get; set; }
        public DateTime dateHappened { get; set; }
        public int daysToBeEffective { get; set; }
        public int fineBookingType { get; set; }
        public string fineBusinessRef { get; set; }
        public int fineGroupCode { get; set; }
        public int fineLocType { get; set; }
        public object fineSubjectRef { get; set; }
        public int fineSubjectType { get; set; }
        public List<GroupFineDetail> groupFineDetails { get; set; }
        public string locDetails { get; set; }
        public int regPersonID { get; set; }
        public int registerAgent { get; set; }
        public string remarks { get; set; }
        public string resolutionRef { get; set; }
        public int resolutionSource { get; set; }
        public int resolutionType { get; set; }
        public int securityOperatorID { get; set; }
        public string timeHappened { get; set; }
        public int violatorID { get; set; }
    }
    public class GroupFineDetail
    {
        public int countOrIntervals { get; set; }
        public int detailDaysMaxValue { get; set; }
        public int detailFineAmount { get; set; }
        public int detailFineType { get; set; }
        public int detailPaymentBlockInd { get; set; }
        public string detailRegSecKey { get; set; }
    }
}
